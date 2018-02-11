using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
using Lookout;

[TestFixture]
public class CheckPossibleSquaresTest {
	

	//CHECK THAT THE TEST IS GENEALLY WORKING
	[Test]
	public void T000PassingTest(){
		Assert.AreEqual(1,1);

	}

	[Test]
	public void T001ArmyInMiddleNoOccupiedSquaresReturns8Squares(){
		List<int[]> possibleSquares = new List<int[]> {
			new int[]{1,0},
			new int[]{1,1},
			new int[]{1,2},
			new int[]{2,0},
			new int[]{2,1},
			new int[]{2,2},
			new int[]{3,0},
			new int[]{3,1},
			new int[]{3,2}

		};



		int[] currentCoords = { 2, 2 };
		Army newArmy = new Army ();
		Dictionary<string, Unit> unitDictionary = new Dictionary<string, Unit> ();

		for (int z = 0; z < 5; z++) {
			for (int x = 0; x < 5; x++) {
				int[] coord = new int[]{ x, z };

				string coordAsString = Game.convertArrayToString (coord);

				Unit newUnit = new Unit ();
				newUnit.allegiance = Mark.None;
				newUnit.unit_type = Unit.UnitType.None;

				unitDictionary.Add (coordAsString, newUnit);

			}
		}



		int height = 5;
		int width = 5;



		Assert.AreEqual (possibleSquares, CheckPossibleSquares.FindArmyMoveableSquares(currentCoords, newArmy, unitDictionary, height, width));


	}






}