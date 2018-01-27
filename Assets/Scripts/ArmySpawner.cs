using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmySpawner : MonoBehaviour {

	public GameObject  ArmyPrefab;
	public int NumPrepopulate;
	public int MaxNum;


	private Queue<GameObject> armyQueue = new Queue<GameObject>();
	private Army[] armies;

	void Start(){
		Army[] armies = GameObject.FindObjectsOfType<Army> ();
	}

	public void InstantiatePrefab(Vector3 location){
		Transform parent = this.transform;

		Instantiate (ArmyPrefab, location, Quaternion.identity, parent);

	}

	private void PreparePrefab(GameObject prefab, Vector3 location){
		Transform parent = this.transform;
		Quaternion rotation = Quaternion.identity;


		prefab.transform.position = location;
		prefab.transform.rotation = rotation;
		prefab.transform.parent = parent;
	

	}

	private void InstantiateEnqueuedItem(GameObject enquedItem){
		Instantiate (enquedItem);
	}

	public void Enqueue(GameObject objectToEnqueue){
		

	}

	public void DeQueue(){
		
	}
		

}
