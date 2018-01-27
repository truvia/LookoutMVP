using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour, IPointerClickHandler	 {

	public const string SquareClickedNotification = "Board.SquareClickedNotification2";

	public int width = 5;
	public int height = 5;


	[SerializeField] ArmySpawner USArmySpawner;
	[SerializeField] ArmySpawner CONArmySpawner;
		
	// Use this for initialization
	void Start () {
		
	
	}


	public void Show(int index, Mark mark){
		ArmySpawner armySpawner = mark == Mark.CON ? CONArmySpawner : USArmySpawner;

		int x = index % 3;
		int z = index / 3;

		Vector3 location = new Vector3 (x + 0.5f, 0, z+ 0.5f);
		armySpawner.InstantiatePrefab (location);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData){


		Vector3 pos = eventData.pointerCurrentRaycast.worldPosition;
		int x = Mathf.FloorToInt (pos.x);
		int z = Mathf.FloorToInt (pos.z);

		if (x < 0 || z < 0 || x > 2 || z > 2) {
			Debug.Log ("IpoitnerClickHander says out of range at x = " + x + " and z = " + z);
			return;
		}

		int index = z * 3 + x;
		EventManager.TriggerEvent (SquareClickedNotification, index); //need to transfer index somehow.

	}

}
