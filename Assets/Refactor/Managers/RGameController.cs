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
	private Display display;

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
		display = FindObjectOfType<Display> ();

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

	void RefreshBoard(object obj){
		DestroyAllUnits ();
		//Debug.Log ("GameController . Refresh Board Called");
	
		foreach(KeyValuePair<string, Square> keyValue in game.squareDictionary){
			string key = keyValue.Key;
			Square value = keyValue.Value;

			//Debug.Log ("square is " + value.squareOccupied.ToString ());

			if (value.squareOccupied != false) {
				//Debug.Log ("coords are : " + key + " and square unit is " + value.unitOccupyingSquare.allegiance);
				int[] coords = ConvertStringToArray (key, 2);
				board.Place (coords, value.unitOccupyingSquare.allegiance, value.unitOccupyingSquare);

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
				if (game.squareDictionary [stringCoords].unitOccupyingSquare.allegiance == localPlayerController.myAllegiance) {
					//there is a piece at this location in the square dictionary and it is my allegiance;
					if (selector.pieceAtThisCoord) {
						//to prevent a bounceback if there is no piece at this coord
						selector.SelectPiece (selector.pieceAtThisCoord);
						board.ShowPossibleSquares (intCoords, selector.pieceAtThisCoord.GetComponent<RUnit> ());

					} else {
						Debug.Log (this + " says that there is no selector.pieceAtThiscoord");	
					
					}
				} 
			}

		} else {
			// A piece is currently selected

			foreach (int[] intArray in board.possibleMovementCoords) {
				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					//Move piece to location
					Debug.Log("Move piece requested");
					game.MovePiece (selector.selectedPiece.GetComponent<RUnit> ().coords, stringCoords);
					selector.PlacePiece (stringCoords);
					board.ClearAllSelectorSquares ();

				}
			}

			foreach (int[] intArray in board.mergeableSquareCoords) {
				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					//prompt merge square
					Debug.Log("merge piece requested");
					//prompt mergesquare;

					RUnit mergeUnit = game.squareDictionary [stringCoords].unitOccupyingSquare;
					GameObject originalPiece = selector.pieceAtThisCoord;

					display.TestMerge (originalPiece, mergeUnit);
					display.ShowHUD (display.MergeUnitHUD);
					//if they say that it is fine
					  
//					if (display.mergetrue) {
//						
//					}

				}
			}

			foreach (int[] intArray in board.battleSquareCoords) {
				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [1]) {
					//Prompt do battle
					Debug.Log("do battle requested");
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




}
