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



	void OnBoardSquareClicked(object args){
		if (game.control == Lookout.Mark.None) {
			Debug.Log ("Nobody is in control so reset game");
			game.Reset ();

		} else {
		
			Debug.Log ("OnBoardSquareClicked now pass to place");
			game.Place((int)args);
		
		}
		
	}

	void OnDidOccupySquare(object args){

		Debug.Log(this.name + "says : OnDidOccupySquare has been called");
		int index = (int)args;
		Mark mark = game.board [index];
		board.Show (index, mark);
	
	}

	void OnDidBeginGame(object args){
	
		Debug.Log ("Did Begin Game");
	
	}
}
