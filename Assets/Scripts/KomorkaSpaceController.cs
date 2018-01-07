using System.Collections;
using System.Collections.Generic;
using Komorki;
using Komorki.Basic;
using Komorki.Common;
using Komorki.Generation;
using UnityEngine;
using UnityEngine.PostProcessing;

public class KomorkaSpaceController : MonoBehaviour {

  public GameObject KomorkaBody;
  private GreatPixelSpace Space;
  private Komorka komorka;
  private Vector3 currentPos = Vector3.zero;
  private Vector3 prevPos = Vector3.zero;
  private Vec3 direction = new Vec3 (0, 0, 0);
  int moveAnimationPhase = 0;
  float moveAnimationStart = 0;
  const float startInsideScaleAt = 0.2f;
  const float finishInsideScaleAt = 0.5f;
  const float startMovementAt = 0.4f;
  const float finishMovementAt = 0.6f;
  float moveSpeed = 0;
  public GameObject komorkaPrefab;
  public float CameraFrontPosition = -2;
  public float CameraBackPosition = -8;

  void Start () {
    var bodyMap = new Buffer<bool> (1, 1);
    bodyMap.Fill (true);
    KomorkaBody.GetComponent<CellGeneratorController> ().Init (bodyMap, true, false);
    Space = new GreatPixelSpace (30, 14, 5);
    komorka = new Komorka ();
    var komorkaPos = Space.Space[15, 7, 2];
    komorka.MoveToPosition (komorkaPos);
    currentPos = getKomorkaPos ();
    prevPos = currentPos;
    KomorkaBody.transform.position = currentPos;
    focusToPosition (currentPos);
    transform.position = new Vector3 (currentPos.x, currentPos.y, transform.position.z);
    GenerateKomorkas ();
  }

  void GenerateKomorkas () {
    for (int i = 0; i < Space.Width; i++) {
      for (int j = 0; j < Space.Height; j++) {
        for (int z = 0; z < Space.Depth; z++) {
          if (Random.value < 0.05f) {
            var komorka = Instantiate (komorkaPrefab, pixelToWorldPosition (new Vec3 (i, j, z)), Quaternion.identity);
            var map = ShapeGeneration.CreateRandomShape ();
            komorka.GetComponent<CellGeneratorController> ().Init (map, false, false);
          }
        }
      }
    }
  }

  private void MoveIfPossible () {

    if (moveAnimationPhase != 0)
      return;

    if (direction.x != 0 || direction.y != 0 || direction.z != 0) {
      GreatPixel nextPosiiton = null;
      if (direction.z != 0) {
        if (direction.z == 1) {
          nextPosiiton = komorka.Position.Front;
        } else {
          nextPosiiton = komorka.Position.Back;
        }
      }

      if (nextPosiiton == null) {
        nextPosiiton = komorka.Position.GetNeighbour (direction.x, direction.y);
      }

      if (nextPosiiton != null) {
        prevPos = KomorkaBody.transform.position;
        komorka.MoveToPosition (nextPosiiton);
        direction = new Vec3 (0, 0, 0);
        moveAnimationPhase = 1;
        moveAnimationStart = 0;
      }
    }
  }

  void updateDirection () {
    float xAxisValue = Input.GetAxis ("Horizontal");
    float yAxisValue = Input.GetAxis ("Vertical");
    float zAxieValue = Input.GetAxis ("Mouse ScrollWheel");

    if (xAxisValue != 0 || yAxisValue != 0 || Mathf.Abs (zAxieValue) > 0.5) {
      direction = new Vec3 (0, 0, 0);

      if (zAxieValue != 0) {
        int offset3d = zAxieValue > 0 ? 1 : -1;
        direction.z = offset3d;
      }

      if (direction.z == 0) {
        if (xAxisValue > 0) direction.x = 1;
        if (xAxisValue < 0) direction.x = -1;
        if (yAxisValue > 0) direction.y = 1;
        if (yAxisValue < 0) direction.y = -1;
      }
    }
  }

  // Update is called once per frame
  void Update () {
    updateDirection ();
    MoveIfPossible ();

    if (moveAnimationPhase != 0) {
      moveAnimationStart += Time.deltaTime;

      if (moveAnimationStart <= startInsideScaleAt) {
        float openKomorkaRatio = moveAnimationStart / startInsideScaleAt;
        KomorkaBody.GetComponent<CellGeneratorController> ().ScaleShape (openKomorkaRatio);
      } else if (moveAnimationStart <= finishInsideScaleAt) {
        float openKomorkaRatio = (moveAnimationStart - startInsideScaleAt) / (finishInsideScaleAt - startInsideScaleAt);
        KomorkaBody.GetComponent<CellGeneratorController> ().ScaleShape (1.0f - openKomorkaRatio);
      }
    }

    if (moveAnimationPhase != 0 && moveAnimationStart >= startMovementAt && moveAnimationStart < finishMovementAt) {
      Vector3 targetPosition = getKomorkaPos ();
      float distanceRatio = (moveAnimationStart - startMovementAt) / (finishMovementAt - startMovementAt);
      currentPos = Vector3.Lerp (prevPos, targetPosition, distanceRatio);
      Debug.Log (" " + distanceRatio);

      KomorkaBody.transform.position = currentPos;
    }

    if (moveAnimationPhase != 0 && moveAnimationStart > finishMovementAt) {
      moveAnimationPhase = 0;
      direction = new Vec3 (0, 0, 0);
    }

    updateCameraPosition();
  }

  void updateCameraPosition () {
    float maxDistance = pixelToWorldPosition(new Vec3(0, 0, Space.Depth)).z;
    float distanceRatio = currentPos.z / maxDistance;
    transform.position = new Vector3 (currentPos.x, currentPos.y, CameraBackPosition - distanceRatio * (CameraBackPosition - CameraFrontPosition));
    focusToPosition (currentPos);
  }

  private Vector3 getKomorkaPos () {
    if (komorka == null) {
      return Vector3.zero;
    }
    return pixelToWorldPosition (komorka.Position.Position);
  }

  private Vector3 pixelToWorldPosition (Vec3 pixelPosition) {
    float pixelSize = 1.0f;
    float zPixelSize = 1.5f;
    return new Vector3 (pixelSize * pixelPosition.x, pixelSize * pixelPosition.y, zPixelSize * pixelPosition.z);
  }

  void focusToPosition (Vector3 position) {
    Vector3 distance = position - transform.position;
    var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
    dofSettings.focusDistance = distance.z;
    GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;
  }

  void OnDrawGizmos () {
    Gizmos.DrawCube (getKomorkaPos () + new Vector3 (0.25f, 0.25f, 0.25f), new Vector3 (0.5f, 0.5f, 0.5f));
  }
}