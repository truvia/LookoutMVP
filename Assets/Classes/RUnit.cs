using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class RUnit : MonoBehaviour {

	//key parameters for the unit
	public UnitType unitType;
	public int strength;
	public Mark allegiance;
	public string coords;
	public int numMoves = 0;
	public int defensiveBonus;

	//Classes used by Runit
	private RGameController gameController;
	private UIController uiController;
	private RBoard board;

	//parameters used solely for game View (e.g. moving and selecting the piece)
	private float[] moveToCoords = new float[2];
	public bool startMoving = false;
	public float speed = 3f;

	//public parameters use for game view (changeable in scene view)
	public GameObject unitMount;
 

	void Start () {
		//used to initialise
		gameController = FindObjectOfType<RGameController> ();
		uiController = FindObjectOfType<UIController> ();
		board = FindObjectOfType<RBoard> ();
	}
	
	void Update(){
		//determines movement
		if (startMoving) {


			float step = speed * Time.deltaTime;
			Vector3 TargetPosition = new Vector3 (moveToCoords [0], 0.05f, moveToCoords [1]);
			transform.position = Vector3.MoveTowards (transform.position, TargetPosition, step);

			if (transform.position == TargetPosition) {
				startMoving = false;
				moveToCoords = null;
			}

		} 
	
	}

	void OnMouseDown(){
		if (allegiance == gameController.localPlayerController.myAllegiance) {
			SelectThisUnit ();
		}
	}

	void SelectThisUnit(){
		RUnit squareDictionaryRunit = gameController.game.squareDictionary [coords].unitOccupyingSquare;
		board.DeselectPiece (); //deslect the original piece, hide the movement squares etc.
		InstantiateUnitMount (); //create the spinning gold circle that shows what unit is selected

		//select the unit in gameController
		gameController.selectedUnit = squareDictionaryRunit;
		gameController.unitSelected = true;
		gameController.selectedGameObject = this.gameObject;

		//update the unit HUD and show it
		uiController.SetUnitHUDValues(squareDictionaryRunit);
		uiController.ShowHUD (uiController.UnitHUD);

		//If the unit has a move left, show possible squares, if not flash red on the unitHUD number of moves.
		if (numMoves > 0) { 
			int[] intCoords =	gameController.ConvertStringToArray (squareDictionaryRunit.coords, 2);
			board.ShowPossibleSquares (intCoords, squareDictionaryRunit);

		} else {
			StartCoroutine (uiController.MakeTextFlashRed (uiController.unitHUDMoves));
			Debug.Log ("You don't have enough moves left for this piece");
		}

	}

	void InstantiateUnitMount(){
		unitMount = GameObject.FindGameObjectWithTag ("unitMount");
		if (unitMount != null) {
			Destroy (unitMount);

		}

			GameObject newMount = Instantiate (board.unitMount, this.transform.position, Quaternion.identity, this.transform);
			newMount.tag = "unitMount";

	}
		

	public void MoveTowardsAPlace(int[] intCoords){

		moveToCoords[0] = intCoords[0] + 0.5f;
		moveToCoords[1] = intCoords[1] + 0.5f;
		startMoving = true;

	}

	public IEnumerator CoroutineMoveTowards (int[] intCoords){
		yield return new WaitForSeconds (3);
			moveToCoords[0] = intCoords[0] + 0.5f;
			moveToCoords[1] = intCoords[1] + 0.5f;
			startMoving = true;

		if (startMoving) {
			float step = speed * Time.deltaTime;
			Vector3 TargetPosition = new Vector3 (moveToCoords [0], 0.05f, moveToCoords [1]);
			transform.position = Vector3.MoveTowards (transform.position, TargetPosition, step);

			if (transform.position == TargetPosition) {
				startMoving = false;
				moveToCoords = null;

			}

		}

	}

	public void SplitUnit(){
		Debug.Log("Unit split requested");
		RUnit squareDictionaryRUnit = GetSquareDictionaryRUnitByCoords (coords);

		strength = strength / 2;
		squareDictionaryRUnit.strength = strength;

		//Get a random square that is available for this unit to move into , in order to instantiate the new piece into.
		int[] coordsToInstantiateSplitUnit = board.possibleMovementCoords [Random.Range (0, board.possibleMovementCoords.Count)]; // get a random avaialbe square that this piece could have moved into that we can place the piece in, 
		string stringCoordsToInstantiateSplitUnit = gameController.ConvertArrayToString (coordsToInstantiateSplitUnit); //convert these coords to string

		gameController.game.ConstructNewUnit (stringCoordsToInstantiateSplitUnit, allegiance, unitType, strength); //create a new unit in the SquareDictionary
		gameController.game.squareDictionary [stringCoordsToInstantiateSplitUnit].unitOccupyingSquare.numMoves = 0; // set the new units numMOves to 0; 
		board.PlaceUnit (gameController.game.squareDictionary [stringCoordsToInstantiateSplitUnit].unitOccupyingSquare); // and now instantiate this piece on the actual board
		board.RegenerateFogOfWar(); //regenerate fog of war (as the unit might have been placed on the border 

		//Take a move and sync this unit to the game.squareDictionary RUnit.
		squareDictionaryRUnit.numMoves -= 1; //take a move
		gameController.SyncSceneUnitToDictionaryUnit (squareDictionaryRUnit, this.gameObject); // and sync this unit's value to the dictionary above. 

		board.DeselectPiece (); //clear the selector squares, hide the UnitHUD and deselect the unit in gameController.

	}

	private RUnit GetSquareDictionaryRUnitByCoords(string coordsToSearchFor){
		if (gameController.game.squareDictionary [coordsToSearchFor].squareOccupied) {
			return gameController.game.squareDictionary [coordsToSearchFor].unitOccupyingSquare;
		} else {
			Debug.Log ("Error! The Game.SquareDictionary has no unit occupying the square at these coords : " + coordsToSearchFor);
			return null;
		}

	}

	public void AddDefensiveBonus(int bonusAmount){
		defensiveBonus += bonusAmount;
	}

	public void RemoveDefensiveBonus(){
		defensiveBonus = 0;
	}









}
