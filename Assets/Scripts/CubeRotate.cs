using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
  public class CubeRotate : MonoBehaviour
  {
    public static CubeRotate Instance;

    public Queue<Color> blocksQueue = new Queue<Color>();
    List<GameObject> cubes = new List<GameObject>();

    public GameObject prefab;
    public Transform spawnPoint;
    public float minTime;
    private float timer = 0.0f;

    private float destroyTimer = 0.0f;

    void Awake() {
      Instance = this;
    }
    void FixedUpdate()
    {
      if (cubes.Count > 0) {
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= 15) {
          destroyTimer = 0;
          clearCubes();
        }
      }
      if (blocksQueue.Count > 0) {
        timer += Time.deltaTime;
        if (timer >= minTime) {
          timer = 0;
          spawnBlock(blocksQueue.Dequeue());
        }
      }
      Physics.gravity = -transform.up * 9.82f;
    }

    private void clearCubes() {
      foreach (GameObject cube in cubes) {
        Destroy(cube);
      }
      cubes.Clear();
    }

    private void spawnBlock(Color col) {
      var inst = Instantiate(prefab, spawnPoint).transform;
      inst.localPosition = Vector3.zero;
      inst.localScale = Vector3.one;
      inst.localRotation = Quaternion.identity;
      inst.GetComponent<MeshRenderer>().material.color = col;
      cubes.Add(inst.gameObject);
    }

    public void AddToQueue(Color color) {
      blocksQueue.Enqueue(color);
    }
  }
}
