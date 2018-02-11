using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lookout;

public static class CheckPossibleSquares {


	public static List<int[]> FindArmyMoveableSquares(int[] coords, Unit unit, Dictionary<string, Unit> unitDictionary, int height, int width){
		List<int[]> moveableSquares = new List<int[]> ();
		List<int[]> battleSquares = new List<int[]>(); 

		if (unit.unit_type == Unit.UnitType.Army) {

			for(int x = -1; x <= 1; x++)	{
				for (int z = -1; z <= 1; z++) {

					Vector3 selectorCoord = new Vector3 ();

					selectorCoord.x = coords[0] + x + 0.5f;
					selectorCoord.z = coords[1] + z + 0.5f;

					int[] newCoords = new int[]{coords[0] + x, coords[1] + z };

					string coordsAsString = Game.convertArrayToString (newCoords);




					if (selectorCoord.x < 0.5f || selectorCoord.z < 0.5f || selectorCoord.x > width || selectorCoord.z > height 
						|| unitDictionary [coordsAsString].allegiance == unit.allegiance) {
						//ie. do nothing (return cancels the loop)

					} else if (unitDictionary [coordsAsString].allegiance == Mark.None) {

//						selector = greenSelectorPrefab;
//						Instantiate (selector, selectorCoord, Quaternion.identity, parent); 
						moveableSquares.Add (newCoords);

					} else {
//						selector = redSelectorPrefab;
//						Instantiate (selector, selectorCoord, Quaternion.identity, parent); 
//						possibleMovementCoords.Add (newCoords);
						battleSquares.Add (newCoords);

					}


				}


			}

		}


		foreach (int[] intArray in moveableSquares) {
			Debug.Log (intArray [0] + " , " + intArray [1]);
		}
			


		return moveableSquares;
	}



	}

