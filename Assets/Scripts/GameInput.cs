using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
  public class GameInput : MonoBehaviour
  {
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.W)) {
        BoardManager.Instance.SwapWithStash();
      } else if (Input.GetKeyDown(KeyCode.A)) {
        BoardManager.Instance.Move(Vector2.left);
      } else if (Input.GetKeyDown(KeyCode.D)) {
        BoardManager.Instance.Move(Vector2.right);
      } else if (Input.GetKeyDown(KeyCode.S)) {
        BoardManager.Instance.Drop();
      }
    }
  }
}
