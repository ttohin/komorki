using System;
using MeshGeneration;
using UnityEngine;

namespace Komorki.Common {
  public class Part {
    public SquareBaseShape shape = SquareBaseShape.Empty;
    public SquarePoint startPoint = SquarePoint.BottomLeft;
  }

  public class ShapeAnalizer {
    public static int scale = 3;
    public ShapeAnalizer (Buffer<bool> inputBuffer, SquareGrid grid) {

      if (grid.width != inputBuffer.width * scale || grid.height != inputBuffer.height * scale)
        throw new ArgumentException (string.Format ("grid should equal to inputBuffer * {}", scale));

      var internalBuffer = new Buffer<bool> (inputBuffer.width * scale, inputBuffer.height * scale);
      inputBuffer.Scale (scale, out internalBuffer);

      internalBuffer.ForEach ((value, x, y) => {

        Part part = new Part ();
        if (value == false) {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);

          if (nSquare == 2 && nDiamond == 2) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 3) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y, 0.6f);
          }
        } else {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);
          if (nSquare == 4) {
            AnalizeSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 3 && nDiamond == 2) {
            AnalizeSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 3 && nDiamond == 3) {
            AnalizeSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 2) {
            AnalizeSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 1) {
            AnalizeTriagle (internalBuffer, grid.grid[x, y], x, y);
          }
        }
      });
    }

    int CountNeighborsSquare (Buffer<bool> buffer, int x, int y) {
      bool value;
      int count = 0;
      if (buffer.Get (x + 1, y, out value) && value == true) count += 1;
      if (buffer.Get (x - 1, y, out value) && value == true) count += 1;
      if (buffer.Get (x, y + 1, out value) && value == true) count += 1;
      if (buffer.Get (x, y - 1, out value) && value == true) count += 1;
      return count;
    }

    int CountNeighborsDiamond (Buffer<bool> buffer, int x, int y) {
      bool value;
      int count = 0;
      if (buffer.Get (x + 1, y + 1, out value) && value == true) count += 1;
      if (buffer.Get (x - 1, y - 1, out value) && value == true) count += 1;
      if (buffer.Get (x - 1, y + 1, out value) && value == true) count += 1;
      if (buffer.Get (x + 1, y - 1, out value) && value == true) count += 1;
      return count;
    }

    void AnalizeSquare (Buffer<bool> buffer, Square square, int x, int y) {
      square.Init (SquarePoint.BottomLeft, SquareBaseShape.Square);
    }

    void AnalizeInternalSquare (Buffer<bool> buffer, Square square, int x, int y) {
      square.Init (SquarePoint.BottomLeft, SquareBaseShape.Square);
    }

    void AnalizeTriagle (Buffer<bool> buffer, Square square, int x, int y, float moveVertextToCorner = 0.6f) {
      SquarePoint startPoint = SquarePoint.Center;
      SquareBaseShape shape = SquareBaseShape.Triangle;

      bool value;
      if (buffer.Get (x, y - 1, out value) && value == true && buffer.Get (x + 1, y, out value) && value == true) {
        startPoint = SquarePoint.BottomRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }

      if (buffer.Get (x, y + 1, out value) && value == true && buffer.Get (x - 1, y, out value) && value == true) {
        startPoint = SquarePoint.TopLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.down * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }
      if (buffer.Get (x, y - 1, out value) && value == true && buffer.Get (x - 1, y, out value) && value == true) {
        startPoint = SquarePoint.BottomLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.up * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }
      if (buffer.Get (x, y + 1, out value) && value == true && buffer.Get (x + 1, y, out value) && value == true) {
        startPoint = SquarePoint.TopRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }

    }
    void AnalizeInternalTriagle (Buffer<bool> buffer, Square square, int x, int y, float moveVertextToCorner = 0.3f) {
      SquarePoint startPoint = SquarePoint.Center;
      SquareBaseShape shape = SquareBaseShape.Triangle;

      bool value;
      if (buffer.Get (x, y - 1, out value) && value == true && buffer.Get (x + 1, y, out value) && value == true) {
        startPoint = SquarePoint.BottomRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.down * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }

      if (buffer.Get (x, y + 1, out value) && value == true && buffer.Get (x - 1, y, out value) && value == true) {
        startPoint = SquarePoint.TopLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }
      if (buffer.Get (x, y - 1, out value) && value == true && buffer.Get (x - 1, y, out value) && value == true) {
        startPoint = SquarePoint.BottomLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }
      if (buffer.Get (x, y + 1, out value) && value == true && buffer.Get (x + 1, y, out value) && value == true) {
        startPoint = SquarePoint.TopRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.up * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos = pointPos1;
        square.GetNode (point2).Vertex.Pos = pointPos2;
      }

    }
  }
}