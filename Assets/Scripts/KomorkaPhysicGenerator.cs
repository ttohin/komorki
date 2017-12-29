using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomorkaPhysicGenerator : MonoBehaviour {

  private int InitialNumberOfKomorkas = 1000;
  public GameObject komorkaPrefab;

  // Use this for initialization
  void Start () {
    for (int i = 0; i < InitialNumberOfKomorkas; i++) {
      Instantiate (komorkaPrefab, new Vector3 (Random.Range (-40, 40), Random.Range (0, 14), Random.Range (-5, 5)), Quaternion.identity);
    }
  }

  // Update is called once per frame
  void Update () {

    float xAxisValue = Input.GetAxis ("Horizontal");
    float yAxisValue = Input.GetAxis ("Vertical");
    if (Camera.current != null) {
      Camera.current.transform.Translate (new Vector3 (xAxisValue, yAxisValue, 0.0f));
    }
  }
}