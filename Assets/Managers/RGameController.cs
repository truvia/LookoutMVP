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

	public const string DidRequestEndTurn = "UIController.DidRequestEndTurn";

	private RBoard board;
	//private RSelector selector;
	private UIController uiController;
	private EnqueueSystem enqueueSystem;

	public RUnit selectedUnit;
	public GameObject selectedGameObject;
	public bool unitSelected = false;
	private RUnit mergeUnit;
	private List<Battle> battleHistory = new List<Battle>();

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
		//Clears the board in Scene view, destroys everything and then reinstantiates according to the master map of the board in game.SquareDictionary
		board.ClearAllSelectorSquares();	
		DestroyAllUnitsInScene ();

		Debug.Log ("GameController . Refresh Board Called");
	
		foreach(KeyValuePair<string, Square> keyValue in game.squareDictionary){
			//Loop through squareDictionary, and instantiate to the physicial board.
			string key = keyValue.Key;
			Square value = keyValue.Value;
			//Debug.Log ("square is " + value.squareOccupied.ToString ());

			if(value.isCitySquare != false){

				board.PlaceCity (value.cityOccupyingSquare);
			}
			if (value.squareOccupied != false) {
				//Debug.Log ("coords are : " + key + " and square unit is " + value.unitOccupyingSquare.allegiance);
				//create unit and sync its values
				board.PlaceUnit (value.unitOccupyingSquare);

			}


		}
		//regenerate fog of war, in line with the new pieces
		board.RegenerateFogOfWar ();
		CheckCityOccupiedStatus ();
	}

	public void RequestChangeTurn(){
		//Actions that should run before the turn is changed should go here.
		BoostTroopsPerTurn();

		EventManager.TriggerEvent (DidRequestEndTurn, this.gameObject);

	}

	public void ChangeTurnAction(object obj){
		//All actions that need to take place immediately after the turn is changed should go here. This is triggered by an Event Listener didChangeTurnNotification

		//Show Player A's movements on Player B's screen immediately after change turn;
		if (localPlayerController.myAllegiance == game.control) {
			enqueueSystem.CallDequeuePieces ();
		}
	}


	public void DestroyAllUnitsInScene(){
		//Kills every unit on the physical board (doesn't clear the squareDictionary though - this is purposeful)
		Debug.Log ("Destroy All Units called");
		RUnit[] units = FindObjectsOfType<RUnit> ();

		foreach (RUnit unit in units) {
			Destroy (unit.gameObject);
		}

		CityObject[] cities = FindObjectsOfType<CityObject> ();
		foreach (CityObject city in cities) {
			Destroy (city.gameObject);
		}
			
	}


		
	void OnBoardSquareClicked(object args){
		//the args are teh coords of the square clicked on. 
		Debug.Log ("onboardsquareclicked");
		if (!enqueueSystem.preventInput) { //prevents bug where user could click in a millisecond before the prevent input panel came up. This panel comes up to prevent user from taking moves before ethey's seen the other player's moves.

			int[] squareClickedIntCoords = (int[]) args; 
		string squareClickedStringCoords = ConvertArrayToString (squareClickedIntCoords);
			//Debug.Log ("boardsquareclicked and enque system hasn't prevented input");
			if (unitSelected) {
			//	Debug.Log ("unit selected");
			// A piece is currently selected

				if (game.control == localPlayerController.myAllegiance) { //if it is my turn;
			//	Debug.Log (" a piece is selected");

					for (int i = 0; i < board.possibleMovementCoords.Count; i++) {  //if there is a moveable Square;
							
					int[] possibleMovementCoords = board.possibleMovementCoords [i];
					if (possibleMovementCoords [0] == squareClickedIntCoords [0] && possibleMovementCoords [1] == squareClickedIntCoords [1]) {
						//Move piece to location

						GameObject gameObjectRunitOfSelectedPiece = FindSceneUnitGameObjectBySquareDictionaryRUnit (selectedUnit);
						enqueueSystem.enquedPieceMovement.Add (selectedUnit.coords, squareClickedIntCoords); //enqueues movement for next player so they can see what moves the enemy took;
							selectedGameObject.GetComponent<UnitObject> ().MoveTowardsAPlace (squareClickedIntCoords); //physicallymoveunit
						game.MovePiece (selectedUnit, squareClickedStringCoords); //move the unit in the unitDictionary - may be worth putting this in its own method here.
						SyncSceneUnitToDictionaryUnit (selectedUnit, selectedGameObject); //syncs the values in the Runit component on the board with the SquareDictionary values in the Game. 
						CheckDefensiveBonus (selectedUnit);
						board.DeselectPiece (); //hides the unit info HUD, deselects the piece in here and sets the gameController unitSelected values etc to null;
						game.CheckForGameOver (); 

					}
				}

				for (int i = 0; i < board.mergeableSquareCoords.Count; i++) {

					int[] intArray = board.mergeableSquareCoords [i];
					if (intArray [0] == squareClickedIntCoords [0] && intArray [1] == squareClickedIntCoords [1]) {
						//Debug.Log("merge piece requested");
							mergeUnit = FindSquareDictionrayUnitByCoords(squareClickedStringCoords);
						//Debug.Log (mergeUnit.coords + " merge unit coords are this and is there a unit occupying square? " + game.squareDictionary[stringCoords].squareOccupied + " and stringcoords are " + stringCoords);
			
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

					if (intArray [0] == squareClickedIntCoords [0] && intArray [1] == squareClickedIntCoords [1]) {
						enqueueSystem.enquedPieceMovement.Add (selectedGameObject.GetComponent<RUnit> ().coords, squareClickedIntCoords);
						selectedGameObject.GetComponent<UnitObject> ().MoveTowardsAPlace (squareClickedIntCoords);
							Debug.Log ("battle square coords are " + ConvertArrayToString(squareClickedIntCoords));
							selectedGameObject.GetComponent<UnitObject> ().ShowBattle (squareClickedIntCoords);
						

						RUnit defender = FindSquareDictionrayUnitByCoords (squareClickedStringCoords);
						RUnit attacker = selectedUnit;
						bool attackerWin = game.DoBattle (attacker, defender);

						int defenderStartStrength = defender.strength;
						int attackerStartStrength = attacker.strength;

						if (attackerWin) {
							int losses = attackerStartStrength - attacker.strength;
							Battle newBattle = new Battle ();
							newBattle.SetWinner (attacker.allegiance);
							newBattle.SetLosses (attacker.allegiance, losses, attackerStartStrength);
							newBattle.SetBattleTime ();
							battleHistory.Add (newBattle);		
							
							uiController.SetBasicInfoText ("You won! Your losses were " + newBattle.GetLossLevel().ToString() + ", amounting to: " + losses, "Okay");
							uiController.ShowHUD (uiController.BasicInfoPopup);
							uiController.HideHUD (uiController.UnitHUD);
							DestroyUnitByUnitDictionary (defender);
							CheckDefensiveBonus (attacker); //as the attacker may have just taken over a city, check their defensive bonus;
							
							

							SyncSceneUnitToDictionaryUnit (attacker, selectedGameObject);

						} else {
							uiController.SetBasicInfoText ("Oh no, you lost", "Dammit");
							uiController.ShowHUD (uiController.BasicInfoPopup);
							DestroyUnitByUnitDictionary (attacker);
							SyncSceneUnitToDictionaryUnit (defender, FindSceneUnitGameObjectBySquareDictionaryRUnit (defender));
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
		CheckCityOccupiedStatus ();
	}

	void CheckCityOccupiedStatus(){
		CityObject[] cityObjects = FindObjectsOfType<CityObject> ();
		foreach (KeyValuePair<string, Square> keyValue in game.squareDictionary) {
			string coords = keyValue.Key;
			Square square = keyValue.Value;

			if (square.isCitySquare) {
				if (square.squareOccupied) {
					square.cityOccupyingSquare.occupiedBy = square.unitOccupyingSquare.allegiance;
				} else {
					square.cityOccupyingSquare.occupiedBy = Mark.None;
				}

				SyncCity (square.cityOccupyingSquare, FindCityObjectBySquareDictionary (square.cityOccupyingSquare).gameObject);
			}
		}
	}

	public CityObject FindCityObjectBySquareDictionary(City city){
		CityObject[] cityObjects = FindObjectsOfType<CityObject> ();
		foreach (CityObject cityObject in cityObjects) {
			if (city.coords == cityObject.coords) {
				return cityObject;
			}
		}
		Debug.LogError ("No cityObjects found in scene");
		return null;
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

	public GameObject FindSceneUnitGameObjectBySquareDictionaryRUnit(RUnit unitToFind){
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


	public void MergeUnits(){
		//Debug.Log("game controller says merge units at " + originalPiece.GetComponent<RUnit>().coords + " , and the mergebble piece at " + mergeUnit.coords);	

		GameObject gameObjectOfMergeUnit = FindSceneUnitGameObjectBySquareDictionaryRUnit (mergeUnit);
		enqueueSystem.enquedPieceMovement.Add (selectedUnit.coords, ConvertStringToArray (mergeUnit.coords, 2));
		game.MergePiece (selectedUnit.coords, mergeUnit.coords);
		CheckDefensiveBonus (mergeUnit);
		SyncSceneUnitToDictionaryUnit (mergeUnit, gameObjectOfMergeUnit);
		if (selectedGameObject.GetComponent<UnitObject>().unitMount != null) {
			selectedGameObject.GetComponent<UnitObject>().unitMount.transform.SetParent (this.transform);
			selectedGameObject.GetComponent<UnitObject> ().unitMount.transform.position = new Vector3 (-50, 0f, -50);
		}
		Destroy (selectedGameObject);
		board.DeselectPiece ();
		board.RegenerateFogOfWar ();
	
	}


	public void SyncSceneUnitToDictionaryUnit(RUnit squareDictionaryRUnit, GameObject unitInScene){
		UnitObject unitInstanceInScene = unitInScene.GetComponent<UnitObject> ();

		unitInstanceInScene.allegiance = squareDictionaryRUnit.allegiance;
		unitInstanceInScene.coords = squareDictionaryRUnit.coords;
		unitInstanceInScene.numMoves = squareDictionaryRUnit.numMoves;

		unitInstanceInScene.unitType = squareDictionaryRUnit.unitType;

		unitInstanceInScene.defensiveBonus = squareDictionaryRUnit.defensiveBonus;
		unitInstanceInScene.strength = squareDictionaryRUnit.strength;
	}

	public void SyncCity(City squareDictionaryCity, GameObject objectInScene){
		
		CityObject cityInScene = objectInScene.GetComponent<CityObject> ();
		cityInScene.baseDefence = squareDictionaryCity.baseDefence;
		cityInScene.coords = squareDictionaryCity.coords;
		cityInScene.occupiedBy = squareDictionaryCity.occupiedBy;
		cityInScene.replenishRatePerTurn = squareDictionaryCity.replenishRatePerTurn;
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


	public bool IsSquareOccupied(string coords){
		bool isSquareOccupied = game.squareDictionary [coords].squareOccupied;

		return isSquareOccupied;
	}

	public RUnit FindSquareDictionrayUnitByCoords(string coords){
		RUnit thisUnit;
		if (IsSquareOccupied (coords)) {
			thisUnit = game.squareDictionary [coords].unitOccupyingSquare;
		} else {
			Debug.LogError("ERROR! - there is no unit on the square in game.SquareeDictionary at these coords: " + coords + ". Did you forget to run an if statement for FindIfSquareOccupiedByCoords in gameController?");
			return null;
		}
	
		return thisUnit;

	}

	public void CheckDefensiveBonus(RUnit squareDictionaryUnit){
		if (game.squareDictionary [squareDictionaryUnit.coords].isCitySquare) {
			int defensiveBonus = game.squareDictionary [squareDictionaryUnit.coords].cityOccupyingSquare.baseDefence;

			squareDictionaryUnit.AddDefensiveBonus (defensiveBonus);

		} else {
			squareDictionaryUnit.RemoveDefensiveBonus ();

		}

		SyncSceneUnitToDictionaryUnit (squareDictionaryUnit, FindSceneUnitGameObjectBySquareDictionaryRUnit (squareDictionaryUnit));

	}

	public void BoostTroopsPerTurn(){
		if (game.control == localPlayerController.myAllegiance) { //to make sure it only updates strength every round, rather than every turn.
			foreach (KeyValuePair<string, Square> keyValue in game.squareDictionary) {
				string coords = keyValue.Key;
				Square square = keyValue.Value;

				if (square.isCitySquare && square.squareOccupied) {
					square.unitOccupyingSquare.strength += square.cityOccupyingSquare.replenishRatePerTurn;
					SyncSceneUnitToDictionaryUnit (square.unitOccupyingSquare, FindSceneUnitGameObjectBySquareDictionaryRUnit (square.unitOccupyingSquare));
				}
			}
		}
	}

}
