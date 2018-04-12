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
	// Use this for initialization

	public RGame game = new RGame();
	public RPlayerController localPlayerController;

	void Awake(){
		didBeginGameNotificationAction = new UnityAction<System.Object> (RefreshBoard); //defines what action that this object should take when the event is triggered
		didChangeTurnNotificationAction = new UnityAction<System.Object>(RefreshBoard);
		//onBoardSquareClickedNotificationAction = new UnityAction<System.Object>(OnBoardSquareClicked);
		didStartLocalPlayerNotificationAction = new UnityAction<System.Object> (IdentifyLocalPlayer);
		board = FindObjectOfType<RBoard> ();
		selector = FindObjectOfType<RSelector> ();
	}

	void OnEnable(){
		EventManager.StartListening(RGame.DidBeginGameNotification, didBeginGameNotificationAction); 
		EventManager.StartListening (RGame.DidChangeTurnNotification, didChangeTurnNotificationAction);
		//EventManager.StartListening (RBoard.SquareClickedNotification, onBoardSquareClickedNotificationAction);
		EventManager.StartListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);
	}

	void OnDisable(){
		EventManager.StopListening(RGame.DidBeginGameNotification, didBeginGameNotificationAction); 
		EventManager.StopListening (RGame.DidChangeTurnNotification, didChangeTurnNotificationAction);
		//EventManager.StopListening (RBoard.SquareClickedNotification, onBoardSquareClickedNotificationAction);
		EventManager.StopListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);
	}

	void RefreshBoard(object obj){
		DestroyAllUnits ();
		Debug.Log ("GameController . Refresh Board Called");
	
		foreach(KeyValuePair<string, Square> keyValue in game.squareDictionary){
			string key = keyValue.Key;
			Square value = keyValue.Value;

			//Debug.Log ("square is " + value.squareOccupied.ToString ());

			if (value.squareOccupied != false) {
				//Debug.Log ("coords are : " + key + " and square unit is " + value.unitOccupyingSquare.allegiance);
				int[] coords = ConvertStringToArray (key, 2);
				board.Show (coords, value.unitOccupyingSquare.allegiance, value.unitOccupyingSquare);

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
		
//	void OnBoardSquareClicked(object args){
//		Debug.Log ("RGameController.OnboardSquareClicked");
//		int[] intCoords = (int[]) args; 
//		string stringCoords = ConvertArrayToString (intCoords);
//
//		if (selector.selectedPiece == null) {
//			//if no piece is currently selected
//
//			if (game.squareDictionary [stringCoords].squareOccupied) {
//				if (game.squareDictionary [stringCoords].unitOccupyingSquare.allegiance == localPlayerController.myAllegiance) {
//					//there is a piece at this location in the square dictionary and it is my allegiance;
//
//					selector.originalParent = selector.pieceAtThisCoord.transform.parent.gameObject;
//					selector.originalPosition = selector.pieceAtThisCoord.transform.position;
//					selector.selectedPiece = selector.pieceAtThisCoord;
//
//					board.ShowPossibleSquares (intCoords, selector.pieceAtThisCoord.GetComponent<RUnit>());
//				}
//			}
//
//		} else {
//			foreach (int[] intArray in board.possibleMovementCoords) {
//				if (intArray [0] == intCoords [0] && intArray [1] == intCoords [2]) {
//					//Move piece to location
//				}
//			}
//			// a piece is currently selected
//		
//		}
//
//			//if a piece is selected
//			//RSelector already only selects pieces that belong to us
//					
//		// If there is a piece where we've clicked:
//			/* check if we're already selected a piece or not
//			 * if we havent, check if the piece belongs to us
//			 * if the piece is ours check the possible movement squares and the possible takeable squares and highlight them
//			 */ 
//			 
//
//			/* 
//			If a piece is already selected
//			check if where we're clicking is in the moveable,  takeable or mergeable squares
//			if in the moveable square, place the piece and reduce the number of moves left;
//			if in the takeable square, Game.DoBattle
//				if, do battle resutls in win and destroy, destroy enemy piece and move this on the square and weaken by relevant amount
//				if DoBattle results in loss and destroy, destroy my piece, weaken enemy by relevant amount
//				If DoBattle results in stalemate, don't move piece and make necessary reductions in strneght to both
//			if in the mergeable square, destroy this army, and add strength to the relevant unit on the square
//			
//			if we've outside of all, do nothing.
//				*/
//	}

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




}
