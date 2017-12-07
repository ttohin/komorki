using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using MeshGeneration;
using UnityEngine;

public class CellGeneratorController : MonoBehaviour {
  public string seed;
  System.Random pseudoRandom;
  public Vector2[] UV;
  MutableMesh mutableMesh;
  ShapeAnalizer shapeAnalizer;
  [Range (0, 1)]
  public float moveBorder = 0.0f;
  public float moveBorderSpeed = 0.7f;

  void Start () {

    seed = Time.time.ToString ();
    var map = new Buffer<bool> (4, 4);

    mutableMesh = new MutableMesh ();

    SquareGrid grid = new SquareGrid (
      map.width * ShapeAnalizer.scale,
      map.height * ShapeAnalizer.scale,
      mutableMesh
    );

    shapeAnalizer = GenerateMap (map, grid);
    CreateGizmos (map);

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    Vector3[] vertices = mutableMesh.GetVertexes ().ToArray ();
    float tileAmount = 0.5f;
    float textureSize = ShapeAnalizer.scale * map.width;
    Vector2[] uvs = new Vector2[vertices.Length];
    for (int i = 0; i < vertices.Length; i++) {
      float percentX = Mathf.InverseLerp (0, textureSize, vertices[i].x) * tileAmount;
      float percentY = Mathf.InverseLerp (0, textureSize, vertices[i].y) * tileAmount;
      uvs[i] = new Vector2 (percentX, percentY);
    }
    var mesh = new Mesh () {
      vertices = vertices,
      triangles = mutableMesh.GetTriangles ().ToArray (),
      uv = uvs,
      uv2 = uvs,
    };
    mesh.RecalculateTangents ();
    mesh.RecalculateNormals ();
    mesh.RecalculateBounds ();
    meshFilter.sharedMesh = mesh;

    GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
  }

  void Update () {

    moveBorder += Time.deltaTime * moveBorderSpeed;
    if (moveBorder > 1.0f) {
      moveBorder = 1.0f;
      moveBorderSpeed *= -1;
    } else if (moveBorder < 0.0f) {
      moveBorder = 0.0f;
      moveBorderSpeed *= -1;
    }

    foreach (var animatedBoderVertex in shapeAnalizer.animatedBorderVertices) {
      animatedBoderVertex.SetRatio (moveBorder);
    }

    var meshFilter = GetComponent<MeshFilter> ();
    meshFilter.mesh.vertices = mutableMesh.GetVertexes ().ToArray ();
  }

  ShapeAnalizer GenerateMap (Buffer<bool> map, SquareGrid grid) {
    map.Fill (false);
    map.Set (true, 0, 0);
    map.Set (true, 1, 1);
    map.Set (true, 1, 2);
    map.Set (true, 2, 2);
    map.Set (true, 1, 3);
    map.Set (true, 0, 2);

    return new ShapeAnalizer (map, grid);
  }

  private void CreateGizmos (Buffer<bool> map) {
    map.ForEach ((bool value, int x, int y) => {
      if (value) {
        var boxCollider = gameObject.AddComponent<BoxCollider2D> ();
        float boxSize = ShapeAnalizer.scale;
        boxCollider.size = new Vector2 (boxSize, boxSize);
        boxCollider.offset = new Vector2 (boxSize / 2.0f + x * ShapeAnalizer.scale, boxSize / 2.0f + y * ShapeAnalizer.scale);
      }
    });
  }
}