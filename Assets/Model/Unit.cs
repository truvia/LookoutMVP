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

		
		public class Army2{
			


			public int strength = 100;	
		}

		public class Fortress{
			public const int strength = 1000;
		}

		public class Spy{
			public const int strength = 1;

		}



}
