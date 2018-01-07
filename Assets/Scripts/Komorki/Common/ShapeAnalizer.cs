using System;
using System.Collections;
using System.Collections.Generic;
using MeshGeneration;
using UnityEngine;

namespace Komorki.Common {

  public class BorderVertex {
    public SquareNode node;
    public Vector3 directionOutside;
  }

  public class ShapeAnalizer {
    private const int Empty = 0;
    private const int DefaultValue = 1;
    private const int EyeCenter = 2;
    private const int EyeLeftCorner = 3;
    private const int EyeRightCorner = 4;
    public static int scale = 3;
    public List<AnimatedBorderVertex> animatedBorderVertices = new List<AnimatedBorderVertex> ();
    public List<BorderVertex> borderVertices = new List<BorderVertex> ();
    public Eye eye;
    public bool eyeFound = false;
    public int eyeX;
    public int eyeY;
    public ShapeAnalizer (Buffer<bool> inputBuffer, SquareGrid grid) {

      if (grid.width != inputBuffer.width * scale || grid.height != inputBuffer.height * scale)
        throw new ArgumentException (string.Format ("grid should equal to inputBuffer * {0}", scale));

      var internalBuffer = new Buffer<int> (inputBuffer.width * scale, inputBuffer.height * scale);
      inputBuffer.ForEach ((value, i, j) => {
        for (int iScale = 0; iScale < scale; ++iScale) {
          for (int jScale = 0; jScale < scale; ++jScale) {
            internalBuffer.Set (value ? DefaultValue : Empty, i * scale + iScale, j * scale + jScale);
          }
        }
      });

      FindEye (internalBuffer);

      internalBuffer.ForEach ((value, x, y) => {
        if (value == Empty) {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);

          if (nSquare == 2 && nDiamond == 2) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y);
          } else if (nSquare == 2 && nDiamond == 3) {
            AnalizeInternalTriagle (internalBuffer, grid.grid[x, y], x, y, moveVertextToCorner: 0.6f);
          }
        } else if (value == EyeCenter) {
          // Eye center should be empty
        } else if (value == EyeLeftCorner || value == EyeRightCorner) {
          AnalizeEyeCorner(value, x, y, grid.grid[x, y]);
        } else if (value == DefaultValue) {
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

      if (eyeFound)
      {
        var centralEyeSquare = grid.grid[eyeX, eyeY];
        var leftEyeSquare = grid.grid[eyeX - 1, eyeY];
        var rightEyeSquare = grid.grid[eyeX + 1, eyeY];

        eye = new Eye(
          topLeftEyeLid: centralEyeSquare.GetNode(SquarePoint.TopLeft).Vertex,
          topRightEyeLid: centralEyeSquare.GetNode(SquarePoint.TopRight).Vertex,
          bottomLeftEyeLid: centralEyeSquare.GetNode(SquarePoint.BottomLeft).Vertex,
          bottomRightEyeLid: centralEyeSquare.GetNode(SquarePoint.BottomRight).Vertex,
          leftCorner: leftEyeSquare.GetNode(SquarePoint.Center).Vertex,
          rightCorner: rightEyeSquare.GetNode(SquarePoint.Center).Vertex
        );


        borderVertices.Add (new BorderVertex {
          node = leftEyeSquare.GetNode (SquarePoint.TopLeft),
            directionOutside = Vector3.left,
        });
        borderVertices.Add (new BorderVertex {
          node = leftEyeSquare.GetNode (SquarePoint.BottomLeft),
            directionOutside = Vector3.left,
        });
        borderVertices.Add (new BorderVertex {
          node = rightEyeSquare.GetNode (SquarePoint.TopRight),
            directionOutside = Vector3.right,
        });
        borderVertices.Add (new BorderVertex {
          node = rightEyeSquare.GetNode (SquarePoint.BottomRight),
            directionOutside = Vector3.right,
        });
      }

      foreach (var borderVertex in borderVertices) {
        float outsideMaxDistance = 0.4f;
        var vertex = borderVertex.node.Vertex;
        animatedBorderVertices.Add (new AnimatedBorderVertex (vertex.Pos, vertex.Pos + borderVertex.directionOutside * outsideMaxDistance, vertex));
      }
      
    }

    void FindEye (Buffer<int> internalBuffer) {
      internalBuffer.ForEach ((value, x, y) => {

        if (eyeFound)
          return;

        if (value == DefaultValue) {
          int nSquare = CountNeighborsSquare (internalBuffer, x, y);
          int nDiamond = CountNeighborsDiamond (internalBuffer, x, y);
          if (nSquare == 4 && nDiamond == 4) {
            eyeFound = true;
            eyeX = x;
            eyeY = y;
            internalBuffer.Set (EyeCenter, x, y);
            internalBuffer.Set (EyeLeftCorner, x - 1, y);
            internalBuffer.Set (EyeRightCorner, x + 1, y);
          }
        }
      });
    }

