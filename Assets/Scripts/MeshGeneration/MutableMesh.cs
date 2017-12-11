using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGeneration {
  public class MutableMeshVertex {
    public Vector3 Pos;
    public int Index;
  }

  public class MutableMeshTriangle {
    public MutableMeshVertex v1;
    public MutableMeshVertex v2;
    public MutableMeshVertex v3;
    MutableMesh mesh;

    public MutableMeshTriangle (MutableMesh mesh, MutableMeshVertex v1, MutableMeshVertex v2, MutableMeshVertex v3) {
      this.v1 = v1;
      this.v2 = v2;
      this.v3 = v3;
      this.mesh = mesh;
    }
  }

  public class MutableMesh {

    public List<MutableMeshVertex> Vertexes = new List<MutableMeshVertex> ();
    public List<MutableMeshTriangle> Triangles = new List<MutableMeshTriangle> ();

    public MutableMesh () { }

    public List<Vector3> GetVertexes () {
      var vertexes = new List<Vector3> ();
      foreach (var v in Vertexes) {
        vertexes.Add (v.Pos);
      }
      return vertexes;
    }

    public List<int> GetTriangles () {
      var triangles = new List<int> ();
      foreach (var t in Triangles) {
        triangles.Add (t.v1.Index);
        triangles.Add (t.v2.Index);
        triangles.Add (t.v3.Index);
      }
      return triangles;
    }

    public MutableMeshVertex CreateVertex (Vector3 pos) {
      var v = new MutableMeshVertex ();
      v.Index = Vertexes.Count;
      v.Pos = pos;
      Vertexes.Add (v);
      return v;
    }

    public MutableMeshTriangle CreateTriangle (List<MutableMeshVertex> vertices) {
      return CreateTriangle (vertices[0], vertices[1], vertices[2]);
    }

    public MutableMeshTriangle CreateTriangle (MutableMeshVertex v1, MutableMeshVertex v2, MutableMeshVertex v3) {
      var triangle = new MutableMeshTriangle (this, v1, v2, v3);
      Triangles.Add (triangle);
      return triangle;
    }

  }
}