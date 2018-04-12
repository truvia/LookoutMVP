using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class Square : MonoBehaviour {
	
	public enum TerrainType
	{
		Tundra,
		Desert,
		River,
		Mountain,
		Grassland
	
	}

	public RUnit unitOccupyingSquare;

	public bool squareOccupied = false;

//	public Mark allegiance;
//	public UnitType unit_type;
//	public int strength = 5000;
//
//
//	public class Fortress:Unit{
//		int strength = 5000;
//	}
//
//	public class Spy:Unit{
//		int strength = 5000;
//
//	}
}
