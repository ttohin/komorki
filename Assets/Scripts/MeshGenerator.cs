﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

  // Use this for initialization
  void Start () {

    var mutableMesh = new MutableMesh ();
    var v1 = mutableMesh.CreateVertex (new Vector3 (0, 0, 0));
    var v2 = mutableMesh.CreateVertex (new Vector3 (0, 1, 0));
    var v3 = mutableMesh.CreateVertex (new Vector3 (1, 1, 0));
    var v4 = mutableMesh.CreateVertex (new Vector3 (1, 0, 0));
    var triangle1 = mutableMesh.CreateTriangle (v1, v2, v3);
    var triangle2 = mutableMesh.CreateTriangle (v1, v3, v4);

    List<Vector3> vertexes;
    List<int> triangles;
    mutableMesh.BuildMesh (out vertexes, out triangles);

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    var mesh = new Mesh () {
      vertices = vertexes.ToArray (),
      triangles = triangles.ToArray ()
    };
    meshFilter.sharedMesh = mesh;
  }

  // Update is called once per frame
  void Update () {

  }
}