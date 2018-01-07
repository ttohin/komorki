using System.Collections;
using System.Collections.Generic;
using Komorki.Generation;
using UnityEngine;

public class KomorkaPhysicGenerator : MonoBehaviour {
  private int InitialNumberOfKomorkas = 100;
  public GameObject komorkaPrefab;

  // Use this for initialization
  void Start () {
    for (int i = 0; i < InitialNumberOfKomorkas; i++) {
      var komorka = Instantiate (komorkaPrefab, new Vector3 (Random.Range (-15, 15), Random.Range (0, 14), Random.Range (-2, 2)), Quaternion.identity);
      var map = ShapeGeneration.CreateRandomShape ();
      komorka.GetComponent<CellGeneratorController>().Init(map, false, true);
    }
  }

  // Update is called once per frame
  void Update () {
  }
}