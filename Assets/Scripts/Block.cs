using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {

	[ExecuteInEditMode]
	public class Block : MonoBehaviour {

		public GameObject[] chunks;
		public Transform pivot;
		public Transform startingPoint;

		public Type type;

		public enum Type
		{
				I, J, L, O, S, T, Z
		}

		void OnEnable() {
			List<GameObject> chunks = new List<GameObject>();
			foreach (Transform child in transform) {
				if (child.CompareTag("Chunk")) {
					chunks.Add(child.gameObject);
				}
			}
			this.chunks = chunks.ToArray();
		}
	}
}
