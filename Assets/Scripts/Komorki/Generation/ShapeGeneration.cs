using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using UnityEngine;

namespace Komorki.Generation {
  public static class ShapeGeneration {
    public static Buffer<bool> CreateRandomShape () {
      int width = 1;
      int height = 1;
      var result = new Buffer<bool> (100, 100);
      int horizontalTurnLength = Random.Range (1, 5);
      for (int i = 0; i < horizontalTurnLength; i++) {
        result.Set (true, i, 0);
      }
      width = horizontalTurnLength;

      if (Random.value < 0.8f) {
        int verticalTurnLengh = Random.Range (1, 4);
        for (int i = 0; i < verticalTurnLengh; i++) {
          result.Set (true, horizontalTurnLength - 1, 1 + i);
        }

        height = verticalTurnLengh + 1;

        if (Random.value < 0.5f) {
          int secondHorizontalTurnLengh = Random.Range (1, 3);

          int direction = Random.value < 0.5f ? -1 : 1;

          for (int i = 0; i < secondHorizontalTurnLengh; i++) {
            result.Set (true, horizontalTurnLength - 1 + direction + (direction * i), verticalTurnLengh);
          }

          if (direction > 0)
            width += secondHorizontalTurnLengh;
        }
      }

      return new Buffer<bool> (result, width, height);
    }

    public static void HorizontalMirror (Buffer<bool> buffer, int y) {
      for (int j = 0; j < y; j++) {
        for (int i = 0; i < buffer.width; i++) {
          buffer.Set (buffer.Get (i, j, false), i, 2 * y - j);
        }
      }
    }
  }

}