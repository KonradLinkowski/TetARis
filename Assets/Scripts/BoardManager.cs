using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
	public class BoardManager : MonoBehaviour {

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

		private bool[,] board;

		[SerializeField]
		private Transform leftTopCorner;
		[SerializeField]
		private Transform rightBottomCorner;
		[SerializeField]
		private Transform spawnPoint;

		// Use this for initialization
		void Start () {
			board = new bool[width, height];
			currentBlock = randomBlock();
			blockQueue = new Queue<Block>();
			fillQueue();
			spawnNewBlock();
		}
		
		// Update is called once per frame
		void Update () {
			timer += Time.deltaTime;
			if (timer >= fallTime) {
				// spawnNewBlock();
				moveBlock();
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
			fillQueue();
		}

		void moveBlock() {
			Vector3 transition = -currentBlock.transform.up;
			currentBlock.transform.Translate(transition);
		}

		void drop() {

		}

		void swapWithStash() {

		}
	}
}
