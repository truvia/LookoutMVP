using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;
using UnityEngine.Networking;
using UnityEngine.Events;

public class RGameController : NetworkBehaviour {

	//Listeners etc
	private UnityAction<System.Object> didBeginGameNotificationAction;
	private UnityAction<System.Object> didChangeTurnNotificationAction;
	private UnityAction<System.Object> onBoardSquareClickedNotificationAction;
	private UnityAction<System.Object> didStartLocalPlayerNotificationAction;

	private RBoard board;
	private RSelector selector;
	private UIController uiController;


	private RUnit mergeUnit;
	private GameObject originalPiece;
	// Use this for initialization

	public RGame game = new RGame();
	public RPlayerController localPlayerController;

	void Awake(){
		didBeginGameNotificationAction = new UnityAction<System.Object> (RefreshBoard); //defines what action that this object should take when the event is triggered
		didChangeTurnNotificationAction = new UnityAction<System.Object>(RefreshBoard);
		onBoardSquareClickedNotificationAction = new UnityAction<System.Object>(OnBoardSquareClicked);
		didStartLocalPlayerNotificationAction = new UnityAction<System.Object> (IdentifyLocalPlayer);
		board = FindObjectOfType<RBoard> ();
		selector = FindObjectOfType<RSelector> ();
		uiController = FindObjectOfType<UIController> ();

	}

