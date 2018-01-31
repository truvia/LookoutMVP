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


/// <summary>
/// what we want is essentially 3 options

	/// </summary>


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
	//	public Dictionary <string, Mark> boardDictionary;

		public Dictionary<string, Unit> unitDictionary;
		

		//public Dictionary<string, Dictionary< Mark, Unit>> unitDictionary = new Dictionary<string, Dictionary<Mark, Unit>>();

		string[] wins = new string[] {
			"0,4", "4,0"
		};

		void Start(){
		
			boardView = GameObject.FindObjectOfType<Board> ();
			boardSize = boardView.height * boardView.width;

		}

		public Game (){
//			if (boardDictionary == null) {
//				boardDictionary = new Dictionary<string, Mark> ();
//			
//			}

			if (unitDictionary == null) {
			
				unitDictionary = new Dictionary<string, Unit> ();
			}

		}

		public void Reset(){
			Debug.Log ("Reset is called");	
			//boardDictionary.Clear ();
			unitDictionary.Clear ();

			for (int z = 0; z < 5; z++) {
				for (int x = 0; x < 5; x++) {
					int[] coord = new int[]{x, z};

					string coordAsString = convertArrayToString(coord);
					//boardDictionary.Add (coordAsString, Mark.None);

					Unit newUnit = new Unit ();
					newUnit.allegiance = Mark.None;
					newUnit.unit_type = Unit.UnitType.None;

					unitDictionary.Add (coordAsString, newUnit);
									
				}

			}
		
			control = startPlayer;
			winner = Mark.None;
			InitialGameSetup ();
			EventManager.TriggerEvent (DidBeginGameNotification);

		}

		public void Place (string coords){
			
//			Mark markAtThisCoord = boardDictionary [coords];
			Mark markAtThisCoord = unitDictionary [coords].allegiance;
			if (markAtThisCoord != Mark.None) {
				return;
			
			}

			//boardDictionary [coords] = control;

			unitDictionary [coords].allegiance = control;
			EventManager.TriggerEvent (DidOccupySquareNotification, coords);

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

//						
//			Mark a = boardDictionary [wins [0]];
//			Mark b = boardDictionary [wins [1]];

			Mark a = unitDictionary[wins[0]].allegiance;
			Mark b = unitDictionary[wins[1]].allegiance;

			if (a == b && a != Mark.None) {
				winner = a;
				return true;
			}

			return false;

		}

		void InitialGameSetup(){
			

			//Deploys initial armies
			int[][] USPossibleStartSquares = new int[][]{
				new int[] {3, 0},
				new int[] {3, 1},
				new int[] {4, 1}

			};

			int[][] ConPossibleStartSquares = new int[][] {
				new int[] { 0, 3 },
				new int[] { 1, 3 },
				new int[] { 1, 4 }

			};

			int x = RandomIntCreator (0f, USPossibleStartSquares.Length);
			int y = 0;

			foreach (int[] USStartSquare in USPossibleStartSquares) {
				
				if (y != x) {

					ConstructNewUnit (USStartSquare, Mark.USA, Unit.UnitType.Army);
				}
				y++;
			}

			y = 0;
			x = Mathf.RoundToInt(Random.Range (0f, ConPossibleStartSquares.Length));

			foreach (int[] CONStartSquare in ConPossibleStartSquares) {
				if (y != x) {
					ConstructNewUnit (CONStartSquare, Mark.CON, Unit.UnitType.Army);
				}
				y++;
			}

			//FORTRESSES
			int[] USFortress = new int[] {4, 0}; 
			int[] CONFortress = new int[]{ 0, 4 };

			ConstructNewUnit (USFortress, Mark.USA, Unit.UnitType.Fortress);
			ConstructNewUnit (CONFortress, Mark.CON, Unit.UnitType.Fortress);


		}

		private void ConstructNewUnit(int[] intCoords, Mark allegiance, Unit.UnitType unitType){

			float minStrength = 0f;
			float maxStrength = 5000f;

			int strength = RandomIntCreator (minStrength, maxStrength);
			Unit newUnit = new Unit();
			newUnit.allegiance = allegiance;
			newUnit.unit_type = unitType;
			newUnit.strength = strength;
			unitDictionary [convertArrayToString (intCoords)] = newUnit;

		
		}



		public string convertArrayToString(int[] array){
			string newString = "";

			int x = 0;
			foreach (int i in array) {
				x++;	
				newString += i.ToString ();

				if (x < array.Length) {
					newString += ",";
				}


			}

			return newString;

		}

		public int[] convertStringToArray(string s, int numberOfCoordinates){
		

			string[] newStringArray = s.Split (',');

			int[] coords = new int[2]; 

			int z = 0;

			foreach (string str in newStringArray) {
				
				int x = 0;
				if (int.TryParse (str, out x)) {

					coords [z] = x;
					z++;
				}


			}

			return coords;
		}


		private int RandomIntCreator(float min, float max){
			int strength = Mathf.RoundToInt (Random.Range (min, max));
			return strength;
		}



	}
		





}
