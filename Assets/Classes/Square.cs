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
	public bool isCitySquare = false;
	public City cityOccupyingSquare;


}
