using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;
using UnityEngine.Events;

public class GameController : MonoBehaviour {

	public Game game = new Game();
	public Board board;

	private UnityAction<System.Object> squareClickedNotificationAction;
	private UnityAction<System.Object> didOccupySquareNotificationAction;
	private UnityAction<System.Object> didBeginGameNotificationAction;

	void Awake(){
		//Listeners - what methods should listen to the notification
		squareClickedNotificationAction = new UnityAction<System.Object> (OnBoardSquareClicked); //defines what action that this object should take when the event is triggered
		didOccupySquareNotificationAction = new UnityAction<System.Object>(OnDidOccupySquare);
		didBeginGameNotificationAction = new UnityAction<System.Object> (OnDidBeginGame);
	
	}

	void Start(){
		board = GetComponentInChildren<Board> ();
		game.Reset ();

	}


	void OnEnable(){
		//GameController is listening to the following: 
		EventManager.StartListening(Board.SquareClickedNotification, squareClickedNotificationAction); 
		EventManager.StartListening(Lookout.Game.DidBeginGameNotification, didBeginGameNotificationAction);
		EventManager.StartListening(Lookout.Game.DidOccupySquareNotification, didOccupySquareNotificationAction);
	
	}

	void OnDisable(){
		EventManager.StopListening(Board.SquareClickedNotification, squareClickedNotificationAction); 
		EventManager.StopListening(Lookout.Game.DidBeginGameNotification, didBeginGameNotificationAction);
		EventManager.StopListening(Lookout.Game.DidOccupySquareNotification, didOccupySquareNotificationAction);
	}



	void OnBoardSquareClicked(object args){
		if (game.control == Lookout.Mark.None) {
			Debug.Log ("Nobody is in control so reset game");
			game.Reset ();

		} else {
			
			string argsAsString = game.convertArrayToString ((int[])args);

			game.Place(argsAsString);

		}
		
	}

	void OnDidOccupySquare(object args){
	
		string coordsAsString = args.ToString ();

	
		int[] coords = game.convertStringToArray (coordsAsString, 2);


		Mark mark = game.boardDictionary [coordsAsString];

		board.Show (coords, mark);
	
	}

	void OnDidBeginGame(object args){
		RefreshBoard ();
		Debug.Log ("Did Begin Game");
	
	}

	void RefreshBoard(){
		foreach (KeyValuePair<string, Mark> keyValue in game.boardDictionary) {
			string key = keyValue.Key;
			Mark value = keyValue.Value;
			int[] coords = game.convertStringToArray (key, 2);
			Debug.Log (key + " , " + value);
			if (value != Mark.None) {
				board.Show (coords, value);
			}

		}

	}
}
	