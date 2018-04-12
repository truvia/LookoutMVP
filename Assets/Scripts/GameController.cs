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

	void Update(){

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
	
		Debug.Log ("onboardsquareclicked");

		string coords = Game.convertArrayToString (coordsAsInt);
		//if we have clicked on a selected square

		if(board.possibleMovementCoords.Any(p => p.SequenceEqual(coordsAsInt))){
			//first check if it is in a moveable square

			Debug.Log ("Clicekd and there is a possible movementsquare");

			if (board.battleSquareCoords.Any (p => p.SequenceEqual (coordsAsInt))) {

				Debug.Log ("Clicekd and there is a possible battlesquare");
				//if it is a battle square, do battle
				bool battleWon = game.DoBattle (coords); 

				if (battleWon) {
					
					game.CmdWipeUnit (coords);
					game.CmdPlace (coords);
					CmdRefreshBoard ();

				} else {
					game.CmdWipeUnit (game.selectedCoords);
					game.selectedCoords = null;
					CmdRefreshBoard ();
				//	N.B. we've use refresh board to hide the selected unit, as there is some kind of bug meaning that the selctor is not picking up the unit for some reason.
				//	selector.KillSelectedPiece ();
					game.CheckForGameOver ();
					game.CmdChangeTurn ();

				}
			
			} else {

				Debug.Log ("Clicekd and there are no possible battlesquares");
				game.CmdPlace (coords);
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

			Debug.Log ("cliked and others in control so keep palying, show whatever possible squares there are ");
			
		//is there an object at this location;

			if (game.unitDictionary [coords].allegiance != Mark.None && game.unitDictionary[coords].allegiance == game.control) {
				Debug.Log ("I've clicked on a unit that is the same as game.control which is " + game.control);

			//I have clicked on my unit
				game.selectedCoords = coords;
				board.ChangeText (game.unitDictionary [coords].strength);
			
			board.ShowPossibleSquares (coordsAsInt, game.unitDictionary[coords]);
		

		}
			//game.Place(coords);

		}
		
	}





	void OnDidBeginGame(object args){
		CmdRefreshBoard ();
		Debug.Log ("Did Begin Game");
	
	}

	[Command]
	void CmdRefreshBoard(){

		RpcRefreshBoard ();

	}

	[ClientRpc]
	void RpcRefreshBoard(){

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
	