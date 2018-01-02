using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

	public GameObject target;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll (ray);
			foreach (RaycastHit hit in hits) {
				target = hit.collider.gameObject;
				if (target.tag == "Komorka") {
					Vector3 distance = target.transform.position - transform.position;
					var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
					dofSettings.focusDistance = distance.z;
					GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;
					break;
				}
			}
		} else {
			var d = Input.GetAxis ("Mouse ScrollWheel");
			var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
			dofSettings.focusDistance += d;
			GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;
		}
	}
}