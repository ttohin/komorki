using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		var d = Input.GetAxis("Mouse ScrollWheel");
		var dofSettings = GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings;
		dofSettings.focusDistance += d * 0.5f;
		GetComponent<PostProcessingBehaviour> ().profile.depthOfField.settings = dofSettings;

	}
}
