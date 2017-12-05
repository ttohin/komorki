using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGeneration {

  public enum SquareBaseShape {
    Empty,
    Triangle,
    Square
  }
  public enum SquarePoint {
    BottomLeft = 0,
    LeftCenter = 1,
    TopLeft = 2,
    TopCenter = 3,
    TopRight = 4,
    RightCenter = 5,
    BottomRight = 6,
    BottomCenter = 7,
    Center = 8,
  }

  public class SquareNode {
    public MutableMeshVertex Vertex;
  }

  public class Square {
    public Vector3 Position;
    private SquarePoint startPoint;
    private SquareBaseShape shape;
    private Dictionary<SquarePoint, SquareNode> nodes;
    private MutableMesh mesh;
    private List<SquarePoint> points;

    public void Init (SquarePoint startPoint, SquareBaseShape shape) {
      if (startPoint == SquarePoint.Center)
        throw new ArgumentException ("start point can't be Center");

      if (shape == SquareBaseShape.Triangle)
        points = CreateTriangle (startPoint);
      else if (shape == SquareBaseShape.Square)
        points = CreateSquare (startPoint);
      else if (shape == SquareBaseShape.Empty)
        points = new List<SquarePoint> ();
      else
        throw new ArgumentException (string.Format ("unknown shape '{0}'", shape));

      Build ();
    }

    public static bool IsTop (SquarePoint point) {
      switch (point) {
        case SquarePoint.TopLeft:
        case SquarePoint.TopCenter:
        case SquarePoint.TopRight:
          return true;
        default:
          return false;
      }
    }

    public static bool IsBottom (SquarePoint point) {
      switch (point) {
        case SquarePoint.BottomLeft:
        case SquarePoint.BottomCenter:
        case SquarePoint.BottomRight:
          return true;
        default:
          return false;
      }
    }

    public Square (MutableMesh mesh, Vector3 pos, List<SquarePoint> points) {
      this.mesh = mesh;
      this.Position = pos;
      this.nodes = new Dictionary<SquarePoint, SquareNode> ();
      this.points = points;

      foreach (SquarePoint point in Enum.GetValues (typeof (SquarePoint))) {
        this.nodes[point] = new SquareNode ();
      }
    }

    public void JoinWithSquare (Square square, SquarePoint position) {
      if (position == SquarePoint.RightCenter) {
        this.nodes[SquarePoint.TopRight] = square.nodes[SquarePoint.TopLeft];
        this.nodes[SquarePoint.RightCenter] = square.nodes[SquarePoint.LeftCenter];
        this.nodes[SquarePoint.BottomRight] = square.nodes[SquarePoint.BottomLeft];
      } else if (position == SquarePoint.TopCenter) {
        this.nodes[SquarePoint.TopRight] = square.nodes[SquarePoint.BottomRight];
        this.nodes[SquarePoint.TopCenter] = square.nodes[SquarePoint.BottomCenter];
        this.nodes[SquarePoint.TopLeft] = square.nodes[SquarePoint.BottomLeft];
      } else {
        throw new ArgumentException ("Unimplemented");
      }
    }

    private void Build () {
      if (points == null)
        return;

      var verticesForTriangle = new List<MutableMeshVertex> ();
      foreach (var point in points) {
        var vertex = CreateVertex (point);
        verticesForTriangle.Add (vertex);
        if (verticesForTriangle.Count == 3) {
          mesh.CreateTriangle (verticesForTriangle);
          verticesForTriangle.Clear ();
        }
      }
    }

    public static List<SquarePoint> CreateTriangle (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 6),
      };
    }

    public static List<SquarePoint> CreateSquare (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 6),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 6),
      };
    }

    private MutableMeshVertex CreateVertex (SquarePoint point) {
      SquareNode node = GetNode (point);
      if (node == null) {
        throw new Exception ("Cannot get node for point " + point);
      }

      if (node.Vertex != null)
        return node.Vertex;

      var pos = GetAbsolutePosition (point);
      node.Vertex = mesh.CreateVertex (pos);
      return node.Vertex;
    }

    public SquareNode GetNode (SquarePoint point) {
      SquareNode node = null;
      if (!nodes.TryGetValue (point, out node)) {
        return null;
      }
      return node;
    }

    public static SquarePoint IncrementPoint (SquarePoint point, int increment) {
      int result = (int) point + increment;
      bool resultIsPositive = result >= 0;
      result = Math.Abs (result) % 8;
      if (!resultIsPositive)
        result = 8 - result;
      return (SquarePoint) (result);
    }

    /// <summary>
    /// Return position for a square point considering the square position
    /// </summary>
    public Vector3 GetAbsolutePosition (SquarePoint point) {
      return Position + PositionFromPoint (point);
    }

    public static Vector3 PositionFromCenter (SquarePoint point) {
      return new Vector3 (0.5f, 0.5f, 0) - PositionFromPoint (point);
    }

    public static Vector3 PositionFromPoint (SquarePoint point) {
      switch (point) {
        case SquarePoint.BottomLeft:
          return new Vector3 (0, 0, 0);
        case SquarePoint.BottomCenter:
          return new Vector3 (0.5f, 0, 0);
        case SquarePoint.BottomRight:
          return new Vector3 (1.0f, 0, 0);
        case SquarePoint.RightCenter:
          return new Vector3 (1.0f, 0.5f, 0);
        case SquarePoint.TopRight:
          return new Vector3 (1.0f, 1.0f, 0);
        case SquarePoint.TopCenter:
          return new Vector3 (0.5f, 1.0f, 0);
        case SquarePoint.TopLeft:
          return new Vector3 (0.0f, 1.0f, 0);
        case SquarePoint.LeftCenter:
          return new Vector3 (0.0f, 0.5f, 0);
        case SquarePoint.Center:
          return new Vector3 (0.5f, 0.5f, 0);
      }
      throw new ArgumentException ("Unknown point " + point);
    }
  }
}