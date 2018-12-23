﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
	public class BoardManager : MonoBehaviour {

		public static BoardManager Instance;

		[SerializeField]
		private Block[] blocksPrefabs;

		private Block currentBlock;

		private Block stash = null;
		private Queue<Block> blockQueue;
		[SerializeField]
		private int queueLength;
		private float timer;
		public float fallTime;

		public int width;
		public int height;

		private float stepHeight;
		private float stepWidth;

		private bool[,] board;

		[SerializeField]
		private Transform leftTopCorner;
		[SerializeField]
		private Transform rightBottomCorner;
		[SerializeField]
		private Transform spawnPoint;

		void OnEnable() {
			Instance = this;
		}

		void Start () {
			stepHeight = Mathf.Abs(rightBottomCorner.position.y - leftTopCorner.position.y) / height;
			stepWidth = Mathf.Abs(rightBottomCorner.position.x - leftTopCorner.position.x) / width;
			board = new bool[width, height];
			currentBlock = randomBlock();
			blockQueue = new Queue<Block>();
			fillQueue();
			spawnNewBlock();
		}
		
		// Update is called once per frame
		void FixedUpdate () {
			timer += Time.deltaTime;
			if (timer >= fallTime) {
				// spawnNewBlock();
				Vector2 transition = new Vector2(0, -stepHeight);
				// if (canMoveBlock(transition)) {
				// 	moveBlock(transition);
				// } else {
				// 	sealBlock();
				// }
				timer = 0;
			}
		}

		Block randomBlock() {
			return blocksPrefabs[Random.Range(0, blocksPrefabs.Length)];
		}

		void fillQueue() {
			while (blockQueue.Count < queueLength) {
				blockQueue.Enqueue(randomBlock());
			}
		}

		void spawnNewBlock() {
			currentBlock = Instantiate(blockQueue.Dequeue(), spawnPoint.position, Quaternion.identity);
			currentBlock.transform.Translate(
				currentBlock.transform.position.x - currentBlock.startingPoint.position.x,
				currentBlock.transform.position.y - currentBlock.startingPoint.position.y,
				0
			);
			fillQueue();
		}

		bool canMoveBlock(Vector2 transition) {
			Utils.Log("new");
			foreach (GameObject block in currentBlock.chunks) {
				Vector2 desiredPosition = (Vector2)block.transform.position + transition;
				Vector2Int boardDesiredPos = new Vector2Int(
					Mathf.FloorToInt((desiredPosition.x - leftTopCorner.position.x) / stepWidth),
					Mathf.FloorToInt((desiredPosition.y - rightBottomCorner.position.y) / stepHeight)
				);
				// Utils.Log(desiredPosition.y, rightBottomCorner.position.y, desiredPosition.y - rightBottomCorner.position.y, boardDesiredPos.y);
				// Utils.Log(boardDesiredPos);
				if (boardDesiredPos.x >= width
					|| boardDesiredPos.x < 0
					|| boardDesiredPos.y < 0) {
						return false;
				}
				if (boardDesiredPos.y < height && board[boardDesiredPos.x, boardDesiredPos.y]) {
					return false;
				}
			}
			return true;
		}

		void moveBlock(Vector2 transition) {
			currentBlock.transform.Translate(transition);
		}

		void sealBlock() {
			foreach (GameObject block in currentBlock.chunks) {
				Vector2Int boardPosition = new Vector2Int(
					Mathf.FloorToInt((block.transform.position.x - leftTopCorner.position.x) / stepWidth),
					Mathf.FloorToInt((block.transform.position.y - rightBottomCorner.position.y) / stepHeight)
				);
				board[boardPosition.x, boardPosition.y] = true;
			}
			currentBlock.transform.DetachChildren();
			Destroy(currentBlock.gameObject);
			spawnNewBlock();
		}

		public void Move(Vector2 direction) {
			Vector2 transition = direction * stepWidth;
			if (canMoveBlock(transition)) {
				moveBlock(transition);
			}
		}

		public void Drop() {
			Vector2 transition = new Vector2(0, -stepHeight);
			while (canMoveBlock(transition)) {
				moveBlock(transition);
			}
			sealBlock();
		}

		public void SwapWithStash() {
			// if (stash == null) {
			// 	stash = currentBlock;
			// 	// stash.gameObject
			// 	spawnNewBlock();
			// 	return;
			// }
			Utils.LogBoard(board, "o", "x");
		}
	}
}
