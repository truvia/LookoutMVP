using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class Board : MonoBehaviour, IPointerClickHandler	 {

	public const string SquareClickedNotification = "Board.SquareClickedNotification2";

	public int width = 5;
	public int height = 5;


	[SerializeField] ArmySpawner USArmySpawner;
	[SerializeField] ArmySpawner CONArmySpawner;
		
	private Army[] armies;


	private UnityAction<System.Object> didBeginGameNotificationAction;

	void Awake(){

		//LISTENERS
		didBeginGameNotificationAction = new UnityAction<System.Object>(ClearAllArmies);

	}

	void OnEnable() {

		EventManager.StartListening (Lookout.Game.DidEndGameNotification, didBeginGameNotificationAction);
	}

	void Start(){
		armies = new Army[width * height];

	}



	public void Show(int[] coords, Mark mark){
		ArmySpawner armySpawner = mark == Mark.CON ? CONArmySpawner : USArmySpawner;

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
		EventManager.TriggerEvent (SquareClickedNotification, coords);

	}

	void ClearAllArmies(object args){
		Army[] armies = GameObject.FindObjectsOfType<Army> ();

		foreach (Army army in armies) {
			Debug.Log ("army found");
			Destroy (army.gameObject);
		}

		Debug.Log(" Clear all armies is called");

	}

}