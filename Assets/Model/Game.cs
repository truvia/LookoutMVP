using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Lookout
{

	// Use this for initialization
	public enum Mark {
		None,
		CON,
		USA
	}




	public class Game{


		public const string DidBeginGameNotification = "Game.DidBeginGameNotification";
		public const string DidOccupySquareNotification = "Game.DidOccupySquareNotification";
		public const string DidChangeControlNotification = "Game.DidChangeControlNotification";
		public const string DidEndGameNotification = "Game.DidEndGameNotification";

		public Mark control { get; private set;}
		public Mark winner {get; private set;}
		public Mark[] board {get; private set;}
		public Mark startPlayer = Mark.CON; //sets start player as the Confederates

		private int boardSize;
		private Board boardView;
		//private Dictionary <int[], Mark> boardDictionary;


		int[] wins = new int[] {
			5, 21
		};

		void Start(){
			boardView = GameObject.FindObjectOfType<Board> ();
			boardSize = boardView.height * boardView.width;


		}

		public Game (){
			board = new Mark[boardSize];

//			if (boardDictionary == null) {
//				boardDictionary = new Dictionary<int[], Mark> ();
//
//			}
		}

		public void Reset(){
			for (int i = 0; i < boardSize; i++) {
				board [i] = Mark.None;
				control = startPlayer; //Sets the start player as the Confederates.
				winner = Mark.None;

				EventManager.TriggerEvent (DidBeginGameNotification);
				 
			}
		
		}

		public void Place (int index){
			Debug.Log ("board at index " + index + " is " + board [index]);

			if (board [index] != Mark.None) {
				return;
			}

			board[index] = control;

			EventManager.TriggerEvent (DidOccupySquareNotification, index);


				CheckForGameOver();

			if(control != Mark.None){
				ChangeTurn();
			
			}
		}
		


				void ChangeTurn(){
					Debug.Log ("ChangeTurn called");
			control = (control == Mark.CON) ? Mark.USA : Mark.CON;
					EventManager.TriggerEvent (DidChangeControlNotification);

				}

			void CheckForGameOver(){

				if (CheckForWin ()) {
				Debug.Log ("Check for win is true");
					control = Mark.None;
					
				EventManager.TriggerEvent (DidEndGameNotification);					}
				
				}

		bool CheckForWin(){
			Debug.Log ("board wins: " + board [wins [0]]);
			Debug.Log ("board wins: " + board [wins [1]]);
			Mark a = board[wins[0]];
			Mark b = board[wins[1]];	

			if (a == b && a != Mark.None) {
				winner = a;
				return true;
			}

			return false;

		}

	}





}
