using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RLookout{

	// Use this for initialization
	public enum Mark {
		None,
		CON,
		USA
	}

	public enum UnitType{
		Army,
		Fortress,
		Spy,
		None

	}

	public class RGame {
		//messages?
		public const string DidBeginGameNotification = "Game.DidBeginGameNotification";
		public const string DidChangeTurnNotification = "Game.DidChangeTurnNotification";
		//public const string BeginGameSetup = "Game.BeginGameSetup";

		//Actions

		public int[] conArmiesStartLocationStrengths = new int[3];
		public int[] uSAArmiesStartLocationStrengths = new int[3];

		public Mark control { get; private set;}
		public Mark winner {get; private set;}
		public Mark startPlayer = Mark.CON; //sets start player as the Confederates
		public Dictionary<string, Square> squareDictionary = new Dictionary<string, Square>();
		public int boardWidth = 5;
		public int boardHeight = 5;

	



		string[] wins = new string[] {
			"0,4", "4,0"
		};


		//only the host can call a clientRPc 
		public void ChangeTurn(){
			Debug.Log ("ChangeTurn called by " + control);
			control = (control == Mark.CON) ? Mark.USA : Mark.CON;

			//trigger event usually
			EventManager.TriggerEvent(DidChangeTurnNotification);
		}


		void CheckForGameOver(){
			if (CheckForWin ()){
				Debug.Log ("Check for win is true");
				control = Mark.None;
			// Trigger Event	EventManager.TriggerEvent (DidEndGameNotification);					}
			}
		}
			
		bool CheckForWin(){
			return false;
		}

		bool CheckForSTaleMate(){
			return false;	
		}

		public void ResetGame(){
			Debug.Log ("Reset is called");	
			squareDictionary.Clear ();

			//Creates a new squre dictionary and pre-poluates with emmpty squares
				for (int z = 0; z < boardWidth; z++) {
					for (int x = 0; x < boardHeight; x++) {
					string coordsAsString = x.ToString () + " , " + z.ToString ();

					Square newSquare = new Square ();
							
					squareDictionary.Add (coordsAsString, newSquare);

					}

				}

				control = startPlayer;
				winner = Mark.None;
				InitialGameSetup ();
			//CmdDefineConStartPosition ();
				EventManager.TriggerEvent (DidBeginGameNotification);

		}



		void InitialGameSetup(){

			//Defines Possible Start Squares
			string[] USPossibleStartSquares = new string[] {
				"3 , 0",
				"3 , 1",
				"4 , 1"
			};

			string[] ConPossibleStartSquares = new string[] {
				"0 , 3",
				"1 , 3",
				"1 , 4"
			};

			int i = 0;

			foreach (int strength in uSAArmiesStartLocationStrengths) {
				if (strength != 0) {
							
					ConstructNewUnit (USPossibleStartSquares [i], Mark.USA, UnitType.Army, strength);		

				}
				i++;

			}
			i = 0;

			foreach (int strength in conArmiesStartLocationStrengths) {
				if (strength != 0) {
					ConstructNewUnit (ConPossibleStartSquares [i], Mark.CON, UnitType.Army, strength);		

				}
				i++;
			}
				
//
			//FORTRESSES
			string USFortress = "4 , 0"; 
			string CONFortress = "0 , 4";

			ConstructNewUnit (USFortress, Mark.USA, UnitType.Fortress, 5000);
			ConstructNewUnit (CONFortress, Mark.CON, UnitType.Fortress, 5000);


		}

			public void ConstructNewUnit(string Coords, Mark allegiance, UnitType unitType, int strength){

			//GameObject obj = new GameObject ();
				RUnit newUnit = new RUnit();
			newUnit.allegiance = allegiance;
			newUnit.strength = strength;
			newUnit.unitType = unitType;
			newUnit.coords = Coords;
			//newUnit.name = "New " + allegiance.ToString () + " Army";
			squareDictionary [Coords].unitOccupyingSquare = newUnit;
			squareDictionary [Coords].squareOccupied = true;
			//Debug.Log ("this is new" + squareDictionary [Coords].squareOccupied);
			//Debug.Log("Construct New Unit: " + squareDictionary[Coords].unitOccupyingSquare.ToString());
		}

		public void DefineStartPositions(int[] CONArmiesStrength, int[] USArmiesStrength){
			conArmiesStartLocationStrengths = CONArmiesStrength;
			uSAArmiesStartLocationStrengths = USArmiesStrength;

			//called from player controller right now....
		

		}



		private void LoopThroughUnitDictionary(){
			foreach (KeyValuePair<string, Square> keyValue in squareDictionary) {
				string key = keyValue.Key;
				Square value = keyValue.Value;

				//Debug.Log ("Game.LoopThroughUnitDictionary: coords are " + key + " and unit occupying square is " + value.squareOccupied);
			} 
		}

		public void MovePiece(string selectedPieceCoords, string squareClickedCoords){
				
			squareDictionary [squareClickedCoords].unitOccupyingSquare = squareDictionary [selectedPieceCoords].unitOccupyingSquare;
			squareDictionary [squareClickedCoords].squareOccupied = true;

			squareDictionary [selectedPieceCoords].unitOccupyingSquare = null;
			squareDictionary [selectedPieceCoords].squareOccupied = false;
			//Debug.Log ("Unit has been moved from " + selectedPieceCoords + " to " + squareClickedCoords + " and this is confirmed in the squareDictionary because the new square we clicked on has this unit on it " + squareDictionary [squareClickedCoords].unitOccupyingSquare + " and the original square is now empty (false if true) " + squareDictionary [selectedPieceCoords].squareOccupied);
		}

	

	}

}