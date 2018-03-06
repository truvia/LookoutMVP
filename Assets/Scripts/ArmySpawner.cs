using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArmySpawner : NetworkBehaviour {

	public GameObject ArmyPrefab;
	public GameObject FortressPrefab;
	public GameObject SpyPrefab;

	public int NumPrepopulate;
	public int MaxNum;


	//	private Queue<GameObject> armyQueue = new Queue<GameObject>();
//	private Army[] armies;


	public void InstantiatePrefab(Vector3 location, Unit unit){
		GameObject newGameObject;

		if (unit.unit_type == Unit.UnitType.Army) {
			newGameObject = ArmyPrefab;
		} else if (unit.unit_type == Unit.UnitType.Fortress) {
			newGameObject = FortressPrefab;
		} else if (unit.unit_type == Unit.UnitType.Spy) {
			newGameObject = SpyPrefab;
		} else {
			newGameObject = ArmyPrefab;
		}

	Transform parent = this.transform;

		GameObject newUnit = Instantiate (newGameObject, location, Quaternion.identity, parent);

		newUnit.AddComponent<Unit> ();
		newUnit.GetComponent<Unit> ().allegiance = unit.allegiance;
		newUnit.GetComponent<Unit> ().unit_type = unit.unit_type;
		newUnit.GetComponent<Unit> ().strength = unit.strength;

		NetworkServer.Spawn (newUnit);
	}



//
//	private void PreparePrefab(GameObject prefab, Vector3 location){
//		Transform parent = this.transform;
//		Quaternion rotation = Quaternion.identity;
//
//
//		prefab.transform.position = location;
//		prefab.transform.rotation = rotation;
//		prefab.transform.parent = parent;
//
//		Enqueue (prefab);
//	}
//
//	private void InstantiateEnqueuedItem(GameObject enquedItem){
//		Instantiate (enquedItem);
//	}
//
//	public void Enqueue(GameObject objectToEnqueue){
//		
//		armyQueue.Enqueue (objectToEnqueue);
//	
//	}
//
//	public void DeQueue(){
//		
//	}
		

	///We could perhaps enque a set of actions, rather than objects e.g. Instanatiate prefab, destroy object blah blah. 
	/// This would allow us to have stored a set of everything that has been done,e so that when it changes player, payer 2 can see the actions that has been taken in the last turn (rather than seeing them live).
	/// Equally, there is some value presumably in having a list of all the things that are being displayed, and make sure it reflects the game state?
	/// Do we keep the prefabs on screen, or can we save the game state so that we could recreate instnatiated objects whenever we want (e.g. if there is a crash, the last section is saved).
	/// Perhaps we create a queue, 
	/// 

}
