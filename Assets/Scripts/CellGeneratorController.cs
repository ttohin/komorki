using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using Komorki.Generation;
using MeshGeneration;
using UnityEngine;

public class CellGeneratorController : MonoBehaviour {
  MutableMesh mutableMesh;
  ShapeAnalizer shapeAnalizer;
  [Range (0, 1)]
  private float moveBorder = 0.0f;
  private float moveBorderSpeed = 0.7f;

  void Start () {

    var map = ShapeGeneration.CreateRandomShape ();

    mutableMesh = new MutableMesh ();

    SquareGrid grid = new SquareGrid (
      map.width * ShapeAnalizer.scale,
      map.height * ShapeAnalizer.scale,
      mutableMesh
    );

    shapeAnalizer = GenerateMap (map, grid);

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

    GetComponent<Renderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);

    CreatePhysics (map);
    transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

    StartCoroutine (MoveToRandomDirection ());
  }

  private IEnumerator MoveToRandomDirection () {
    yield return new WaitForSeconds (Random.Range (0, 10));
    while (true) {
      var rigidBody = gameObject.GetComponent<Rigidbody> ();
      Vector3 direction = new Vector3 (Random.Range (-10.0f, 10.0f), Random.Range (-10.0f, 10.0f), Random.Range (-10.0f, 10.0f));
      direction.Normalize ();
      direction *= Random.Range (0.1f, 1.0f);
      rigidBody.velocity += direction;
      yield return new WaitForSeconds (Random.Range (1, 5));
    }
  }

  void Update () {

    moveBorder += Time.deltaTime * moveBorderSpeed;
    if (moveBorder > 1.0f) {
      moveBorder = 0.0f;
    }

    int totalVertexCount = shapeAnalizer.animatedBorderVertices.Count;
    int vertexIndex = 0;
    foreach (var animatedBoderVertex in shapeAnalizer.animatedBorderVertices) {
      var ratio = 0.5f * (1 + Mathf.Sin (8 * Mathf.PI * vertexIndex / totalVertexCount + (2 * Mathf.PI * moveBorder)));
      vertexIndex += 1;
      animatedBoderVertex.SetRatio (ratio);
    }

    var meshFilter = GetComponent<MeshFilter> ();
    meshFilter.mesh.vertices = mutableMesh.GetVertexes ().ToArray ();
  }

  ShapeAnalizer GenerateMap (Buffer<bool> map, SquareGrid grid) {
    return new ShapeAnalizer (map, grid);
  }

  private void CreatePhysics (Buffer<bool> map) {

    var rigidBody = gameObject.AddComponent<Rigidbody> ();
    rigidBody.useGravity = false;
    rigidBody.drag = 0.2f;
    rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

    map.ForEach ((bool value, int x, int y) => {
      if (value) {
        var boxCollider = gameObject.AddComponent<BoxCollider> ();
        float boxSize = ShapeAnalizer.scale;
        boxCollider.size = new Vector3 (boxSize, boxSize, 0.5f);
        boxCollider.center = new Vector3 (boxSize / 2.0f + x * ShapeAnalizer.scale, boxSize / 2.0f + y * ShapeAnalizer.scale, 0.0f);
      }
    });
  }
}