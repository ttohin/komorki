using System.Collections;
using System.Collections.Generic;
using MeshGeneration;
using UnityEngine;

public class EyeLidVertex {
  private MutableMeshVertex vertex;
  Vector3 closedPosition;
  Vector3 openDirection;
  float openDistance;

  public EyeLidVertex (MutableMeshVertex vertex, Vector3 openPosition, Vector3 closedPosition) {
    this.vertex = vertex;
    this.closedPosition = closedPosition;
    this.vertex.Color = Color.white;

    var openPositionOffset = openPosition - closedPosition;
    openDirection = openPositionOffset.normalized;
    openDistance = openPositionOffset.magnitude;
  }

  public void MoveEyeLid (float open) {
    vertex.Pos = closedPosition + (openDirection * openDistance * open);
  }
}

public class Eye {
  private EyeLidVertex topLeftEyeLid;
  private EyeLidVertex topRightEyeLid;
  private EyeLidVertex bottomLeftEyeLid;
  private EyeLidVertex bottomRightEyeLid;
  private MutableMeshVertex leftCorner;
  private MutableMeshVertex rightCorner;
  public readonly Vector3 center;
  private GameObject pupil;
  private float maxOffset;

  static EyeLidVertex CreateEyeLidVertex (MutableMeshVertex vertex, Vector3 center) {
    var openPosition = vertex.Pos;
    var closedPosition = new Vector3 (vertex.Pos.x, center.y, 0.0f);
    return new EyeLidVertex (vertex, openPosition, closedPosition);
  }

  public Eye (
    MutableMeshVertex topLeftEyeLid,
    MutableMeshVertex topRightEyeLid,
    MutableMeshVertex bottomLeftEyeLid,
    MutableMeshVertex bottomRightEyeLid,
    MutableMeshVertex leftCorner,
    MutableMeshVertex rightCorner
  ) {
    center = new Vector3 (
      leftCorner.Pos.x + (rightCorner.Pos.x - leftCorner.Pos.x) / 2.0f,
      bottomLeftEyeLid.Pos.y + (topLeftEyeLid.Pos.y - bottomLeftEyeLid.Pos.y) / 2.0f,
      0.0f
    );

    this.topLeftEyeLid = CreateEyeLidVertex (topLeftEyeLid, center);
    this.topRightEyeLid = CreateEyeLidVertex (topRightEyeLid, center);
    this.bottomLeftEyeLid = CreateEyeLidVertex (bottomLeftEyeLid, center);
    this.bottomRightEyeLid = CreateEyeLidVertex (bottomRightEyeLid, center);
    this.leftCorner = leftCorner;
    this.rightCorner = rightCorner;

    this.leftCorner.Color = Color.white;
    this.rightCorner.Color = Color.white;
  }

  public void OpenEye (float open) {
    topLeftEyeLid.MoveEyeLid (open);
    topRightEyeLid.MoveEyeLid (open);
    bottomLeftEyeLid.MoveEyeLid (open);
    bottomRightEyeLid.MoveEyeLid (open);
  }

  public void SetPupil (GameObject pupil, float maxOffset) {
    this.pupil = pupil;
    this.maxOffset = maxOffset;
  }

  public void MovePupil (Vector3 relativeOffset) {
    if (pupil == null)
      return;

    pupil.transform.localPosition = center + (relativeOffset * maxOffset) + Vector3.forward * 0.1f;
  }

}