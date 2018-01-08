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
  public GameObject eyePrefab;
  private float blinkSpeed;
  private float blinkDirection = 0;
  private float blinkValue = 1.0f;

  void Start () {

    var map = ShapeGeneration.CreateRandomShape ();

    mutableMesh = new MutableMesh ();

    SquareGrid grid = new SquareGrid (
      map.width * ShapeAnalizer.scale,
      map.height * ShapeAnalizer.scale,
      mutableMesh
    );

    blinkSpeed = Random.Range (1.0f, 2.5f);
    shapeAnalizer = GenerateMap (map, grid);

    var meshFilter = gameObject.GetComponent<MeshFilter> ();
    Vector3[] vertices = mutableMesh.GetVertexes ().ToArray ();
    float tileAmount = 0.5f;
    float textureWidth = ShapeAnalizer.scale * map.width;
    float textureHeight = ShapeAnalizer.scale * map.height;
    Vector2[] uvs = new Vector2[vertices.Length];
    for (int i = 0; i < vertices.Length; i++) {
      float percentX = Mathf.InverseLerp (0, textureWidth, vertices[i].x) * tileAmount;
      float percentY = Mathf.InverseLerp (0, textureHeight, vertices[i].y) * tileAmount;
      uvs[i] = new Vector2 (percentX, percentY);
    }
    var mesh = new Mesh () {
      vertices = vertices,
      triangles = mutableMesh.GetTriangles ().ToArray (),
      uv = uvs,
      uv2 = uvs,
      colors = mutableMesh.GetColors ().ToArray (),
    };
    mesh.RecalculateTangents ();
    mesh.RecalculateNormals ();
    mesh.RecalculateBounds ();
    meshFilter.sharedMesh = mesh;

    GetComponent<Renderer> ().material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);

    // CreatePhysics (map);
    transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

    var pupil = Instantiate (eyePrefab, shapeAnalizer.eye.center, Quaternion.identity, transform);
    pupil.transform.localScale = new Vector3 (2.0f, 2.0f, 1.0f);

    shapeAnalizer.eye.SetPupil (pupil, 0.12f);
    shapeAnalizer.eye.MovePupil (Vector3.zero);

    // StartCoroutine (MoveToRandomDirection ());
    StartCoroutine (BlinkPeriodically ());
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

  private IEnumerator BlinkPeriodically () {
    yield return new WaitForSeconds (Random.Range (0, 10));
    while (true) {
      blinkDirection = -1;
      yield return new WaitForSeconds (5);
    }
  }

  void Update () {
    if (blinkDirection != 0) {
      blinkValue += Time.deltaTime * blinkSpeed * blinkDirection;
      shapeAnalizer.eye.OpenEye (blinkValue);

      if (blinkValue <= 0.0f) {
        blinkDirection = 1;
      }

      if (blinkValue >= 1.0f && blinkDirection > 0) {
        blinkDirection = 0;
      }

      // update mesh to apply all changes made by MutableMesh
      var meshFilter = GetComponent<MeshFilter> ();
      meshFilter.mesh.vertices = mutableMesh.GetVertexes ().ToArray ();
    }

    // point pupil to cursor
    var mousePos = Input.mousePosition;
    mousePos.z = 10; // select distance = 10 units from the camera
    var mousePosition = Camera.main.ScreenToWorldPoint (mousePos);
    var pupilPos = transform.position + shapeAnalizer.eye.center;
    var pupilOffset = mousePosition - pupilPos;
    if (pupilOffset.sqrMagnitude > 1.0f)
      pupilOffset.Normalize ();

    pupilOffset.z = 0;
    shapeAnalizer.eye.MovePupil (pupilOffset);
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

  private bool IsInView () {
    var cam = Camera.current;
    if (cam == null) {
      return false;
    }
    Vector3 pointOnScreen = cam.WorldToScreenPoint (transform.position);

    //Is in front
    if (pointOnScreen.z < 0) {
      return false;
    }

    //Is in FOV
    if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
      (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height)) {
      return false;
    }

    return true;
  }
}