using System;
using MeshGeneration;
using UnityEngine;

public class CustomAnimation {
  private float animationDuration;
  private float duration = 0.0f;
  private Func<float, bool> onUpdate;
  private Action<bool> completion;
  private bool completed = false;
  public CustomAnimation (float animationDuration, Func<float, bool> onUpdate, Action<bool> completion) {
    this.animationDuration = animationDuration;
    this.onUpdate = onUpdate;
    this.completion = completion;
    completed = false;
  }

  public void Update (float deltaTime) {
    if (completed)
      return;

    duration += deltaTime;
    float progress = duration / animationDuration;
    if (!onUpdate (progress)) {
      completed = true;
      completion (true);
      return;
    }

    if (progress >= animationDuration) {
      completed = true;
      completion (false);
      return;
    }
  }
}