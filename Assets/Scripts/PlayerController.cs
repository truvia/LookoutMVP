using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using Lookout;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {
	public const string Started = "PlayerController.Start";
	public const string StartedLocal = "PlayerController.StartedLocal";
	public const string Destroyed = "PlayerController.Destroyed";
	public const string CoinToss = "PlayerController.CoinToss";
	public const string RequestMarkSquare = "PlayerController.RequestMarkSquare";
	public const string GetAllegianceOfPlayer = "PlayerController.GetAllegianceOfPlayer";

	public int score;
	//public Mark mySide;
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
//		if (isServer) {
//			mySide = Mark.CON;
//		} else if (!isServer) {
//			mySide = Mark.USA;
//		
//		}
//		EventManager.TriggerEvent (GetAllegianceOfPlayer, mySide);

	}

	void OnDestroy ()
	{
		EventManager.TriggerEvent (Destroyed);
	}

	[Command]
	public void CmdCoinToss(){
		RpcCoinToss(Random.value < 0.5f);

	}

	[ClientRpc]
	void RpcCoinToss(bool coinToss){
		EventManager.TriggerEvent (CoinToss, coinToss);
	}



	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
