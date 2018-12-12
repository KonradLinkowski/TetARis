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
		public int queueLength;
		private float timer;
		public float fallTime;

		public int width;
		public int height;

		// Use this for initialization
		void Start () {
			currentBlock = randomBlock();
			blockQueue = new Queue<Block>();
			fillQueue();
		}
		
		// Update is called once per frame
		void Update () {
			timer += Time.deltaTime;
			if (timer >= fallTime) {
				spawnNewBlock();
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
			currentBlock = Instantiate(blockQueue.Dequeue());
			fillQueue();
		}

		void moveBlock() {

		}

		void drop() {

		}

		void swapWithStash() {

		}
	}
}
