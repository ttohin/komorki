using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

  public GameObject target;
  // Use this for initialization
  void Start () { }

  void Update () {
    float xAxisValue = Input.GetAxis ("Horizontal");
    float yAxisValue = Input.GetAxis ("Vertical");
    transform.Translate (new Vector3 (xAxisValue, yAxisValue, 0.0f));

    if (Input.GetMouseButtonDown (0)) {
      var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
      focusOnFirstKomorka ();
    } else {
      var d = Input.GetAxis ("Mouse ScrollWheel");
      var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
      dofSettings.focusDistance += d;
      GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;
    }
  }

  void focusOnFirstKomorka () {
    var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
    RaycastHit[] hits = Physics.RaycastAll (ray);
    foreach (RaycastHit hit in hits) {
      target = hit.collider.gameObject;
      if (target.tag == "Komorka") {
        focusToPosition(target.transform.position);
        break;
      }
    }
  }

  void focusToPosition (Vector3 position) {
    Vector3 distance = position - transform.position;
    var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
    dofSettings.focusDistance = distance.z;
    GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;
  }
}