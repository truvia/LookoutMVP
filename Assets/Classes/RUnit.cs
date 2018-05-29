using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class RUnit : MonoBehaviour {

	//key parameters for the unit
	public UnitType unitType;
	public int strength;
	public Mark allegiance;
	public string coords;
	public int numMoves = 0;
	public int defensiveBonus;


	public void AddDefensiveBonus(int bonusAmount){
		defensiveBonus += bonusAmount;
	}

	public void RemoveDefensiveBonus(){
		defensiveBonus = 0;
	}









}
