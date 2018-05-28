using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RLookout;

public class CitySpawner: MonoBehaviour {

	public GameObject cityPrefab;

	private RGameController gameController;


	void Start(){
		gameController = FindObjectOfType<RGameController> ();
	}


	public void InstantiateCityPrefab(Vector3 location, City city){
		GameObject newGameObject;

		newGameObject = cityPrefab;

		Transform parent = this.transform;
		GameObject newCity = Instantiate (newGameObject, location, Quaternion.identity, parent);

		gameController.SyncCity (city, newCity);
	}

}
