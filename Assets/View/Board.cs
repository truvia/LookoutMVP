using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour, IPointerClickHandler	 {

	public const string SquareClickedNotification = "Board.SquareClickedNotification";

		

	NotificationCentre2 notificationCentre = new NotificationCentre2();

	[SerializeField] ArmySpawner USArmySpawner;
	[SerializeField] ArmySpawner CONArmySpawner;
		
	// Use this for initialization
	void Start () {
		notificationCentre = GameObject.FindObjectOfType<NotificationCentre2> ();

	
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
			return;
		}

		int index = z * 3 + x;
		notificationCentre.PostNotification (SquareClickedNotification, index);
	
	}
}
