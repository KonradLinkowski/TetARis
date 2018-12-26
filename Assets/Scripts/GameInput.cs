using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
  public class GameInput : MonoBehaviour
  {
    private bool clicked = false;
    private Vector2 lastMousePosition;

    [Range(0, 90)]
    [SerializeField]
    private float angleRange;

    [SerializeField]
    private float minSwipeLength;
    
    void Update()
    {
      // keyboard input
      if (Input.GetKeyDown(KeyCode.W)) {
        BoardManager.Instance.Rotate(false);
      } else if (Input.GetKeyDown(KeyCode.A)) {
        BoardManager.Instance.Move(Vector2.left);
      } else if (Input.GetKeyDown(KeyCode.D)) {
        BoardManager.Instance.Move(Vector2.right);
      } else if (Input.GetKeyDown(KeyCode.S)) {
        BoardManager.Instance.Move(Vector2.down);
      } else if (Input.GetKeyDown(KeyCode.Space)) {
        BoardManager.Instance.Drop();
      } else if (Input.GetKeyDown(KeyCode.R)) {
        BoardManager.Instance.SwapWithStash();
      } else if (Input.GetKeyDown(KeyCode.P)) {
        BoardManager.Instance.PrintBoard();
      }

      if (Input.GetMouseButtonDown(0)) {
        clicked = true;
        lastMousePosition = Input.mousePosition;
      } else if (Input.GetMouseButtonUp(0)) {
        clicked = false;
        Vector2 difference = (Vector2)Input.mousePosition - lastMousePosition;
        handleSwipe(difference);
      }
    }
    
    private void handleSwipe(Vector2 vector) {
      // Single click
      if (vector.magnitude < minSwipeLength) {
        BoardManager.Instance.Rotate(false);
        return;
      }
      // Swipes
      float angle = Vector2.SignedAngle(Vector2.up, vector);
      // Left
      if (angle > 90 - angleRange && angle < 90 + angleRange) {
        BoardManager.Instance.Move(Vector2.left);
      }
      // Right
      else if (angle > -90 - angleRange && angle < -90 + angleRange) {
        BoardManager.Instance.Move(Vector2.right);
      }
      // Up
      else if (angle <= 90 - angleRange && angle >= -90 + angleRange) {
        BoardManager.Instance.SwapWithStash();
      }
      // Down
      else if (angle >= 90 + angleRange || angle <= -90 - angleRange) {
        BoardManager.Instance.Drop();
      }
    }
  }
}
