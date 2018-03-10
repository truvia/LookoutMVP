using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour {

	public Game game = new Game();
	public Board board;

	private UnityAction<System.Object> squareClickedNotificationAction;
	private UnityAction<System.Object> didOccupySquareNotificationAction;
	private UnityAction<System.Object> didBeginGameNotificationAction;
	private Selector selector;



	void Awake(){
		//Listeners - what methods should listen to the notification
		squareClickedNotificationAction = new UnityAction<System.Object> (OnBoardSquareClicked); //defines what action that this object should take when the event is triggered
		//didOccupySquareNotificationAction = new UnityAction<System.Object>(OnDidOccupySquare);
		didBeginGameNotificationAction = new UnityAction<System.Object> (OnDidBeginGame);
	
	}

	void Start(){
		selector = FindObjectOfType<Selector> ();
		board = FindObjectOfType<Board> ();
		game.Reset ();


	}


	void OnEnable(){
		//GameController is listening to the following: 
		EventManager.StartListening(Board.SquareClickedNotification, squareClickedNotificationAction); 
		EventManager.StartListening(Lookout.Game.DidBeginGameNotification, didBeginGameNotificationAction);
		//EventManager.StartListening(Lookout.Game.DidOccupySquareNotification, didOccupySquareNotificationAction);
	
	}

	void OnDisable(){
		EventManager.StopListening(Board.SquareClickedNotification, squareClickedNotificationAction); 
		EventManager.StopListening(Lookout.Game.DidBeginGameNotification, didBeginGameNotificationAction);
	//	EventManager.StopListening(Lookout.Game.DidOccupySquareNotification, didOccupySquareNotificationAction);
	}


	void OnBoardSquareClicked(object args){
		int[] coordsAsInt = (int[])args;
		CmdOnBoardSquareClicked(coordsAsInt);
	}

	[Command]
	void CmdOnBoardSquareClicked(int[] coordsAsInt){
		Debug.Log ("onboardsquareclicked");
		RpcTurnTeller ();
		
		string coords = Game.convertArrayToString (coordsAsInt);
		//if we have clicked on a selected square

		if(board.possibleMovementCoords.Any(p => p.SequenceEqual(coordsAsInt))){
			//first check if it is in a moveable square

			if (board.battleSquareCoords.Any (p => p.SequenceEqual (coordsAsInt))) {
				//if it is a battle square, do battle
				bool battleWon = game.DoBattle (coords); 

				if (battleWon) {
					game.WipeUnit (coords);
					game.Place (coords);
					CmdRefreshBoard ();

				} else {
					game.WipeUnit (game.selectedCoords);
					selector.KillSelectedPiece ();
					game.CheckForGameOver ();
					game.ChangeTurn ();

				}
			
			} else {
				game.Place (coords);
				CmdRefreshBoard ();
			}



			board.possibleMovementCoords.Clear ();
			board.battleSquareCoords.Clear ();
		}

		board.ClearAllSelectorSquares ();
		if (game.control == Lookout.Mark.None) {
			Debug.Log ("Nobody is in control so reset game");

			game.Reset ();

		} else {
			
		//is there an object at this location;

			if (game.unitDictionary [coords].allegiance != Mark.None && game.unitDictionary[coords].allegiance == game.control) {
			//I have clicked on my unit
				game.selectedCoords = coords;
				board.ChangeText (game.unitDictionary [coords].strength);
			
			board.ShowPossibleSquares (coordsAsInt, game.unitDictionary[coords]);
		

		}
			//game.Place(coords);

		}
		
	}

	[ClientRpc]
	void RpcTurnTeller(){
		Debug.Log(game.control);
	}



	void OnDidBeginGame(object args){
		CmdRefreshBoard ();
		Debug.Log ("Did Begin Game");
	
	}

	[Command]
	void CmdRefreshBoard(){
		Unit[] units =	FindObjectsOfType<Unit> ();

		foreach (Unit unit in units) {
			Destroy (unit.gameObject);
		
		}	

		foreach(KeyValuePair<string, Unit> keyValue in game.unitDictionary){
			string key = keyValue.Key;
			Unit value = keyValue.Value;

			int[] coords = game.convertStringToArray(key, 2);

			if (value.allegiance != Mark.None) {
				board.Show (coords, value.allegiance, value);
			
			}


		}

		board.RevertStrengthText ();



	}


}
	