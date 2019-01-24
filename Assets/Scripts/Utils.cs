using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TetARis.Core {
  public static class Utils
  {
    private static GameObject textPrefab;

    public static void Log(params object[] values) {
      List<object> objects = new List<object>(values);
      string value = string.Join(" ", objects.ConvertAll(o => o.ToString()).ToArray());
      Debug.Log(value);
    }
    public static void LogLines(params object[] values) {
      List<object> objects = new List<object>(values);
      string value = string.Join("\n", objects.ConvertAll(o => o.ToString()).ToArray());
      Debug.Log(value);
    }

    public static void LogBoard(Object[,] array, string trueChar, string falseChar) {
      List<string> lines = new List<string>();
      for (int i = array.GetLength(1) - 1; i >= 0; i--) {
        List<string> cells = new List<string>();
        for (int j = 0; j < array.GetLength(0); j++) {
          cells.Add(array[j, i] ? trueChar : falseChar);
        }
        lines.Add(string.Join(" ", cells.ToArray()));
      }
      string value = string.Join("\n", lines.ToArray());
      Debug.Log(value);
    }

    public static void HandleLog(string logString, string stackTrace, LogType type) {
      Color? color = null;
      switch(type) {
        case LogType.Error:
        case LogType.Exception:
        color = Color.red;
        break;
        case LogType.Warning:
        color = Color.yellow;
        break;
        default:
        color = Color.black;
        break;
      }
      ConsoleLog(logString + "\n" + stackTrace, color);
    }

    private static void ConsoleLog(string text, Color? color = null) {
      if (text.StartsWith("StopVuforia")) return;
      if (!textPrefab) {
        textPrefab = Resources.Load("Prefabs/Text") as GameObject;
      }
      Text textObject = GameObject.Instantiate(textPrefab, MenuManager.Instance.console).GetComponent<Text>();
      textObject.text = text;
      textObject.color = (Color)(color != null ? color : Color.black);
    }
  }
}
