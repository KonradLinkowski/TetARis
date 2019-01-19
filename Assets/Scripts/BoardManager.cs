using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

namespace TetARis.Core {
	public class BoardManager : MonoBehaviour {

		public static BoardManager Instance;

		[SerializeField]
		private Block[] blocksPrefabs;

		private Block currentBlock;

		private Block.Type? stash = null;

		[SerializeField]
		private Transform[] queuePositions;
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

		private bool _paused = false;

		public bool Paused {
			get {
				return _paused;
			}
			set {
				MenuManager.Instance.gameObject.SetActive(value);
				_paused = value;
			}
		}
		public bool undetected = true;

		void OnEnable() {
			Instance = this;
		}

		void Start () {
			stepHeight = Mathf.Abs(rightBottomCorner.localPosition.y - leftTopCorner.localPosition.y) / height;
			stepWidth = Mathf.Abs(rightBottomCorner.localPosition.x - leftTopCorner.localPosition.x) / width;
			board = new GameObject[width, height];
			blockQueue = new Queue<Block>();
			blockQueue.Enqueue(randomBlock());
		}
		
		void FixedUpdate () {
			if (!recalc && !gameOver && !undetected && !Paused) {
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
			foreach (Transform tran in queuePositions) {
				foreach (Transform child in tran) {
					Destroy(child.gameObject);
				}
			}
			Block[] blocks = blockQueue.ToArray();
			for (int i = 0; i < queuePositions.Length; i++) {
				var inst = Instantiate(blocks[i], queuePositions[i]).transform;
				inst.localScale = Vector3.one;
				inst.localPosition = Vector3.zero;
				inst.localRotation = Quaternion.identity;
			}
		}

		void spawnNewBlock(Vector3? position, Block.Type? pref) {
			Block prefab = pref != null ? blocksPrefabs[(int)pref] : blockQueue.Dequeue();
			if (pref != null) {
				Vector3 stashOffset = prefab.startingPoint.localPosition - currentBlock.startingPoint.localPosition;
				currentBlock.transform.localPosition += stashOffset;
			}
			currentBlock = Instantiate(
				prefab,
				transform
			);
			currentBlock.transform.localPosition = position != null ? (Vector3)position : spawnPoint.localPosition;
			
			currentBlock.transform.localPosition += currentBlock.startingPoint.localPosition;

			currentBlock.type = prefab.type;
			fillQueue();
		}

		public void reloadGame() {
			SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
		}

		bool canMoveBlock(Vector2 transition) {
			if (!currentBlock) return false;
			foreach (GameObject block in currentBlock.chunks) {
				Vector2Int boardDesiredPos = calcBoardPosition(
					(Vector2)transform.InverseTransformPoint(
						block.transform.position
					) + transition
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

		Vector2Int calcBoardPosition(Vector2 point) {
			return new Vector2Int(
				Mathf.FloorToInt((point.x - leftTopCorner.localPosition.x) / stepWidth),
				Mathf.FloorToInt((point.y - rightBottomCorner.localPosition.y) / stepHeight)
			);
		}
		void moveBlock(Vector2 transition) {
			currentBlock.transform.localPosition += (Vector3)transition;
		}
		void sealBlock() {
			if (!currentBlock) return;
			foreach (GameObject block in currentBlock.chunks) {
				Vector2Int boardPosition = calcBoardPosition(
					transform.InverseTransformPoint(
						block.transform.position
					)
				);
				if (boardPosition.y >= height) {
					gameOver = true;
					reloadGame();
				}

				board[boardPosition.x, boardPosition.y] = block;
			}
			foreach (GameObject child in currentBlock.chunks) {
				child.transform.parent = transform;
			}
			Destroy(currentBlock.gameObject);
			PrintBoard();
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

		private bool canRotateBlock(bool counterclockwise) {
			if (!currentBlock) return false;
			foreach (GameObject block in currentBlock.chunks) {
				Vector2 desiredPosition = transform.InverseTransformPoint(
					Quaternion.Euler(0, 0, 90 * (counterclockwise ? 1 : -1))
					* (block.transform.position - currentBlock.pivot.position)
					+ currentBlock.pivot.position
				);
				Vector2Int boardDesiredPos = calcBoardPosition(desiredPosition);
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
					currentBlock.pivot.forward,
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
			// Vector3 color = Vector3.zero;
			// destroy line
			for (int el = 0; el < board.GetLength(0); el += 1) {
				Color col = board[el, line].GetComponent<MeshRenderer>().material.color;
				CubeRotate.Instance.AddToQueue(col);
				Destroy(board[el, line]);
				board[el, line] = null;
			}
			// color /= board.GetLength(0);
			// inst.GetComponent<MeshRenderer>().material.color = new Color(color.x, color.y, color.z);
			// move blocks down
			for (int row = line + 1; row < board.GetLength(1); row += 1) {
				for (int el = 0; el < board.GetLength(0); el += 1) {
					if (board[el, row]) {
						board[el, row].transform.localPosition += new Vector3(0, -stepHeight, 0);
						// board[el, row].transform.Translate(Vector2.down * stepHeight, Space.Self);
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
			Utils.LogBoard(board, "x", "o");
		}
	}
}
