﻿using System;
using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using UnityEngine;

public class CellGeneratorController : MonoBehaviour {
  public string seed;
  System.Random pseudoRandom;
  public Vector2[] UV;
  MeshGeneration.MutableMesh mutableMesh;

  void Start () {

    seed = Time.time.ToString ();
    var map = new Buffer<bool> (4, 4);

    pseudoRandom = new System.Random (seed.GetHashCode ());

    mutableMesh = new MeshGeneration.MutableMesh ();
    var shapeAnalizer = GenerateMap (map);
    var grid = CreateMesh (shapeAnalizer, mutableMesh);
    grid.Build ();

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    var mesh = new Mesh () {
      vertices = mutableMesh.GetVertexes ().ToArray (),
      triangles = mutableMesh.GetTriangles ().ToArray (),
    };
    meshFilter.sharedMesh = mesh;
  }

  void Update () { }

  ShapeAnalizer GenerateMap (Buffer<bool> map) {
    map.Fill (false);
    map.Set (true, 0, 0);
    map.Set (true, 1, 1);
    map.Set (true, 1, 2);
    map.Set (true, 2, 2);
    map.Set (true, 1, 3);
    map.Set (true, 0, 2);
    return new ShapeAnalizer (map);
  }

  static MeshGeneration.SquareGrid CreateMesh (ShapeAnalizer shapeAnalizer, MeshGeneration.MutableMesh mutableMesh) {

    MeshGeneration.SquareGrid grid = new MeshGeneration.SquareGrid (
      shapeAnalizer.resultBuffer.width,
      shapeAnalizer.resultBuffer.height,
      mutableMesh
    );

    shapeAnalizer.resultBuffer.ForEach ((value, x, y) => {

      if (value.type == Part.Type.Fill)
        grid.grid[x, y].points = MeshGeneration.Square.CreateSquareFromPoint (MeshGeneration.SquarePoint.BottomLeft);
      if (value.type == Part.Type.Corner) {
        var point = SquarePointFromPosition (value.position);
        grid.grid[x, y].points = MeshGeneration.Square.CreateShorCorner (GetOposite (point));
      }
      if (value.type == Part.Type.Border) {
        var point = SquarePointFromPosition (value.position);
        grid.grid[x, y].points = MeshGeneration.Square.CreateBorder (GetOposite (point));
      }
      if (value.type == Part.Type.InnnerCornter) {
        var point = SquarePointFromPosition (value.position);
        grid.grid[x, y].points = MeshGeneration.Square.CreateShorCorner (point);
      }
      if (value.type == Part.Type.Bridge) {
        var point = SquarePointFromPosition (value.position);
        grid.grid[x, y].points = MeshGeneration.Square.CreateChannel (point);
      }
      if (
        value.type == Part.Type.BorderBottomToCorner ||
        value.type == Part.Type.BorderLeftToCorner ||
        value.type == Part.Type.BorderRightToCorner ||
        value.type == Part.Type.BorderTopToCorner
      ) {

        var point = SquarePointFromPosition (value.position);
        if (value.type == Part.Type.BorderRightToCorner) {
          if (value.position == Part.Position.Bottom)
            grid.grid[x, y].points = MeshGeneration.Square.CreatetLeftBorderToCorner(IncrementPoint(point, 3));
          else
            grid.grid[x, y].points = MeshGeneration.Square.CreateRightBorderToCorner(IncrementPoint(point, -3));
        }
        if (value.type == Part.Type.BorderLeftToCorner) {
          if (value.position == Part.Position.Bottom)
            grid.grid[x, y].points = MeshGeneration.Square.CreateRightBorderToCorner(IncrementPoint(point, -3));
          else
            grid.grid[x, y].points = MeshGeneration.Square.CreatetLeftBorderToCorner(IncrementPoint(point, 3));
        }
        if (value.type == Part.Type.BorderTopToCorner) {
          if (value.position == Part.Position.Right)
            grid.grid[x, y].points = MeshGeneration.Square.CreatetLeftBorderToCorner(IncrementPoint(point, 3));
          else
            grid.grid[x, y].points = MeshGeneration.Square.CreateRightBorderToCorner(IncrementPoint(point, -3));
        }
        if (value.type == Part.Type.BorderBottomToCorner) {
          if (value.position == Part.Position.Right)
            grid.grid[x, y].points = MeshGeneration.Square.CreateRightBorderToCorner(IncrementPoint(point, -3));
          else
            grid.grid[x, y].points = MeshGeneration.Square.CreatetLeftBorderToCorner(IncrementPoint(point, 3));
        }

      }
    });

    return grid;
  }

  public static MeshGeneration.SquarePoint BorderToCornerConvert (Part position) {
    var result = SquarePointFromPosition (position.position);

    if (position.type == Part.Type.BorderRightToCorner)
      return MeshGeneration.Square.IncrementPoint (result, -3);
    if (position.type == Part.Type.BorderLeftToCorner)
      return MeshGeneration.Square.IncrementPoint (result, 3);
    if (position.type == Part.Type.BorderBottomToCorner)
      return MeshGeneration.Square.IncrementPoint (result, 3);
    if (position.type == Part.Type.BorderTopToCorner)
      return MeshGeneration.Square.IncrementPoint (result, -3);

    throw new ArgumentException ("Unknown position " + position);
  }

  public static MeshGeneration.SquarePoint SquarePointFromPosition (Part.Position position) {
    switch (position) {
      case Part.Position.Bottom:
      case Part.Position.BottomCenter:
        return MeshGeneration.SquarePoint.BottomCenter;
      case Part.Position.Top:
      case Part.Position.TopCenter:
        return MeshGeneration.SquarePoint.TopCenter;
      case Part.Position.Left:
      case Part.Position.CenterLeft:
        return MeshGeneration.SquarePoint.LeftCenter;
      case Part.Position.Right:
      case Part.Position.CenterRight:
        return MeshGeneration.SquarePoint.RightCenter;
      case Part.Position.BottomLeft:
        return MeshGeneration.SquarePoint.BottomLeft;
      case Part.Position.BottomRight:
        return MeshGeneration.SquarePoint.BottomRight;
      case Part.Position.TopLeft:
        return MeshGeneration.SquarePoint.TopLeft;
      case Part.Position.TopRight:
        return MeshGeneration.SquarePoint.TopRight;
      default:
        throw new ArgumentException ("Unknown Part.Position " + position);
    }
  }

  public static MeshGeneration.SquarePoint GetOposite (MeshGeneration.SquarePoint point) {
    return MeshGeneration.Square.IncrementPoint (point, 4);
  }

  public static MeshGeneration.SquarePoint IncrementPoint (MeshGeneration.SquarePoint point, int increment) {
    return MeshGeneration.Square.IncrementPoint (point, increment);
  }
}