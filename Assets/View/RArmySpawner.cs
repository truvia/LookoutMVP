using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RLookout;

public class RArmySpawner : NetworkBehaviour {

	public GameObject ArmyPrefab;
	public GameObject FortressPrefab;
	public GameObject SpyPrefab;

	public int NumPrepopulate;
	public int MaxNum;

	private RGameController gameController;

	//	private Queue<GameObject> armyQueue = new Queue<GameObject>();
	//	private Army[] armies;
	void Start(){
		gameController = FindObjectOfType<RGameController> ();
	}

	public void InstantiatePrefab(Vector3 location, RUnit unit){
		GameObject newGameObject;

		if (unit.unitType == UnitType.Army) {
			newGameObject = ArmyPrefab;
		} else if (unit.unitType == UnitType.Fortress) {
			newGameObject = FortressPrefab;
		} else if (unit.unitType == UnitType.Spy) {
			newGameObject = SpyPrefab;
		} else {
			newGameObject = ArmyPrefab;
		}
			

		Transform parent = this.transform;

		GameObject newUnit = Instantiate (newGameObject, location, Quaternion.identity, parent);

		gameController.SyncSceneUnitToDictionaryUnit (unit, newUnit);

	}
		

}
