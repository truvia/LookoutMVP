using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using RLookout;

public class RPlayerController : NetworkBehaviour {

	public const string DidStartLocalPlayer = "RPlayerController.DidStartLocalPlayer";



	public Mark myAllegiance;
	private RGameController gameController;
	private UnityAction<System.Object> endTurnRequestNotification;

	//Developer Only Listeners
	private UnityAction<System.Object> didRequestResetGameNotificationAction;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
	}

	void Awake(){
			//Listeners - what methods should listen to the notification
		endTurnRequestNotification = new UnityAction<System.Object> (ChangeTurn); //defines what action that this object should take when the event is triggered
		didRequestResetGameNotificationAction = new UnityAction<System.Object>(RequestResetGame);
	}

	void OnEnable(){
		EventManager.StartListening(Display.DidRequestEndTurn, endTurnRequestNotification); 

		//Developer area
		EventManager.StartListening(Display.DidRequestResetGame, didRequestResetGameNotificationAction);
	}

	void OnDisable(){
		EventManager.StopListening(Display.DidRequestEndTurn, endTurnRequestNotification);	
		EventManager.StopListening (Display.DidRequestResetGame, didRequestResetGameNotificationAction);
	}
		

	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		EventManager.TriggerEvent (DidStartLocalPlayer, this);
		//Triggers GameController Trigger DestroyAllUnits
		//Triggers Display HideNetworkButtons;

		//When the client joins reset the game. May want to amend this at a future date.


		//Display
		if (isServer) {
			myAllegiance = Mark.CON; //The Host is always Confederate
		} else if (!isServer) {
			myAllegiance = Mark.USA; //The cleint is always US
			CmdDefineStartPositions();
		}

	}


	void ChangeTurn(object obj){

		if (isLocalPlayer && gameController.game.control == myAllegiance) {
		
			CmdChangeTurn ();


		}
	}

	[Command]
	void CmdChangeTurn(){
		RpcChangeTurn();
	}

	[ClientRpc]
	void RpcChangeTurn(){
		gameController.game.ChangeTurn();

	}
		

	[Command]
	void CmdDefineStartPositions(){
		bool conThreeOrTwoArmies = Random.value < 0.5;
		bool usThreeOrTwoArmies = Random.value < 0.5;

		int[] USArmiesStrength = new int[] {
			//0 means no unit instantiated
			0,
			0,
			0
		};

		int[] CONArmiesStrength = new int[] {
			//0 means no unit instantiated
			0,
			0,
			0, 
		};


		for (int i = 0; i < 3; i++) {
			if (i < 2 || conThreeOrTwoArmies) {
				CONArmiesStrength [i] = Mathf.RoundToInt (Random.Range (0f, 5000f));
			}

			if (i < 2 || usThreeOrTwoArmies) {
				USArmiesStrength [i] = Mathf.RoundToInt (Random.Range (0f, 5000f));
			}

		}

	
		//if true = three armies
		//if false = two armies

			RpcDefineStartPositions (CONArmiesStrength, USArmiesStrength);
	}


	[ClientRpc]
	void RpcDefineStartPositions(int[] CONArmiesStrength, int[] USArmiesStrength){
		gameController.DestroyAllUnits ();
		gameController.game.DefineStartPositions(CONArmiesStrength, USArmiesStrength);
		gameController.game.ResetGame ();
	}


	#region DeveloperDebugOnly
	void RequestResetGame(object obj){
		//Debug.Log ("Request Reset Game called");
		CmdDefineStartPositions ();
	}


	#endregion
	
		

}
