using System;
using System.Collections;
using System.Collections.Generic;
using MeshGeneration;
using UnityEngine;

public class AnimatedBorderVertex {

	private Vector3 posInside;
	private Vector3 posOutside;
	private MutableMeshVertex vertex;
	private float ratio;
	public AnimatedBorderVertex (Vector3 posInside, Vector3 posOutside, MutableMeshVertex vertex) {
		if (vertex == null)
			throw new ArgumentException ("vertex cannot be null");

		if (posInside != vertex.Pos)
			throw new ArgumentException (string.Format ("Vertex.Pos {0} should be equal to posInside {1}", vertex.Pos, posInside));

		this.posInside = posInside;
		this.posOutside = posOutside;
		this.vertex = vertex;
		ratio = 0.0f;
	}

	public void SetRatio (float newRatio) {
		Vector3 resultPos = Vector3.Lerp (posInside, posOutside, newRatio);
		ratio = newRatio;
		vertex.Pos = resultPos;
	}
}