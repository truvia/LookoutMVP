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
	//private RSelector selector;
	private UIController uiController;
	private EnqueueSystem enqueueSystem;

	public RUnit selectedUnit;
	public GameObject selectedGameObject;
	public bool unitSelected = false;
	private RUnit mergeUnit;

	public int boardwidth; 
	public int boardheight; 
	//private GameObject originalPiece;
	// Use this for initialization

	public RGame game = new RGame();
	public RPlayerController localPlayerController;

	void Awake(){
		didBeginGameNotificationAction = new UnityAction<System.Object> (RefreshBoard); //defines what action that this object should take when the event is triggered
		didChangeTurnNotificationAction = new UnityAction<System.Object>(ChangeTurnAction);
		onBoardSquareClickedNotificationAction = new UnityAction<System.Object>(OnBoardSquareClicked);
		didStartLocalPlayerNotificationAction = new UnityAction<System.Object> (IdentifyLocalPlayer);
		board = FindObjectOfType<RBoard> ();
		//selector = FindObjectOfType<RSelector> ();
		uiController = FindObjectOfType<UIController> ();
		enqueueSystem = FindObjectOfType<EnqueueSystem> ();

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
		//enqueueSystem.CallDequeuePieces ();
		board.ClearAllSelectorSquares();	
		DestroyAllUnits ();
		Debug.Log ("GameController . Refresh Board Called");
	
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

		board.RegenerateFogOfWar ();
	}

	public void ChangeTurnAction(object obj){
		if (localPlayerController.myAllegiance == game.control) {
			enqueueSystem.CallDequeuePieces ();

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
		Debug.Log ("onboardsquareclicked");
		if (!enqueueSystem.preventInput) {


		//Debug.Log ("RGameController.OnboardSquareClicked");
		int[] intCoords = (int[]) args; 
		string stringCoords = ConvertArrayToString (intCoords);
			//Debug.Log ("boardsquareclicked and enque system hasn't prevented input");
			if (unitSelected) {
			//	Debug.Log ("unit selected");
			// A piece is currently selected

				if (game.control == localPlayerController.myAllegiance) { //if it is my turn;
			//	Debug.Log (" a piece is selected");
					for (int i = 0; i < board.possibleMovementCoords.Count; i++) {  //if there is a moveable Square;

					int[] intArray = board.possibleMovementCoords [i];
					if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
						//Move piece to location
					//Debug.Log ("Move piece requested" + selectedUnit + " blah " + " " + selectedUnit.allegiance);


						GameObject gameObjectRunitOfSelectedPiece = FindUnitByUnitDictionary (selectedUnit);
						enqueueSystem.enquedPieceMovement.Add (selectedUnit.coords, intCoords); //enqueues movement for next player so they can see what moves the enemy took;
						selectedGameObject.GetComponent<RUnit> ().MoveTowardsAPlace (intCoords);

						//board.MovePiece (selectedGameObject, intCoords); //physically move the unit
						game.MovePiece (selectedUnit, stringCoords); //move the unit in the unitDictionary - may be worth putting this in its own method here.
						SyncSceneUnitToDictionaryUnit (selectedUnit, selectedGameObject); //syncs the values in the Runit component on the board with the SquareDictionary values in the Game. 
						board.DeselectPiece (); //hides the unit info HUD, deselects the piece in here and sets the gameController unitSelected values etc to null;
						game.CheckForGameOver (); 

					}
				}

				for (int i = 0; i < board.mergeableSquareCoords.Count; i++) {

					int[] intArray = board.mergeableSquareCoords [i];
					if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
						//Debug.Log("merge piece requested");

						mergeUnit = game.squareDictionary [stringCoords].unitOccupyingSquare;
						//Debug.Log (mergeUnit.coords + " merge unit coords are this and is there a unit occupying square? " + game.squareDictionary[stringCoords].squareOccupied + " and stringcoords are " + stringCoords);
						//originalPiece = selector.selectedPiece;
						if (selectedUnit.numMoves > 0 && mergeUnit.numMoves > 0) {
			
							//tells UI controlelr to request merge or not.						
								UnityAction mergeAction = new UnityAction (() => {
									uiController.MergeUnits (); //hides hUD and calls Gamecontroller.MergeUnits
								});

								UnityAction cancelAction = new UnityAction (() => {
									uiController.CancelInput();	//cancels the merge;
								});
									
								uiController.PromptUser("Do you want to merge units of strength " + selectedUnit.strength + " and  new unit " + mergeUnit.strength + "?", mergeAction, cancelAction);
						} else {
							uiController.SetBasicInfoText ("Both units don't have enough moves to merge!", "Sorry!");
							uiController.ShowHUD (uiController.BasicInfoPopup);
						}
					}
				}
				for (int i = 0; i < board.battleSquareCoords.Count; i++) {
					int[] intArray = board.battleSquareCoords [i];

					if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
						enqueueSystem.enquedPieceMovement.Add (selectedGameObject.GetComponent<RUnit> ().coords, intCoords);
						selectedGameObject.GetComponent<RUnit> ().MoveTowardsAPlace (intCoords);
						RUnit defender = game.squareDictionary [stringCoords].unitOccupyingSquare;
						RUnit attacker = selectedUnit;
						bool attackerWin = game.DoBattle (attacker, defender);

						if (attackerWin) {
							uiController.SetBasicInfoText ("You won! Congratulations!", "Okay");
							uiController.ShowHUD (uiController.BasicInfoPopup);
							uiController.HideHUD (uiController.UnitHUD);
							DestroyUnitByUnitDictionary (defender);
							SyncSceneUnitToDictionaryUnit (attacker, selectedGameObject);

						} else {
							uiController.SetBasicInfoText ("Oh no, you lost", "Dammit");
							uiController.ShowHUD (uiController.BasicInfoPopup);
							DestroyUnitByUnitDictionary (attacker);
							SyncSceneUnitToDictionaryUnit (defender, FindUnitByUnitDictionary (defender));
						}
						//Debug.Log ("done battle"); 

						board.DeselectPiece ();
						game.CheckForGameOver ();
					}
				}
				// a piece is currently selected

	

			}
		}
		
		}

		board.RegenerateFogOfWar ();
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
				//Debug.Log ("successfully found gameobject");
				return thisUnit.gameObject;

			}
		}
		Debug.Log ("did not find a gameobject of the coords " + unitToFind.coords);
		return null;
	}

