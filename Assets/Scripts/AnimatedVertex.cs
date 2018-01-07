using System;
using MeshGeneration;
using UnityEngine;

public class AnimatedVertex {
  public Vector3 initialPosition;
  private MutableMeshVertex vertex;
  private float speed;
  private Vector3 targetPosition;
  private Action completion;
  private bool completed = false;

  public AnimatedVertex (MutableMeshVertex vertex, float speed, Action completion) {
    if (vertex == null)
      throw new ArgumentNullException ("vertex cannot be null");
    this.vertex = vertex;
    this.speed = speed;
    initialPosition = vertex.Pos;
    this.completion = completion;
  }

  public void SetTarget (Vector3 targetPosition) {
    this.targetPosition = targetPosition;
    completed = false;
  }

  public void Update (float deltaTime) {
    if (completed)
      return;

    var offset = targetPosition - vertex.Pos;
    if (offset.sqrMagnitude < 0.001f) {
      completed = true;
      completion ();
      return;
    }

    offset.Normalize ();
    offset = offset * speed * deltaTime;
    vertex.Pos += offset;
  }
}