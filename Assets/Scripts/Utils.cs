using System.Collections.Generic;
using UnityEngine;
public static class Utils
{
  public static void Log(params object[] values) {
    List<object> objects = new List<object>(values);
    Debug.Log(string.Join(" ", objects.ConvertAll(o => o.ToString()).ToArray()));
  }
  public static void LogLines(params object[] values) {
    List<object> objects = new List<object>(values);
    Debug.Log(string.Join("\n", objects.ConvertAll(o => o.ToString()).ToArray()));
  }

  public static void LogBoard(bool[,] array, string trueChar, string falseChar) {
    List<string> lines = new List<string>();
    for (int i = array.GetLength(1) - 1; i >= 0; i--) {
      List<string> cells = new List<string>();
      for (int j = 0; j < array.GetLength(0); j++) {
        cells.Add(array[j, i] ? trueChar : falseChar);
      }
      lines.Add(string.Join(" ", cells.ToArray()));
    }
    Debug.Log(string.Join("\n", lines.ToArray()));
  }
}