//	public void PromptUser(){
//		uiController.WaitForUser("Do you want to merge units of strength " + selectedUnit.strength + " and  new unit " + mergeUnit.strength + "?", new UnityAction ( () => {
//			uiController.MergeUnits();
//		}), new UnityAction( () => {
//			uiController.CancelInput();
//		}));
//	}





	public void MergeUnits(){
		//Debug.Log("game controller says merge units at " + originalPiece.GetComponent<RUnit>().coords + " , and the mergebble piece at " + mergeUnit.coords);	

		GameObject gameObjectOfMergeUnit = FindUnitByUnitDictionary (mergeUnit);
		enqueueSystem.enquedPieceMovement.Add (selectedUnit.coords, ConvertStringToArray (mergeUnit.coords, 2));
		game.MergePiece (selectedUnit.coords, mergeUnit.coords);
		SyncSceneUnitToDictionaryUnit (mergeUnit, gameObjectOfMergeUnit);
		if (selectedGameObject.GetComponent<RUnit>().unitMount != null) {
			selectedGameObject.GetComponent<RUnit>().unitMount.transform.SetParent (this.transform);
			selectedGameObject.GetComponent<RUnit> ().unitMount.transform.position = new Vector3 (-50, 0f, -50);
		}
		Destroy (selectedGameObject);
		board.DeselectPiece ();
		board.RegenerateFogOfWar ();
	
	}


	public void SyncSceneUnitToDictionaryUnit(RUnit squareDictionaryRUnit, GameObject unitInScene){
		RUnit unitInSceneRUnit = unitInScene.GetComponent<RUnit> ();
		unitInSceneRUnit.allegiance = squareDictionaryRUnit.allegiance;
		unitInSceneRUnit.coords = squareDictionaryRUnit.coords;
		unitInSceneRUnit.numMoves = squareDictionaryRUnit.numMoves;
		unitInSceneRUnit.strength = squareDictionaryRUnit.strength;
		unitInSceneRUnit.unitType = squareDictionaryRUnit.unitType;

	}

	public void EndGame(Mark winner){
		string winnerString = "";
		string descriptionText;

		if (winner == Mark.CON) {
			winnerString = "Confederates ";
		} else if (winner == Mark.USA) {
			winnerString = "United States ";
		} 

		descriptionText = winnerString + "won the game. Play Again?";
		if (winner == Mark.None) {
			descriptionText = "Stalemate! You both lost. Play Again?";
		}

		uiController.SetBasicInfoText (descriptionText, "Yes");

		uiController.RequestResetGame ();
	
	}


	public void AddEnqueuedItem(string originalPiece, int[] coordsToMoveTo){
		//adds move to the enqueue system list (i.e. to show player moves. In future this will be done with UnityActions rather than just wtih ints 
		if (localPlayerController.myAllegiance != game.control) {
			if (!enqueueSystem.enquedPieceMovement.ContainsKey(originalPiece)) {


				enqueueSystem.enquedPieceMovement.Add (originalPiece, coordsToMoveTo); //update enqueue system on opposite machine. When game is reset, it will call enqueueSystem.MoveAllPieces
			} else {
				//Debug.Log ("this is already in existence");
			}
		}
	}

	public int[] DefineStartPositions(){
		ArrayList starterSquares = new ArrayList ();
		int[] starterSquaresArray = new int[5];
		for (int i = 0; i < 5; i++) {
			int randNum = Mathf.RoundToInt(Random.Range(0f, 7f));

			if(starterSquares.Contains(randNum) || randNum == 4){
				i--;
			}else{
				starterSquares.Add(randNum);
			}
		}
		int y = 0;
		foreach (int z in starterSquares) {
			starterSquaresArray[y] = z;
		//	Debug.Log ("starterSquaresArray is" + starterSquaresArray [y]);
			y++;

		}

		return starterSquaresArray;
	}



}
