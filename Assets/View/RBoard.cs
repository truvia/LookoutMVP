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
	public GameObject unitMount;
	public GameObject fogOfWarPrefab;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
	}
		

	public void Place(RUnit unit){
		int[] coords = gameController.ConvertStringToArray (unit.coords, 2);
		RArmySpawner armySpawner = unit.allegiance == Mark.CON ? CONArmySpawner : USArmySpawner;
		Vector3 location = new Vector3 (coords [0] + 0.5f, 0.05f, coords [1] + 0.5f);
		armySpawner.InstantiatePrefab (location, unit);
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData){
		//N.B. if you change the size of the board you need to change the size of the box collider on the board gameobject, because this is reliant on that box collider.
		if (eventData.button == PointerEventData.InputButton.Right) {
			Vector3 pos = eventData.pointerCurrentRaycast.worldPosition;
			int x = Mathf.FloorToInt (pos.x);
			int z = Mathf.FloorToInt (pos.z);
			if (x < 0 || z < 0 || x > gameController.game.boardWidth || z > gameController.game.boardHeight) {
				return;
			}

			int[] coords = new int[] {
				x,
				z
			};

			EventManager.TriggerEvent (SquareClickedNotification, coords);
		}
	}


	public void ShowPossibleSquares(int[] intCoords, RUnit unit){
		//After clicking on one of your units, show the squares that you can move to, take and merge with.
		//Debug.Log ("Show Possible Squares called");

		ClearAllSelectorSquares (); // if any are already instantiated for any reason, clear this and the lists that contain the movement squares etc.
		GameObject selectionSquare;


		if (unit.unitType == UnitType.Army) { 

			for(int x = -1; x <= 1; x++)	{
				for (int z = -1; z <= 1; z++) {
					Vector3 selectorCoord = new Vector3 ();

					selectorCoord.x = intCoords [0] + x + 0.5f;
					selectorCoord.z = intCoords [1] + z + 0.5f;

					int[] positionCoords = new int[]{ intCoords[0] + x, intCoords[1] + z};

					string coordsAsString = gameController.ConvertArrayToString (positionCoords);

					if (selectorCoord.x < 0.5f || selectorCoord.z < 0.5f || selectorCoord.x > gameController.game.boardWidth || selectorCoord.z > gameController.game.boardHeight) {
						// don't instantiate out of range ie. do nothing (return cancels the loop)
					} else if (gameController.game.squareDictionary [coordsAsString].squareOccupied) {
						//if there is a unit there
						if (gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.allegiance == unit.allegiance && gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.unitType == UnitType.Army) {
							//if it is a unit of my allegiance there
							if (unit.coords != coordsAsString) {
								//stops a blue square forming on original position;
								selectionSquare = blueSelectionSquarePrefab;

								mergeableSquareCoords.Add (positionCoords);
								GameObject newUnit = Instantiate (selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
								newUnit.tag = "selectorSquare";
							} else {
								//if we want to instantiate a special square that says where it came from.
							}
						} else if (gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.allegiance == unit.allegiance && gameController.game.squareDictionary [coordsAsString].unitOccupyingSquare.unitType == UnitType.Fortress) {
							//If the unit in that space is a fortress of my allegiance then do nothing
						 
						}else{
							//if it is an enemy unit there
							selectionSquare = redSelectionSquarePrefab;
							battleSquareCoords.Add (positionCoords);
							GameObject newUnit = Instantiate(selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
							newUnit.tag = "selectorSquare";
						}


					}
						else {
							//there is no unit there and it is a moveable square
						selectionSquare = greenSelectionSquarePrefab;
						GameObject newUnit = Instantiate (selectionSquare, selectorCoord, Quaternion.identity, selectionSquaresSpawner);
						newUnit.tag = "selectorSquare";
						possibleMovementCoords.Add (positionCoords);

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

	public void MovePiece(GameObject pieceToMove, int[] intCoordsToMoveTo){
		pieceToMove.transform.position = new Vector3 (intCoordsToMoveTo [0] + 0.5f, 0.05f, intCoordsToMoveTo [1] + 0.5f);
	}

	public void DeselectPiece(){
		UIController uiController = FindObjectOfType<UIController> ();
		uiController.HideHUD (uiController.UnitHUD); //hide Unit info popup;

		//Formally deselect object in GameController
		gameController.selectedUnit = null;
		gameController.selectedGameObject = null;
		gameController.unitSelected = false;

		//Get rid of the red, blue and green squares that show when you click on a unit, and clear the Lists that sit behind that.
		ClearAllSelectorSquares ();

	}

	public void RegenerateFogOfWar(){
		DestroyFogOfWar ();
		foreach (KeyValuePair<string, Square> keyValue in gameController.game.squareDictionary) {
			Debug.Log ("Generated fog of war");
			string coords = keyValue.Key;
			Square square = keyValue.Value;

			int[] intCoords = gameController.ConvertStringToArray (coords, 2);

			if (!square.squareOccupied || square.unitOccupyingSquare.allegiance != gameController.localPlayerController.myAllegiance) {			

				GameObject fogOfWarInstantiation = Instantiate (fogOfWarPrefab);
				fogOfWarInstantiation.transform.position = new Vector3 (intCoords [0] + 0.5f, 0f, intCoords [1] + 0.5f);
				fogOfWarInstantiation.transform.rotation = Quaternion.identity;
				fogOfWarInstantiation.transform.parent = this.transform.parent;
			}


		}

		List<RUnit> allMyUnits = gameController.game.FindUnitsByAllegiance (gameController.localPlayerController.myAllegiance);
		List<string> coordsToClearFogOfWar = new List<string> ();

		foreach (RUnit unit in allMyUnits) {
			int[] intCoords = gameController.ConvertStringToArray (unit.coords, 2);


			for (int x = -1; x <= 1; x++) {
				for (int z = -1; z <= 1; z++) {


					int[] coordsToClear = new int[2];
					coordsToClear [0] = (intCoords [0] + x);
					coordsToClear [1] = (intCoords [1] + z);

					if (coordsToClear [0] < 0f || coordsToClear [1] < 0f || coordsToClear [0] > gameController.game.boardWidth || coordsToClear [1] > gameController.game.boardHeight) {
					
					} else {


						string stringCoords = coordsToClear [0] + " , " + coordsToClear [1];
						coordsToClearFogOfWar.Add (stringCoords);
//					Debug.Log ("intCoords " + (intCoords [0] + x) + " , " + (intCoords [1] + z));
//					Debug.Log ("Coords to clear: " + coordsToClear [0] + " , " + coordsToClear [1]);
//					Debug.Log ("string coords are : " + stringCoords);


					}
				}
			}
		}

		FogOfWar[] fogsOfWarSquares = FindObjectsOfType<FogOfWar> ();

		foreach (FogOfWar fogOfWar in fogsOfWarSquares) {



			int[] fogOfWarCoords = new int[2];
			fogOfWarCoords [0] = Mathf.RoundToInt (fogOfWar.transform.position.x - 0.5f);
			fogOfWarCoords [1] = Mathf.RoundToInt (fogOfWar.transform.position.z - 0.5f);
			string fogOfWarStringCoords = fogOfWarCoords [0] + " , " + fogOfWarCoords [1];
			//Debug.Log ("coords to destroy = " + fogOfWarCoords [0] + " , " + fogOfWarCoords [1]);

			if (coordsToClearFogOfWar.Contains (fogOfWarStringCoords)) {
				//	Debug.Log ("coords to destroy are: " + fogOfWarStringCoords);
				Destroy (fogOfWar.gameObject);
			}

		}
	}
		public void DestroyFogOfWar(){
			
			FogOfWar[] allFogOfWar = FindObjectsOfType<FogOfWar>();
			foreach (FogOfWar fog in allFogOfWar) {
				Destroy (fog.gameObject);
			}

		}



}




