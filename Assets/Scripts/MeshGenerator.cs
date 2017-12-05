using System.Collections;
using System.Collections.Generic;
using MeshGeneration;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

  AnimatedVertex v1;
  MutableMesh mutableMesh;
  float direction = 1;
  // Use this for initialization
  void Start () {

    mutableMesh = new MutableMesh ();
    var grid = new SquareGrid (10, 10, mutableMesh);
    grid.grid[0, 0].Init (SquarePoint.BottomLeft, SquareBaseShape.Triangle);

    SquareNode node = grid.grid[0, 0].GetNode (SquarePoint.BottomLeft);
    v1 = new AnimatedVertex (node.Vertex, 0.5f, () => {
      direction *= -1;
      v1.SetTarget (v1.initialPosition - new Vector3 (direction * 0.2f, 0, 0));
    });
    v1.SetTarget (v1.initialPosition + new Vector3 (0.2f, 0, 0));

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    var mesh = new Mesh () {
      vertices = mutableMesh.GetVertexes ().ToArray (),
      triangles = mutableMesh.GetTriangles ().ToArray (),
    };
    meshFilter.sharedMesh = mesh;
  }

  // Update is called once per frame
  void Update () {
    v1.Update (Time.deltaTime);

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    meshFilter.mesh.vertices = mutableMesh.GetVertexes ().ToArray ();
  }
}