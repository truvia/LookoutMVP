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
	public GameObject greenSelectorPrefab;
	public GameObject redSelectorPrefab;
	public GameObject selectorSpawner;
	public List<int[]> possibleMovementCoords = new List<int[]> ();

	[SerializeField] ArmySpawner USArmySpawner;
	[SerializeField] ArmySpawner CONArmySpawner;
		
	private Army[] armies;
	private GameController gameController;
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
		gameController = FindObjectOfType<GameController> ();


	}



	public void Show(int[] coords, Mark mark, Unit unit){
		ArmySpawner armySpawner = mark == Mark.CON ? CONArmySpawner : USArmySpawner;

		Vector3 location = new Vector3 (coords[0] + 0.5f, 0.05f, coords[1] + 0.5f);
		armySpawner.InstantiatePrefab (location, unit);
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
			Destroy (army.gameObject);
		}

		Debug.Log(" Clear all armies is called");

	}

	public void ShowPossibleSquares(int[] coords, Unit unit){
		ClearAllSelectorSquares ();

		if (unit.unit_type == Unit.UnitType.Army) {
			
			Transform parent = selectorSpawner.transform;
			GameObject selector;

			for(int x = -1; x <= 1; x++)	{
				for (int z = -1; z <= 1; z++) {

					Vector3 selectorCoord = new Vector3 ();

					selectorCoord.x = coords[0] + x + 0.5f;
					selectorCoord.z = coords[1] + z + 0.5f;

					int[] newCoords = new int[]{coords[0] + x, coords[1] + z };

					string coordsAsString = gameController.game.convertArrayToString (newCoords);

					if (selectorCoord.x < 0.5f || selectorCoord.z < 0.5f || selectorCoord.x > width || selectorCoord.z > height 
						|| gameController.game.unitDictionary [coordsAsString].allegiance == unit.allegiance) {
						//ie. do nothing (return cancels the loop)

					} else if (gameController.game.unitDictionary [coordsAsString].allegiance == Mark.None) {
						
						selector = greenSelectorPrefab;
						Instantiate (selector, selectorCoord, Quaternion.identity, parent); 
						possibleMovementCoords.Add (newCoords);
					
					} else {
						selector = redSelectorPrefab;
						Instantiate (selector, selectorCoord, Quaternion.identity, parent); 
						possibleMovementCoords.Add (newCoords);
					
					}
						



				
				
				}

			
			}
		
		}
			
	}


	public void MovePiece(int[] squareToMoveTo){
		int[] squareToMoveFrom = gameController.game.convertStringToArray(gameController.game.selectedCoords, 2);




	}

	public void ClearAllSelectorSquares(){
		
		foreach (Transform child in selectorSpawner.transform) {
			Destroy (child.gameObject);

		}

	}
}