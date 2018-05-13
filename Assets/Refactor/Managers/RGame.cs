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

			ConstructNewUnit (USFortress, Mark.USA, UnitType.Fortress, 2000);
			ConstructNewUnit (CONFortress, Mark.CON, UnitType.Fortress, 2000);


		}

		public void ConstructNewUnit(string Coords, Mark allegiance, UnitType unitType, int strength){
			
			//GameObject obj = new GameObject ();
				RUnit newUnit = new RUnit();
			newUnit.allegiance = allegiance;
			newUnit.strength = strength;
			newUnit.unitType = unitType;
			newUnit.coords = Coords;
			//newUnit.name = "New " + allegiance.ToString () + " Army";

			if (unitType == UnitType.Army) {
				newUnit.numMoves = 1;
			} else if (unitType == UnitType.Fortress) {
				newUnit.numMoves = 0;
			} else if (unitType == UnitType.None) {
				newUnit.numMoves = 0;
			} else if (unitType == UnitType.Spy) {
				newUnit.numMoves = 2;
			} else {
				Debug.Log ("RGame says that you haven't defined the number of moves for the unit type you are trying to construct in ConstructNewUnit. The unitType is " + unitType);
			}

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



		public void LoopThroughUnitDictionary(){
			foreach (KeyValuePair<string, Square> keyValue in squareDictionary) {
				string key = keyValue.Key;
				Square value = keyValue.Value;
				if (value.squareOccupied) {
					Debug.Log ("Game.LoopThroughUnitDictionary: coords are " + key + " and unit occupying square is " + value.squareOccupied + " and nummves is " + value.unitOccupyingSquare.numMoves + "Unit occupying square is " + value.unitOccupyingSquare + " and allegiance is " + value.unitOccupyingSquare.allegiance);
				}
			} 
		}

		public void MovePiece(RUnit pieceToMove, string coordsToMoveTo){
			string originalCoords = pieceToMove.coords;
			squareDictionary [coordsToMoveTo].unitOccupyingSquare = pieceToMove;
			squareDictionary [coordsToMoveTo].squareOccupied = true;
			squareDictionary [coordsToMoveTo].unitOccupyingSquare.coords = coordsToMoveTo;
			squareDictionary [coordsToMoveTo].unitOccupyingSquare.numMoves -= 1;

			squareDictionary[originalCoords].unitOccupyingSquare = null;
			squareDictionary [originalCoords].squareOccupied = false;
			//Debug.Log ("Unit has been moved from " + selectedPieceCoords + " to " + squareClickedCoords + " and this is confirmed in the squareDictionary because the new square we clicked on has this unit on it " + squareDictionary [squareClickedCoords].unitOccupyingSquare + " and the original square is now empty (false if true) " + squareDictionary [selectedPieceCoords].squareOccupied);
		}

		public void DestroyPiece(string coordsOfPieceToDestroy){
			squareDictionary [coordsOfPieceToDestroy].squareOccupied = false;
			squareDictionary [coordsOfPieceToDestroy].unitOccupyingSquare = null;
		}

		public void MergePiece(string coordsOfOriginalPiece, string coordsOfPieceToMergeWith){
			RUnit mergePiece = squareDictionary [coordsOfPieceToMergeWith].unitOccupyingSquare;
			RUnit originalPiece = squareDictionary [coordsOfOriginalPiece].unitOccupyingSquare;
		
			if (mergePiece.numMoves > 0 && originalPiece.numMoves > 0) {
				mergePiece.strength += originalPiece.strength;
				mergePiece.numMoves -= 1;
			}
		
			DestroyPiece (coordsOfOriginalPiece);
		}

		public bool DoBattle(RUnit attacker, RUnit defender){
			
			bool AttackerWin = false;
			string attackerCoords = attacker.coords;
			string defenderCoords = defender.coords;

			//Debug.Log ("attacker strength is " + unitDictionary [attackercoords].strength + " defender strength is " + unitDictionary [defendercoorrds].strength);

			int actOfGodRandomizer = Mathf.FloorToInt (Random.Range (0f, 100f));


			if (actOfGodRandomizer != 1) {

				float attackAdvantageMultiplier = 1;
				float defenceAdvantageMultiplier = 4 / 3;
				float attackerStrength = attacker.strength;
				float defenderStrength = defender.strength;

				float attackerOdds = 0.5f * (attackerStrength / defenderStrength) * attackAdvantageMultiplier;
				float defenderOdds = 0.5f * (defenderStrength / attackerStrength) * defenceAdvantageMultiplier;

				Debug.Log ("Attacker to defender strength ratio is " + attackerStrength + " : " + defenderStrength);
				Debug.Log ("Attacker odds to defender odds is " + attackerOdds + " : " + defenderOdds);

				if (attackerOdds > defenderOdds) {
					float newFloat = defenderOdds / attackerOdds;
					int newStrength = Mathf.RoundToInt(attackerStrength * (1 - newFloat));

					Debug.Log ("attacker loss is " + (attackerStrength - newStrength));

					attacker.strength = newStrength;
					Debug.Log("attacker strength is now " + attacker.strength + " but the square dictionary strength is now" + squareDictionary[attacker.coords].unitOccupyingSquare.strength);

					DestroyPiece (defender.coords);
					Debug.Log (" is the defender destroyed?" + defender.coords);
					MovePiece (attacker, defenderCoords);

					AttackerWin = true;
				} else {
					float newFloat = attackerOdds / defenderOdds;
					int newStrength = Mathf.RoundToInt(defenderStrength * (1 - newFloat));
					Debug.Log ("defender loss is " + (defenderStrength - newStrength));

					defender.strength = newStrength;

					DestroyPiece (attacker.coords);
					Debug.Log("defender strength is now " + defender.strength + " but the square dictionary strength is now" + squareDictionary[defender.coords].unitOccupyingSquare.strength);
					AttackerWin = false;
				}

			} else {
				AttackerWin = true;
			}

			return AttackerWin;
		}

		}
	

	}
	