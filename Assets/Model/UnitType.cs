using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lookout;

public static class UnitType {

	public static readonly CON con;
	public static readonly USA usa;
	public static readonly None none;

	public  class CON{
		public static class Army{
			public const int strength = 100;	
		}

		public static class Fortress{
			public const int strength = 1000;
		}

		public static class Spy{
			public const int strength = 1;

		}
	}

	public class USA{

		public static class Army{
			public const int strength = 100;	
		}

		public static class Fortress{
			public const int strength = 1000;
		}

		public static class Spy{
			public const int strength = 1;

		}
	}

	public class None{


	}



}
