using System;
using MeshGeneration;
using UnityEngine;

namespace Komorki.Common {

  public class ShapeAnalizer {
    public static int scale = 3;
    public ShapeAnalizer (Buffer<bool> inputBuffer, SquareGrid grid) {

      if (grid.width != inputBuffer.width * scale || grid.height != inputBuffer.height * scale)
        throw new ArgumentException (string.Format ("grid should equal to inputBuffer * {0}", scale));

      var internalBuffer = new Buffer<int> (inputBuffer.width * scale, inputBuffer.height * scale);
      inputBuffer.ForEach ((value, i, j) => {
        for (int iScale = 0; iScale < scale; ++iScale) {
          for (int jScale = 0; jScale < scale; ++jScale) {
            internalBuffer.Set (value ? 1 : 0, i * scale + iScale, j * scale + jScale);
          }
        }
      });
      internalBuffer.ForEach ((value, x, y) => {

        if (value == 0) {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);

          if (nSquare == 2 && nDiamond == 2) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 3) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y, moveVertextToCorner: 0.6f);
          }
        } else {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);
          if (nSquare == 4) {
            AnalizeInternalSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 3 && nDiamond == 2) {
            AnalizeSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 3 && nDiamond == 3) {
            AnalizeInternalSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 2) {
            AnalizeInternalSquare (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 1) {
            AnalizeTriagle (internalBuffer, grid.grid[x, y], x, y);
          }
        }
      });
    }

    int CountNeighborsSquare (Buffer<int> buffer, int x, int y) {
      int value;
      int count = 0;
      if (buffer.Get (x + 1, y, out value) && value == 1) count += 1;
      if (buffer.Get (x - 1, y, out value) && value == 1) count += 1;
      if (buffer.Get (x, y + 1, out value) && value == 1) count += 1;
      if (buffer.Get (x, y - 1, out value) && value == 1) count += 1;
      return count;
    }

    int CountNeighborsDiamond (Buffer<int> buffer, int x, int y) {
      int value;
      int count = 0;
      if (buffer.Get (x + 1, y + 1, out value) && value == 1) count += 1;
      if (buffer.Get (x - 1, y - 1, out value) && value == 1) count += 1;
      if (buffer.Get (x - 1, y + 1, out value) && value == 1) count += 1;
      if (buffer.Get (x + 1, y - 1, out value) && value == 1) count += 1;
      return count;
    }

    void AnalizeSquare (Buffer<int> buffer, Square square, int x, int y) {
      square.Init (SquarePoint.BottomLeft, SquareBaseShape.Square);

      float moveVertexToBorder = 0.1f;
      int value;
      if (!buffer.Get (x - 1, y, out value) || value == 0) {
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopLeft;
        var pointOffset1 = Vector3.left * moveVertexToBorder;
        var pointOffset2 = Vector3.left * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      } else if (!buffer.Get (x + 1, y, out value) || value == 0) {
        var point1 = SquarePoint.BottomRight;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertexToBorder;
        var pointOffset2 = Vector3.right * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      } else if (!buffer.Get (x, y - 1, out value) || value == 0) {
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertexToBorder;
        var pointOffset2 = Vector3.down * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      } else if (!buffer.Get (x, y + 1, out value) || value == 0) {
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertexToBorder;
        var pointOffset2 = Vector3.up * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      }
    }

    void AnalizeInternalSquare (Buffer<int> buffer, Square square, int x, int y) {
      square.Init (SquarePoint.BottomLeft, SquareBaseShape.Square);
    }

    void AnalizeTriagle (Buffer<int> buffer, Square square, int x, int y, float moveVertextToCorner = 0.6f) {
      SquarePoint startPoint = SquarePoint.Center;
      SquareBaseShape shape = SquareBaseShape.Triangle;

      int value;
      if (buffer.Get (x, y - 1, out value) && value == 1 && buffer.Get (x + 1, y, out value) && value == 1) {
        startPoint = SquarePoint.BottomRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      }

      if (buffer.Get (x, y + 1, out value) && value == 1 && buffer.Get (x - 1, y, out value) && value == 1) {
        startPoint = SquarePoint.TopLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.down * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      }

      if (buffer.Get (x, y - 1, out value) && value == 1 && buffer.Get (x - 1, y, out value) && value == 1) {
        startPoint = SquarePoint.BottomLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.up * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      }

      if (buffer.Get (x, y + 1, out value) && value == 1 && buffer.Get (x + 1, y, out value) && value == 1) {
        startPoint = SquarePoint.TopRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      }
    }

    void AnalizeInternalTriagle (Buffer<int> buffer, Square square, int x, int y, float moveVertextToCorner = 0.3f) {
      SquarePoint startPoint = SquarePoint.Center;
      SquareBaseShape shape = SquareBaseShape.Triangle;

      int value;
      if (buffer.Get (x, y - 1, out value) && value == 1 && buffer.Get (x + 1, y, out value) && value == 1) {
        startPoint = SquarePoint.BottomRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.down * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      }

      if (buffer.Get (x, y + 1, out value) && value == 1 && buffer.Get (x - 1, y, out value) && value == 1) {
        startPoint = SquarePoint.TopLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      }

      if (buffer.Get (x, y - 1, out value) && value == 1 && buffer.Get (x - 1, y, out value) && value == 1) {
        startPoint = SquarePoint.BottomLeft;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertextToCorner;
        var pointOffset2 = Vector3.left * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
      }

      if (buffer.Get (x, y + 1, out value) && value == 1 && buffer.Get (x + 1, y, out value) && value == 1) {
        startPoint = SquarePoint.TopRight;
        square.Init (startPoint, shape);
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.right * moveVertextToCorner;
        var pointOffset2 = Vector3.up * moveVertextToCorner;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
      }
    }
  }
}