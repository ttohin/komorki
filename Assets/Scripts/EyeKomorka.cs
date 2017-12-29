using System.Collections;
using System.Collections.Generic;
using Komorki.Common;
using Komorki.Generation;
using MeshGeneration;
using UnityEngine;

public class EyeKomorka : MonoBehaviour {
	public GameObject eyePrefab;
	private MutableMesh mutableMesh;
	private ShapeAnalizer shapeAnalizer;
	[Range (-2, 2)]
	public float openEye = 0.0f;

	// Use this for initialization
	void Start () {
		var map = ShapeGeneration.CreateRandomShape ();

		mutableMesh = new MutableMesh ();

		SquareGrid grid = new SquareGrid (
			map.width * ShapeAnalizer.scale,
			map.height * ShapeAnalizer.scale,
			mutableMesh
		);

		shapeAnalizer = new ShapeAnalizer (map, grid);

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
		};

		mesh.RecalculateTangents ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		meshFilter.sharedMesh = mesh;

		var renderer = gameObject.GetComponent<MeshRenderer> ();
		renderer.material.color = Random.ColorHSV (0f, 1f, 1f, 1f, 0.5f, 1f);

    var pupil = Instantiate (eyePrefab, shapeAnalizer.eye.center, Quaternion.identity, transform);
		pupil.transform.localScale = new Vector3(2.0f, 2.0f, 0);

		shapeAnalizer.eye.SetPupil(pupil, 0.12f);
		shapeAnalizer.eye.MovePupil(Vector3.zero);
	}

	// Update is called once per frame
	void Update () {

		if (!IsInView ()) {
			return;
		}

		shapeAnalizer.eye.OpenEye (openEye);

		var mousePos = Input.mousePosition;
   	mousePos.z = 10; // select distance = 10 units from the camera
		var mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
		var pupilPos = transform.position + shapeAnalizer.eye.center;
		var pupilOffset = mousePosition - pupilPos;
		if (pupilOffset.sqrMagnitude > 1.0f)
			pupilOffset.Normalize();

		pupilOffset.z = 0;

		//Debug.Log("pupilOffset " + pupilOffset);
		shapeAnalizer.eye.MovePupil(pupilOffset);

		var meshFilter = GetComponent<MeshFilter> ();
		meshFilter.mesh.vertices = mutableMesh.GetVertexes ().ToArray ();
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