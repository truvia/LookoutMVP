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

	enum LossLevels{
		Minimal, //<0-10%
		Some, //<11-30%
		Significant, //<31-50%  
		Heavy, //<51-70%
		Pyrrhic//71-100
	}

	public class RGame {
		//messages?
		public const string DidBeginGameNotification = "Game.DidBeginGameNotification";
		public const string DidChangeTurnNotification = "Game.DidChangeTurnNotification";
		public const string DidEndGameNotification = "Game.DidEndGameNotification";
		//public const string BeginGameSetup = "Game.BeginGameSetup";

		//Actions

		public int[] conArmiesStartLocations = new int[3];
		public int[] usArmiesStartLocations = new int[3];
		public List<Battle> battleHistory = new List<Battle> ();

		public Mark control { get; private set;}
		public Mark winner {get; private set;}
		public Mark startPlayer = Mark.CON; //sets start player as the Confederates
		public Dictionary<string, Square> squareDictionary = new Dictionary<string, Square>();
		public int boardWidth = 8; //N.B. remember to change the size of the box collider on the board gameobject as well as changing the win conditions immediately below, if the fotress is moved
		public int boardHeight = 8;

		private string[] wins = new string[] {
			"1 , 6", "6 , 1"
		};


		//only the host can call a clientRPc 
		public void ChangeTurn(){
			Debug.Log ("ChangeTurn called by " + control);
			control = (control == Mark.CON) ? Mark.USA : Mark.CON;

			EventManager.TriggerEvent(DidChangeTurnNotification);
		}


		public void CheckForGameOver(){
			if (CheckForWin ()) {
				Debug.Log ("Check for win is true");

				EventManager.TriggerEvent (DidEndGameNotification, control);
				control = Mark.None;
			} else {
				Debug.Log ("check for win is false");
			}

			if (CheckForStaleMate()) {
				Debug.Log ("Stalemate is true!");
				control = Mark.None;
				EventManager.TriggerEvent (DidEndGameNotification, control);
			}
			
		}
			
		bool CheckForWin(){
			for(int i = 0; i < wins.Length; i++) {
				Square square = squareDictionary [wins[i]];
				if (square.squareOccupied) {
					if (square.unitOccupyingSquare.unitType != UnitType.Fortress && square.unitOccupyingSquare.allegiance == control) {
						return true;
					}
				}
			}
			return false;
		}

		bool CheckForStaleMate(){
			foreach (KeyValuePair<string, Square> keyValue in squareDictionary) {
				string key = keyValue.Key;
				Square value = keyValue.Value;
				if (value.squareOccupied) {
					if (value.unitOccupyingSquare.unitType == UnitType.Army || value.unitOccupyingSquare.unitType == UnitType.Spy) {
						return false;
					}
				}

			}
			return true;
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
				"5 , 0",
				"6 , 0",
				"7 , 0",
				"5 , 1",
				"7 , 1",
				"5 , 2",
				"6 , 2",
				"7 , 2"
			};

			string[] ConPossibleStartSquares = new string[] {
				"0 , 5",
				"0 , 6",
				"0 , 7", 
				"1 , 5",
				"1 , 7",
				"2 , 5", 
				"2 , 6",
				"2 , 7"
			};


			foreach (int squarenumber in conArmiesStartLocations) {
				ConstructNewUnit (USPossibleStartSquares [squarenumber], Mark.USA, UnitType.Army, 5000);
			}

			foreach (int squareNumber in usArmiesStartLocations) {
				ConstructNewUnit(ConPossibleStartSquares[squareNumber], Mark.CON, UnitType.Army, 5000);
			}

			//FORTRESSES
			string USFortress = "6 , 1"; 
			string CONFortress = "1 , 6";

			ConstructNewUnit (USFortress, Mark.USA, UnitType.Fortress, 8000);
			ConstructNewUnit (CONFortress, Mark.CON, UnitType.Fortress, 8000);

			//Cities
			string cityACoords = "1 , 2";
			string cityBCoords = "6 , 5";

			ConstructNewCity (cityACoords, Mark.None);
			ConstructNewCity (cityBCoords, Mark.None);

		
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

		private void ConstructNewCity(string coords, Mark allegiance){
			City newCity = new City ();
			newCity.coords = coords;
			newCity.occupiedBy = allegiance;
			squareDictionary [coords].isCitySquare = true;
			squareDictionary [coords].cityOccupyingSquare = newCity;
		}

		public void SetStartPositions(int[] CONArmiesStartPositions, int[] USAArmiesStartPositions){
			conArmiesStartLocations = CONArmiesStartPositions;
			usArmiesStartLocations = USAArmiesStartPositions;
			//called from player controller right now....
		}

		public List<RUnit> FindUnitsByAllegiance(Mark thisAllegiance){
			List<RUnit> unitsOfThisAllegiance = new List<RUnit> ();
			foreach (KeyValuePair<string, Square> keyValue in squareDictionary) {
				string coords = keyValue.Key;
				Square square = keyValue.Value;
				if (square.squareOccupied) {
					if(square.unitOccupyingSquare.allegiance == thisAllegiance){
						unitsOfThisAllegiance.Add (square.unitOccupyingSquare);
					}
				}
			}
			return unitsOfThisAllegiance;
		}



		public void LoopThroughUnitDictionary(){
			foreach (KeyValuePair<string, Square> keyValue in squareDictionary) {
				string key = keyValue.Key;
				Square value = keyValue.Value;
				if (value.squareOccupied) {
					Debug.Log ("Game.LoopThroughUnitDictionary: coords are:" + key + "; allegiance is " + value.unitOccupyingSquare.allegiance + "; strength is: " + value.unitOccupyingSquare.strength + "; and unit occupying square is " + value.squareOccupied + "; and nummoves is: " + value.unitOccupyingSquare.numMoves + "; and Unit occupying square is " + value.unitOccupyingSquare + ";");
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

		public Mark DoBattle(RUnit attacker, RUnit defender){ 
			//Definitions
			Mark battleWinner = Mark.None; //overall winner (can be stalemate)
//			Mark winner = Mark.None; //nominal winner (i.e. they lost fewer troops)
//			Mark loser = Mark.None; //nominalloser

			string attackerCoords = attacker.coords;
			string defenderCoords = defender.coords;

			Battle newBattle = new Battle ();

			//Set relative strengths of attacker and defender. Defenders might have an additional defensive bonus in place (e.g. if in a city).
			int attackerStrength = attacker.strength;
			int defenderStrength = defender.strength + defender.defensiveBonus;

			Debug.Log ("attacker strength is " + attackerStrength + " and defender strength is " + defenderStrength);
			//Set the advantage multipliers. By default attacker is set to 1 and defender is set to 1.333 to simulate defensive bonus
			// the attackers multiplier could be changed by other factors (e.g. leadership etc) in future.
			float attackerAdvantageMultiplier = 1f;
			float defenderAdvantageMultiplier = 4f / 3f;
			Debug.Log ("attacker advantage  is " + attackerAdvantageMultiplier + " and defender advantage is " + defenderAdvantageMultiplier);

			// randomizers give a random advantage to either defender or attacker and are separated in case we wish to further give more advantages
			float attackerRandomizer = Random.Range (0.1f, 10f);
			float defenderRandomizer = Random.Range (0.1f, 10f);
			Debug.Log ("attacker randomizer is " + attackerRandomizer + " and defender randomizer is " + defenderRandomizer);


			//Odds are the relative proporiton of strength once al the above variables are added in. 
			// For now it is based aroudn the attacker strength divided by defender strength times by multiplier and randomizer. 
			float attackerOdds = ((0.5f * (attackerStrength / defenderStrength)) * attackerAdvantageMultiplier) * attackerRandomizer;
			float defenderOdds = ((0.5f * (defenderStrength / attackerStrength)) * defenderAdvantageMultiplier) * defenderRandomizer;

			Debug.Log ("Attacker odds are: " + attackerOdds + " but defender odds are " + defenderOdds);

			//defines the nominal winner (i.e. who technically lost fewer percent of troops) - N.B. not the Battle winner (as an indecisive victory has the same technical effect as a stalemate). 
//			if (attackerOdds > defenderOdds) { 
//				//Then it is an attacker win;
//				winner = attacker.allegiance;
//				loser = defender.allegiance;
//
//			} else {
//				//Then it is a defefnder win;
//				winner = defender.allegiance;
//				loser = attacker.allegiance;
//			}

			//To determine the proportion of losses in line with caclulated odds, identify what prportion of the total both are as a percentage
			//N.B. may want to throw a further randomizer in there so it is not always in total proprtion ( to allow for pyhrric victories etc).
			float totalOddsValue = attackerOdds + defenderOdds;
			float attackerPercent = (attackerOdds / totalOddsValue);
			float defenderPercent = (defenderOdds / totalOddsValue);


			//if the attacker has lost 90% of it is troops, destroy the unit.
			if (attackerPercent < 0.1) {
				DestroyPiece (attacker.coords);
				battleWinner = defender.allegiance;
			
				//if the defender has lost 90% of its troops, destroy the unit and declare attacker the battle winner
			} else if (defenderPercent < 0.1) {
				battleWinner = defender.allegiance;
				DestroyPiece (defender.coords);
				MovePiece (attacker, defender.coords);

			} else {
			//it was really a stalemate rather than an outright win, though there is a nominal winner.
				battleWinner = Mark.None;
				attacker.strength = Mathf.RoundToInt(attackerStrength * attackerPercent);

				defender.strength = Mathf.RoundToInt(defenderStrength * defenderPercent); 
			}			

			//create a "Battle" instance, and set relative loss levles for each player. 
			// add the battle to a list of battles (BattleHistory) so that we can access this info whenever we want. 
			newBattle.SetWinner (battleWinner);
			newBattle.SetBattleTime ();
			newBattle.SetLosses (attacker.allegiance, Mathf.RoundToInt((1-attackerPercent)*attackerStrength), attacker.strength);
			newBattle.SetLosses (defender.allegiance, Mathf.RoundToInt((1-defenderPercent)*defenderStrength), defender.strength);

			battleHistory.Add (newBattle);

			return battleWinner;
		}

//		private void CalculateDecisiveBattle(float winnerOdds, float loserOdds, RUnit winner, RUnit loser){
//			float battleResult = winnerOdds / loserOdds; 
//			int newStrength = Mathf.RoundToInt (winner.strength * (1 - battleResult));
//			int winnerLoss = winner.strength - newStrength;
//			Debug.Log ("winner loss is " + winnerLoss);
//
//			Battle newBattle = new Battle ();
//			newBattle.SetWinner (winner.allegiance);
//			newBattle.SetBattleTime ();
//			newBattle.SetLosses (winner.allegiance, winnerLoss, winner.strength);
//			newBattle.SetLosses (loser.allegiance, loser.strength, loser.strength);
//
//			winner.strength = newStrength;
//			DestroyPiece (loser.coords);
//
//			if (winner.allegiance == control) {
//
//				newBattle.SetCoords (loser.coords);
//
//				//If the winner is the person currently taking their turn, the winner must be an attacker and should therefore moved. Otherwise it is a defender win and the piece should just die.
//				MovePiece (winner, loser.coords);
//			}
//			battleHistory.Add (newBattle);
//
//		}

//		private void CalculateIndecisiveBattle(float attackerOdds, float defenderOdds, RUnit attacker, RUnit defender){
//			float attackerStrengthHit = attackerOdds / defenderOdds;
//			float defenderStrengthHit = defenderOdds / attackerOdds;
//
//
//
//			int newAttackerStrength = Mathf.RoundToInt (attacker.strength * (1 - attackerStrengthHit));
//			int newDefenderStrength = Mathf.RoundToInt (defender.strength * (1 - defenderStrengthHit));
//
//
//			Debug.Log ("Indecisive Battle. Attacker to defender odds ratio was " + attackerOdds + " : " + defenderOdds);
//			Debug.Log ("Attacker loss was: " + (attacker.strength - newAttackerStrength));
//			Debug.Log ("Defender Loss was : " + (defender.strength - newDefenderStrength));
//
//			Battle newBattle = new Battle ();
//			newBattle.SetWinner (Mark.None);
//			newBattle.SetBattleTime ();
//			newBattle.SetLosses (attacker.allegiance, Mathf.RoundToInt (attacker.strength - newAttackerStrength), attacker.strength);
//			newBattle.SetLosses (defender.allegiance, Mathf.RoundToInt (defender.strength - newDefenderStrength), defender.strength);
//			battleHistory.Add (newBattle);
//
//				
//			attacker.strength = newAttackerStrength;
//			defender.strength = newDefenderStrength;	
//			attacker.numMoves -= 1;
//			defender.numMoves -= 1;
//
//		}


		//DEBUG AREA


		}
	

	}
	