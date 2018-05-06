using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class RBoard : MonoBehaviour, IPointerClickHandler {

	private RGameController gameController;
	public const string SquareClickedNotification = "RBoard.SquareClickedNotification";

	public RArmySpawner CONArmySpawner;
	public RArmySpawner USArmySpawner;
	public Transform selectionSquaresSpawner;
	public List<int[]> possibleMovementCoords = new List<int[]> ();
	public List<int[]> battleSquareCoords = new List<int[]> ();
	public List<int[]> mergeableSquareCoords = new List<int[]> ();

	public GameObject redSelectionSquarePrefab;
	public GameObject greenSelectionSquarePrefab;
	public GameObject blueSelectionSquarePrefab;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
	}

	public void Place(int[] coords, Mark allegiance, RUnit unit){
		
		RArmySpawner armySpawner = allegiance == Mark.CON ? CONArmySpawner : USArmySpawner;
		Vector3 location = new Vector3 (coords [0] + 0.5f, 0.05f, coords [1] + 0.5f);
		armySpawner.InstantiatePrefab (location, unit);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData){

		//Debug.Log ("clicked");
		Vector3 pos = eventData.pointerCurrentRaycast.worldPosition;
		int x = Mathf.FloorToInt (pos.x);
		int z = Mathf.FloorToInt (pos.z);

		if (x < 0 || z < 0 || x > gameController.game.boardWidth|| z > gameController.game.boardHeight) {
			return;
		}

		int[] coords = new int[] {
			x,
			z
		};

		EventManager.TriggerEvent (SquareClickedNotification, coords);
	}


	public void ShowPossibleSquares(int[] coords, RUnit unit){
		//Debug.Log ("Show Possible Squares called");
		ClearAllSelectorSquares ();
		GameObject selectionSquare;


		if (unit.unitType == UnitType.Army) {

			for(int x = -1; x <= 1; x++)	{
				for (int z = -1; z <= 1; z++) {
					Vector3 selectorCoord = new Vector3 ();

					selectorCoord.x = coords [0] + x + 0.5f;
					selectorCoord.z = coords [1] + z + 0.5f;

					int[] newCoords = new int[]{ coords[0] + x, coords[1] + z};

					string coordsAsString = gameController.ConvertArrayToString (newCoords);

					if (selectorCoord.x < 0.5f || selectorCoord.z < 0.5f || selectorCoord.x > gameController.game.boardWidth || selectorCoord.z > gameController.game.boardHeight) {
						// don't instantiate out of range ie. do nothing (return cancels the loop)

					} else if (gameController.game.squareDictionary [coordsAsString].squareOccupied) {
						//if there is a unit there
						if (gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.allegiance == unit.allegiance && gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.unitType == UnitType.Army) {
							//if it is a unit of my allegiance there
							if (unit.coords != coordsAsString) {
								//stops a blue square forming on original position;
								selectionSquare = blueSelectionSquarePrefab;

								mergeableSquareCoords.Add (newCoords);
								GameObject newUnit = Instantiate (selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
								newUnit.tag = "selectorSquare";
							} else {
								//if we want to instantiate a special square that says where it came from.
							}
						} else if (gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.allegiance == unit.allegiance && gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.unitType == UnitType.Fortress) {
							//Do nothing
						 
						}else{
							//if it is an enemy unit there
							selectionSquare = redSelectionSquarePrefab;
							battleSquareCoords.Add (newCoords);
							GameObject newUnit = Instantiate(selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
							newUnit.tag = "selectorSquare";
						}


					}
						else {
							//there is no unit there
						selectionSquare = greenSelectionSquarePrefab;
						GameObject newUnit = Instantiate (selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
						newUnit.tag = "selectorSquare";
						possibleMovementCoords.Add (newCoords);

						}
						
				}
			}
		}

	}

	public void ClearAllSelectorSquares(){
		if (selectionSquaresSpawner.childCount > 0) {


			Transform[] allSelectorSquare = selectionSquaresSpawner.GetComponentsInChildren<Transform> ();

			foreach (Transform selectorSquareTransform in allSelectorSquare) {
				if (selectorSquareTransform.gameObject.tag == "selectorSquare") {
					//.Log (selectorSquareTransform.name);
					Destroy (selectorSquareTransform.gameObject);

				}
			}

		}

		if (possibleMovementCoords.Count > 0) {
			possibleMovementCoords.Clear ();
		}

		if (battleSquareCoords.Count > 0) {
			battleSquareCoords.Clear ();
		}

		if (mergeableSquareCoords.Count > 0) {
			mergeableSquareCoords.Clear ();
		}
	}




}
