using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KomorkaPhysicGenerator : MonoBehaviour {

	public int InitialNumberOfKomorkas = 100;
	public GameObject komorkaPrefab;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < InitialNumberOfKomorkas; i++)
		{
			Instantiate(komorkaPrefab, new Vector3(Random.Range(-40, 40), Random.Range(0, 14), Random.Range(-5, 5)), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
