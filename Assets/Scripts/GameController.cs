using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;

public class GameController : MonoBehaviour {

	public Game game = new Game();
	public Board board;
	private NotificationCentre2 notificationCentre = new NotificationCentre2 ();

	void Start(){
		notificationCentre = GameObject.FindObjectOfType<NotificationCentre2> ();
		board = GetComponentInChildren<Board> ();
		game.Reset ();
	}


	void OnEnable(){
		notificationCentre.AddObserver (this.gameObject, "Board.SquareClickedNotification");
		notificationCentre.AddObserver (this.gameObject, "Lookout.Game.DidBeginGameNotification");
		notificationCentre.AddObserver (this.gameObject, "Lookout.Game.DidOccupySquareNotification");	
	}

	void OnBoardSquareClicked(object sender, object args){
		if (game.control == Lookout.Mark.None) {
			game.Reset ();
		} else {
			game.Place((int)args);
		}
		
	}

	void OnDidOccupySquare(object sender, object args){
	
		int index = (int)args;
		Mark mark = game.board [index];
		board.Show (index, mark);
	}
}
