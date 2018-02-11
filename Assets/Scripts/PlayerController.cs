using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Lookout;

public class PlayerController : NetworkBehaviour {
	public const string Started = "PlayerController.Start";
	public const string StartedLocal = "PlayerController.StartedLocal";
	public const string Destroyed = "PlayerController.Destroyed";
	public const string CoinToss = "PlayerController.CoinToss";
	public const string RequestMarkSquare = "PlayerController.RequestMarkSquare";

	public int score;
	public Mark mark;
	// Use this for initialization

	public override void OnStartClient ()
	{

		base.OnStartClient ();
		EventManager.TriggerEvent (Started);
	}
	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		EventManager.TriggerEvent (StartedLocal);
	}
	void OnDestroy ()
	{
		EventManager.TriggerEvent (Destroyed);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