    int CountNeighborsSquare (Buffer<int> buffer, int x, int y) {
      int value;
      int count = 0;
      if (buffer.Get (x + 1, y, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x - 1, y, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x, y + 1, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x, y - 1, out value) && value >= DefaultValue) count += 1;
      return count;
    }

    int CountNeighborsDiamond (Buffer<int> buffer, int x, int y) {
      int value;
      int count = 0;
      if (buffer.Get (x + 1, y + 1, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x - 1, y - 1, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x - 1, y + 1, out value) && value >= DefaultValue) count += 1;
      if (buffer.Get (x + 1, y - 1, out value) && value >= DefaultValue) count += 1;
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
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point1),
            directionOutside = pointOffset1.normalized,
        });
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point2),
            directionOutside = pointOffset2.normalized,
        });
      } else if (!buffer.Get (x + 1, y, out value) || value == 0) {
        var point1 = SquarePoint.BottomRight;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.right * moveVertexToBorder;
        var pointOffset2 = Vector3.right * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.x = pointPos1.x;
        square.GetNode (point2).Vertex.Pos.x = pointPos2.x;
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point1),
            directionOutside = pointOffset1.normalized,
        });
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point2),
            directionOutside = pointOffset2.normalized,
        });
      } else if (!buffer.Get (x, y - 1, out value) || value == 0) {
        var point1 = SquarePoint.BottomLeft;
        var point2 = SquarePoint.BottomRight;
        var pointOffset1 = Vector3.down * moveVertexToBorder;
        var pointOffset2 = Vector3.down * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point1),
            directionOutside = pointOffset1.normalized,
        });
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point2),
            directionOutside = pointOffset2.normalized,
        });
      } else if (!buffer.Get (x, y + 1, out value) || value == 0) {
        var point1 = SquarePoint.TopLeft;
        var point2 = SquarePoint.TopRight;
        var pointOffset1 = Vector3.up * moveVertexToBorder;
        var pointOffset2 = Vector3.up * moveVertexToBorder;
        var pointPos1 = square.GetAbsolutePosition (point1) + pointOffset1;
        var pointPos2 = square.GetAbsolutePosition (point2) + pointOffset2;
        square.GetNode (point1).Vertex.Pos.y = pointPos1.y;
        square.GetNode (point2).Vertex.Pos.y = pointPos2.y;
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point1),
            directionOutside = pointOffset1.normalized,
        });
        borderVertices.Add (new BorderVertex {
          node = square.GetNode (point2),
            directionOutside = pointOffset2.normalized,
        });
      }
    }

    MutableMeshVertex AnalizeEyeCorner (int value, int x, int y, Square square) {
      if (value == EyeLeftCorner) {
        square.Init (SquarePoint.BottomLeft, SquareBaseShape.Pie);
      } else if (value == EyeRightCorner) {
        square.Init (SquarePoint.TopRight, SquareBaseShape.Pie);
      } else {
        throw new ArgumentException(string.Format("Unexpected value for eye corner: {0}, [{1}, {2}]", value, x, y));
      }

      return square.GetNode (SquarePoint.Center).Vertex;
    }

    void AnalizeInternalSquare (Buffer<int> buffer, Square square, int x, int y) {
      square.Init (SquarePoint.BottomLeft, SquareBaseShape.Square);
    }

    void AnalizeTriagle (Buffer<int> buffer, Square square, int x, int y, float moveVertextToCorner = 0.6f) {
      SquarePoint startPoint = SquarePoint.Center;
      SquareBaseShape shape = SquareBaseShape.Triangle;

      int value;
      if (buffer.Get (x, y - 1, out value) && value >= DefaultValue && buffer.Get (x + 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y + 1, out value) && value >= DefaultValue && buffer.Get (x - 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y - 1, out value) && value >= DefaultValue && buffer.Get (x - 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y + 1, out value) && value >= DefaultValue && buffer.Get (x + 1, y, out value) && value >= DefaultValue) {
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
      if (buffer.Get (x, y - 1, out value) && value >= DefaultValue && buffer.Get (x + 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y + 1, out value) && value >= DefaultValue && buffer.Get (x - 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y - 1, out value) && value >= DefaultValue && buffer.Get (x - 1, y, out value) && value >= DefaultValue) {
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

      if (buffer.Get (x, y + 1, out value) && value >= DefaultValue && buffer.Get (x + 1, y, out value) && value >= DefaultValue) {
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