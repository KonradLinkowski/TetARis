using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
	public class BoardManager : MonoBehaviour {

		public static BoardManager Instance;

		[SerializeField]
		private Block[] blocksPrefabs;

		private Block currentBlock;

		private Block.Type? stash = null;
		private Queue<Block> blockQueue;
		[SerializeField]
		private int queueLength;
		private float timer;
		public float fallTime;

		public int width;
		public int height;

		private float stepHeight;
		private float stepWidth;

		private GameObject[,] board;

		[SerializeField]
		private Transform leftTopCorner;
		[SerializeField]
		private Transform rightBottomCorner;
		[SerializeField]
		private Transform spawnPoint;

		private bool recalc = false;

		public bool gameOver = false;

		private bool stashed = false;

		void OnEnable() {
			Instance = this;
		}

		void Start () {
			stepHeight = Mathf.Abs(rightBottomCorner.position.y - leftTopCorner.position.y) / height;
			stepWidth = Mathf.Abs(rightBottomCorner.position.x - leftTopCorner.position.x) / width;
			board = new GameObject[width, height];
			currentBlock = randomBlock();
			blockQueue = new Queue<Block>();
			fillQueue();
			spawnNewBlock(null, null);
		}
		
		void FixedUpdate () {
			if (!recalc && !gameOver) {
				timer += Time.deltaTime;
				if (timer >= fallTime) {
					Vector2 transition = new Vector2(0, -stepHeight);
					if (canMoveBlock(transition)) {
						moveBlock(transition);
					} else {
						recalculation();
					}
					timer = 0;
				}
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

		void spawnNewBlock(Vector3? position, Block.Type? pref) {
			Block prefab = pref != null ? blocksPrefabs[(int)pref] : blockQueue.Dequeue();
			Vector3 stashOffset = Vector3.zero;
			if (pref != null) {
				stashOffset = prefab.startingPoint.localPosition - currentBlock.startingPoint.localPosition;
			}
			currentBlock = Instantiate(
				prefab,
				position != null ? (Vector3)position : spawnPoint.position,
				Quaternion.identity
			);
			currentBlock.transform.Translate(stashOffset);
			// if (position == null) {
				Vector3 offset = currentBlock.startingPoint.position - currentBlock.transform.position;
				currentBlock.transform.Translate(offset);
			// }
			currentBlock.type = prefab.type;
			fillQueue();
		}

		bool canMoveBlock(Vector2 transition) {
			foreach (GameObject block in currentBlock.chunks) {
				Vector2 desiredPosition = (Vector2)block.transform.position + transition;
				Vector2Int boardDesiredPos = new Vector2Int(
					Mathf.FloorToInt((desiredPosition.x - leftTopCorner.position.x) / stepWidth),
					Mathf.FloorToInt((desiredPosition.y - rightBottomCorner.position.y) / stepHeight)
				);
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
				if (boardPosition.y >= height) {
					gameOver = true;
					break;
				}
				board[boardPosition.x, boardPosition.y] = block;
			}
			currentBlock.transform.DetachChildren();
			Destroy(currentBlock.gameObject);
		}

		private void recalculation() {
			recalc = true;
			sealBlock();
			tryToClearLines();
			spawnNewBlock(null, null);
			recalc = false;
			stashed = false;
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
			recalculation();
		}

		public void Rotate(bool counterClockwise) {
			if(canRotateBlock(counterClockwise)) {
				rotateBlock(counterClockwise);
			}
		}

		private bool canRotateBlock(bool counterClockwise) {
			foreach (GameObject block in currentBlock.chunks) {
				Vector2 desiredPosition = 
					Quaternion.Euler(0, 0, 90 * (counterClockwise ? 1 : -1))
					* (block.transform.position - currentBlock.pivot.position)
					+ currentBlock.pivot.position;
				Vector2Int boardDesiredPos = new Vector2Int(
					Mathf.FloorToInt((desiredPosition.x - leftTopCorner.position.x) / stepWidth),
					Mathf.FloorToInt((desiredPosition.y - rightBottomCorner.position.y) / stepHeight)
				);
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

		private void rotateBlock(bool counterClockwise) {
			foreach (GameObject block in currentBlock.chunks) {
				block.transform.RotateAround(
					currentBlock.pivot.position,
					Vector3.forward,
					90 * (counterClockwise ? 1 : -1)
				);
			}
		}

		private void tryToClearLines() {
			for (int row =  board.GetLength(1) - 1; row >= 0;) {
				for (int el = 0; el <= board.GetLength(0); el += 1) {
					if (el == board.GetLength(0)) {
						clearLine(row);
					} else if (!board[el, row]) {
						row -= 1;
						break;
					}
				}
			}
		}

		private void clearLine(int line) {
			for (int el = 0; el < board.GetLength(0); el += 1) {
				Destroy(board[el, line]);
				board[el, line] = null;
			}
			for (int row = line + 1; row < board.GetLength(1); row += 1) {
				for (int el = 0; el < board.GetLength(0); el += 1) {
					if (board[el, row]) {
						board[el, row].transform.Translate(Vector2.down * stepHeight, Space.World);
						board[el, row - 1] = board[el, row];
						board[el, row] = null;
					}
				}
			}
		}

		public void SwapWithStash() {
			// if (!stashed) {
			// 	stashed = true;
			// 	Block.Type temp = currentBlock.type;
			// 	Destroy(currentBlock.gameObject);
			// 	spawnNewBlock(currentBlock.transform.position, stash);
			// 	stash = temp;
			// }
		}

		public void PrintBoard() {
			Utils.LogBoard(board, "x", " ");
		}
	}
}
