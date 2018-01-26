using System.Collections;
using UnityEngine;

namespace Lookout
{

	// Use this for initialization
	public enum Mark {
		None,
		CON,
		USA
	}



	public class Game{

		public NotificationCentre2 notificationCentre = new NotificationCentre2();

		public const string DidBeginGameNotification = "Game.DidBeginGameNotification";
		public const string DidOccupySquareNotification = "Game.DidOccupySquareNotification";
		public const string DidChangeControlNotification = "Game.DidChangeControlNotification";
		public const string DidEndGameNotification = "Game.DidEndGameNotification";

		public Mark control { get; private set;}
		public Mark winner {get; private set;}
		public Mark[] board {get; private set;}
		public int boardSize = 25;
		public Mark startPlayer = Mark.CON; //sets start player as the Confederates

		

		int[] wins = new int[] {
			5, 21
		};

		void Start(){
			notificationCentre = GameObject.FindObjectOfType<NotificationCentre2> ();
		}

		public Game (){
			board = new Mark[boardSize];

		}

		public void Reset(){
			for (int i = 0; i < boardSize; i++) {
				board [i] = Mark.None;
				control = startPlayer; //Sets the start player as the Confederates.
				winner = Mark.None;



				notificationCentre.PostNotification (DidBeginGameNotification, this);

				//this.PostNotification (DidBeginGameNotification);
			//	BroadcastMessage() 
			}
		
		}

		public void Place (int index){
			if (board [index] != Mark.None) {
				return;
			}
				board[index] = control;
				notificationCentre.PostNotification(DidOccupySquareNotification, index);
					
				CheckForGameOver();

			if(control != Mark.None){
				ChangeTurn();
			
			}
		}
		


				void ChangeTurn(){
		
					control = (control == Mark.CON) ? Mark.CON : Mark.USA;
			notificationCentre.PostNotification (DidChangeControlNotification, this);


				}

			void CheckForGameOver(){
			
				if (CheckForWin ()) {
					control = Mark.None;
					notificationCentre.PostNotification (DidEndGameNotification, this);
					}
				
				}

		bool CheckForWin(){
			Mark a = board[wins[0]];
			Mark b = board[wins[1]];	

			if (a == b) {
				winner = a;
				return true;
			}

			return false;

		}

	}





}