	void OnEnable(){
		EventManager.StartListening(RGame.DidBeginGameNotification, didBeginGameNotificationAction); 
		EventManager.StartListening (RGame.DidChangeTurnNotification, didChangeTurnNotificationAction);
		EventManager.StartListening (RBoard.SquareClickedNotification, onBoardSquareClickedNotificationAction);
		EventManager.StartListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);
	}

	void OnDisable(){
		EventManager.StopListening(RGame.DidBeginGameNotification, didBeginGameNotificationAction); 
		EventManager.StopListening (RGame.DidChangeTurnNotification, didChangeTurnNotificationAction);
		EventManager.StopListening (RBoard.SquareClickedNotification, onBoardSquareClickedNotificationAction);
		EventManager.StopListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);
	}

	public void RefreshBoard(object obj){
		DestroyAllUnits ();
		//Debug.Log ("GameController . Refresh Board Called");
	
		foreach(KeyValuePair<string, Square> keyValue in game.squareDictionary){
			string key = keyValue.Key;
			Square value = keyValue.Value;

			//Debug.Log ("square is " + value.squareOccupied.ToString ());

			if (value.squareOccupied != false) {
				//Debug.Log ("coords are : " + key + " and square unit is " + value.unitOccupyingSquare.allegiance);
				int[] coords = ConvertStringToArray (key, 2);

				//create unit and sync its values
				board.Place (value.unitOccupyingSquare);

			}

		}
	}


	public void DestroyAllUnits(){
		Debug.Log ("Destroy All Units called");
		RUnit[] units = FindObjectsOfType<RUnit> ();

		foreach (RUnit unit in units) {
			Destroy (unit.gameObject);
		}
	
	}


		
	void OnBoardSquareClicked(object args){
		//Debug.Log ("RGameController.OnboardSquareClicked");
		int[] intCoords = (int[]) args; 
		string stringCoords = ConvertArrayToString (intCoords);

		if (selector.selectedPiece == null) {
			//if no piece is currently selected

			if (game.squareDictionary [stringCoords].squareOccupied) {
				RUnit unit = game.squareDictionary[stringCoords].unitOccupyingSquare;
				if (unit.allegiance == localPlayerController.myAllegiance) {
					//there is a piece at this location in the square dictionary and it is my allegiance;
					if (selector.pieceAtThisCoord) {
						//to prevent a bounceback if there is no piece at this coord

						uiController.SetUnitHUDValues(unit);
						uiController.ShowHUD(uiController.UnitHUD);


						if (unit.numMoves > 0) {
							selector.SelectPiece (selector.pieceAtThisCoord);												
							board.ShowPossibleSquares (intCoords, unit);

						} else {
							StartCoroutine (uiController.MakeTextFlashRed (uiController.unitHUDMoves));
							Debug.Log ("You don't have enough moves left for this piece");
						}
					} else {
						Debug.Log (this + " says that there is no selector.pieceAtThiscoord");	
					
					}
				} 
			}

		} else {
			// A piece is currently selected
			RUnit squareDictionarySelectedPiece = game.squareDictionary[selector.selectedPiece.GetComponent<RUnit>().coords].unitOccupyingSquare;

			for (int i =0; i<board.possibleMovementCoords.Count; i++){
				int[] intArray = board.possibleMovementCoords [i];
				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					//Move piece to location
					Debug.Log("Move piece requested");
					game.MovePiece (squareDictionarySelectedPiece, stringCoords);
					selector.PlacePiece (stringCoords);
					uiController.HideHUD (uiController.UnitHUD);
					board.ClearAllSelectorSquares ();

				}
			}

			for(int i = 0; i < board.mergeableSquareCoords.Count; i++){

				int[] intArray = board.mergeableSquareCoords [i];
				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					//Debug.Log("merge piece requested");

					mergeUnit = game.squareDictionary [stringCoords].unitOccupyingSquare;
					//Debug.Log (mergeUnit.coords + " merge unit coords are this and is there a unit occupying square? " + game.squareDictionary[stringCoords].squareOccupied + " and stringcoords are " + stringCoords);
					originalPiece = selector.selectedPiece;

					PromptUser ();

				}
			}
			for(int i = 0; i <board.battleSquareCoords.Count; i++){
				int[] intArray = board.battleSquareCoords [i];

				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					RUnit defender = game.squareDictionary [stringCoords].unitOccupyingSquare;
					RUnit attacker = squareDictionarySelectedPiece;
					bool attackerWin = game.DoBattle (attacker, defender);
				
					if (attackerWin) {
						DestroyUnitByUnitDictionary (defender);
						DestroyUnitByUnitDictionary (attacker);

						board.Place (attacker);
						SyncSceneUnitToDictionaryUnit (attacker, FindUnitByUnitDictionary(attacker));

					} else {
						DestroyUnitByUnitDictionary (attacker);
						SyncSceneUnitToDictionaryUnit (defender, FindUnitByUnitDictionary(defender));
					}
					
					Debug.Log ("done battle"); 

				}
			}
			// a piece is currently selected

	

			}

		
		}

	    
	void IdentifyLocalPlayer(object obj){
		Debug.Log ("Identify Local Player called");
		RPlayerController playerController = (RPlayerController)obj;
		localPlayerController = playerController;
	}

	public int[] ConvertStringToArray(string s, int numberOfCoordinates){


		string[] newStringArray = s.Split (',');

		int[] coords = new int[2]; 

		int z = 0;

		foreach (string str in newStringArray) {

			int x = 0;
			if (int.TryParse (str, out x)) {

				coords [z] = x;
				z++;
			}


		}

		return coords;
	}


	public string ConvertArrayToString(int[] array){
		string newString = "";
		int x = 0;
		foreach (int i in array) {
			x++;	
			newString += i.ToString ();
			if (x < array.Length) {
				newString += " , ";
			}
		}
		return newString;

	}	


	public void DestroyUnitByUnitDictionary(RUnit unitToDestroy){
		RUnit[] allUnits = FindObjectsOfType<RUnit> ();

		foreach (RUnit thisUnit in allUnits) {
			if (thisUnit.coords == unitToDestroy.coords) {
				Destroy (thisUnit.gameObject);
			}
		}
	}

	public GameObject FindUnitByUnitDictionary(RUnit unitToFind){
		RUnit[] allUnits = FindObjectsOfType<RUnit> ();
		foreach (RUnit thisUnit in allUnits) {
			if (thisUnit.coords == unitToFind.coords) {
				return thisUnit.gameObject;

			}
		}
		return null;
	}

	public void PromptUser(){
		uiController.WaitForUser("Do you want to merge units of strength " + originalPiece.GetComponent<RUnit>().strength + " and  new unit " + mergeUnit.strength + "?", new UnityAction ( () => {
			uiController.MergeUnits();
		}), new UnityAction( () => {
			uiController.CancelInput();
		}));
	}

	public void MergeUnits(){
		//Something is wrong with the mergeunit as the coords are the original position not the updated position;
		//Debug.Log("game controller says merge units at " + originalPiece.GetComponent<RUnit>().coords + " , and the mergebble piece at " + mergeUnit.coords);	
		GameObject gameObjectOfMergeUnit = FindUnitByUnitDictionary (mergeUnit);
		gameObjectOfMergeUnit.GetComponent<RUnit> ().strength += originalPiece.GetComponent<RUnit> ().strength;
		game.MergePiece (originalPiece.GetComponent<RUnit> ().coords, mergeUnit.coords);

		uiController.HideHUD (uiController.UnitHUD);
		selector.KillSelectedPiece ();
		board.ClearAllSelectorSquares ();
		//Merge Units

		GameObject gameObject = new GameObject ();

	
	}


	public void SyncSceneUnitToDictionaryUnit(RUnit squareDictionaryRUnit, GameObject unitInScene){
		RUnit unitInSceneRUnit = unitInScene.GetComponent<RUnit> ();


		unitInSceneRUnit.allegiance = squareDictionaryRUnit.allegiance;
		unitInSceneRUnit.coords = squareDictionaryRUnit.coords;
		unitInSceneRUnit.numMoves = squareDictionaryRUnit.numMoves;
		unitInSceneRUnit.strength = squareDictionaryRUnit.strength;
		unitInSceneRUnit.unitType = squareDictionaryRUnit.unitType;

	}




}
