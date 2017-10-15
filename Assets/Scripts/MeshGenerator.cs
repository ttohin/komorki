using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

  // Use this for initialization
  void Start () {

    var mutableMesh = new MeshGeneration.MutableMesh ();
    var grid = new MeshGeneration.SquareGrid(10, 10, mutableMesh);
    grid.grid[0, 0].points = MeshGeneration.Square.CreateSquareFromPoint(MeshGeneration.SquarePoint.BottomLeft);
    grid.grid[0, 1].points = MeshGeneration.Square.CreateSquareFromPoint(MeshGeneration.SquarePoint.BottomLeft);
    grid.grid[1, 0].points = MeshGeneration.Square.CreateSquareFromPoint(MeshGeneration.SquarePoint.BottomLeft);
    grid.grid[1, 1].points = MeshGeneration.Square.CreateSquareFromPoint(MeshGeneration.SquarePoint.BottomLeft);
    grid.grid[2, 1].points = MeshGeneration.Square.CreateSquareFromPoint(MeshGeneration.SquarePoint.BottomLeft);
    grid.Build();

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    var mesh = new Mesh () {
      vertices = mutableMesh.GetVertexes ().ToArray (),
      triangles = mutableMesh.GetTriangles ().ToArray (),
    };
    meshFilter.sharedMesh = mesh;
  }

  // Update is called once per frame
  void Update () {

  }
}