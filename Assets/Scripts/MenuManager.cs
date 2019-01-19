using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetARis.Core {
  public class MenuManager : MonoBehaviour
  {
    public static MenuManager Instance;

    void OnEnable() {
      Instance = this;
    }

    void Start() {
      gameObject.SetActive(false);
    }
    public GameObject consoleCanvas;

    public Transform console;

    public void RestartGame() {
      BoardManager.Instance.reloadGame();
    }

    public void ShowConsole(bool value) {
      consoleCanvas.SetActive(value);
    }

    public void ResumeGame() {
      BoardManager.Instance.Paused = false;
    }
  }
}
