using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGeneration {
  public class Mesh {
    public List<Vector3> vertices;
    public List<int> triangles;
    public Mesh (MutableMesh mutableMesh) {
      this.vertices = mutableMesh.GetVertexes ();
      this.triangles = mutableMesh.GetTriangles ();
    }
  }
}