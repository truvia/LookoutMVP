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


	public void Show(int[] coords, Mark mark){
		ArmySpawner armySpawner = mark == Mark.CON ? CONArmySpawner : USArmySpawner;
//
//		int x = coords % 3;
//		int z = coords / 3;

		Vector3 location = new Vector3 (coords[0] + 0.5f, 0, coords[1] + 0.5f);
		armySpawner.InstantiatePrefab (location);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData){


		Vector3 pos = eventData.pointerCurrentRaycast.worldPosition;
		int x = Mathf.FloorToInt (pos.x);
		int z = Mathf.FloorToInt (pos.z);

		if (x < 0 || z < 0 || x > width || z > height) {
			return;
		}

		int[] coords = new int[]{x, z};
		EventManager.TriggerEvent (SquareClickedNotification, coords); //need to transfer index somehow.

	}

}