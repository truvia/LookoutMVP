using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySpawner : MonoBehaviour {

	public GameObject  ArmyPrefab;
	public int NumPrepopulate;
	public int MaxNum;



	public void InstantiatePrefab(Vector3 location){
		Transform parent = this.transform;

		Instantiate (ArmyPrefab, location, Quaternion.identity, parent);

	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
