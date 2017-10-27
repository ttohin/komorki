using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGeneration {
  public enum SquarePoint {
    BottomLeft = 0,
      LeftCenter = 1,
      TopLeft = 2,
      TopCenter = 3,
      TopRight = 4,
      RightCenter = 5,
      BottomRight = 6,
      BottomCenter = 7,
  }

  public class SquareNode {
    public MutableMeshVertex Vertex;
  }

  public class Square {
    private Vector3 pos;
    private SquarePoint startPoint;
    private Dictionary<SquarePoint, SquareNode> nodes;
    private MutableMesh mesh;
    public List<SquarePoint> points;

    public Square (MutableMesh mesh, Vector3 pos, List<SquarePoint> points) {
      this.mesh = mesh;
      this.pos = pos;
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

    public void Build () {
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

    public static List<SquarePoint> CreateSquareFromPoint (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 6),
      };
    }

    public static List<SquarePoint> CreateShorCorner (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 7),
      };
    }
    public static List<SquarePoint> CreateBorder (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 7),
          IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 6),
        IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 6),
      };
    }
    public static List<SquarePoint> CreateRightBorderToCorner (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 7),
        IncrementPoint (startPoint, 2),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 7),
      };
    }
    public static List<SquarePoint> CreatetLeftBorderToCorner (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 6),
        IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 6),
      };
    }

    public static List<SquarePoint> CreateChannel (SquarePoint startPoint) {
      return new List<SquarePoint> () {
        IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 1),
          IncrementPoint (startPoint, 3),
          IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 3),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 4),
          IncrementPoint (startPoint, 5),
          IncrementPoint (startPoint, 0),
          IncrementPoint (startPoint, 5),
          IncrementPoint (startPoint, 7),
      };
    }

    private MutableMeshVertex CreateVertex (SquarePoint point) {
      SquareNode node = GetNode(point);
      if (node == null) {
        throw new Exception ("Cannot get node for point " + point);
      }

      if (node.Vertex != null)
        return node.Vertex;

      var pos = getAbsolutePosition (point);
      node.Vertex = mesh.CreateVertex (pos);
      return node.Vertex;
    }

    public SquareNode GetNode(SquarePoint point)
    {
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
    private Vector3 getAbsolutePosition (SquarePoint point) {
      return pos + PositionFromPoint (point);
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
      }
      throw new ArgumentException ("Unknown point " + point);
    }
  }
}