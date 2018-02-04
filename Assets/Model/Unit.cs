using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;

public class Unit: MonoBehaviour {

	public enum UnitType{
		Army,
		Fortress,
		Spy,
		None

	}

	public Mark allegiance;
	public UnitType unit_type;
	public int strength = 5000;

		


	public class Fortress:Unit{
		int strength = 5000;
		}

	public class Spy:Unit{
		int strength = 5000;

		}



}
