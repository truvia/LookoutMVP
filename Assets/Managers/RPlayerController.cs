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
	private UnityAction<System.Object> didEndGameNotificationAction;
	private EnqueueSystem enqueueSystem;

	//Developer Only Listeners
	private UnityAction<System.Object> didRequestResetGameNotificationAction;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
		enqueueSystem = FindObjectOfType<EnqueueSystem> ();
	}

	void Awake(){
			//Listeners - what methods should listen to the notification
		endTurnRequestNotification = new UnityAction<System.Object> (ChangeTurn); //defines what action that this object should take when the event is triggered
		didRequestResetGameNotificationAction = new UnityAction<System.Object>(RequestResetGame);
		didEndGameNotificationAction = new UnityAction<System.Object> (EndGame);
	}

	void OnEnable(){
		EventManager.StartListening(RGameController.DidRequestEndTurn, endTurnRequestNotification); 
		EventManager.StartListening (RGame.DidEndGameNotification, didEndGameNotificationAction);
		//Developer area
		EventManager.StartListening(UIController.DidRequestResetGame, didRequestResetGameNotificationAction);
	}

	void OnDisable(){
		EventManager.StopListening(RGameController.DidRequestEndTurn, endTurnRequestNotification);	
		EventManager.StopListening (RGame.DidEndGameNotification, didEndGameNotificationAction);
		EventManager.StopListening (UIController.DidRequestResetGame, didRequestResetGameNotificationAction);
	}
		

	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		EventManager.TriggerEvent (DidStartLocalPlayer, this);
		//Triggers GameController Trigger DestroyAllUnits
		//Triggers Display HideNetworkButtons;

		//When the client joins reset the game. May want to amend this at a future date.

		if (isServer) {
			myAllegiance = Mark.CON; //The Host is always Confederate
		} else if (!isServer) {
			myAllegiance = Mark.USA; //The client is always US
			CmdDefineStartPositions();
		}

	}


	void ChangeTurn(object obj){

		if (isLocalPlayer && gameController.game.control == myAllegiance) { //only runs from whoever has control

			//sends the movements made by this player to the other player's machine
			foreach (KeyValuePair<string, int[]> keyValue in enqueueSystem.enquedPieceMovement) {
				string originalPieceCoords = keyValue.Key;
				int[] MoveToCoords = keyValue.Value;
				CmdBroadcastEnqueuedMovements (originalPieceCoords, MoveToCoords);
				//Debug.Log ("sending across original coords: " + originalPieceCoords + " and moveTo coords " + MoveToCoords [0] + " , " + MoveToCoords [1]);
			}
			enqueueSystem.enquedPieceMovement.Clear (); //empty the list of movements made by the player who just pushed end turn's on that machine (should this go in gameController?);

			CmdChangeTurn ();

			//send across the squareDictionary, which has details of what squares have what pieces in the game, to the next player
			foreach (KeyValuePair<string, Square> keyValue in gameController.game.squareDictionary) {
				string key = keyValue.Key;
				Square value = keyValue.Value;

				bool squareOccupied = value.squareOccupied;
				Mark allegiance;
				string coords;
				int strength;
				UnitType unitType;
				int numMoves;
				int defensiveBonus;

				if (squareOccupied) {
					 allegiance = value.unitOccupyingSquare.allegiance;
					 coords = value.unitOccupyingSquare.coords;
					 strength = value.unitOccupyingSquare.strength;
					 unitType = value.unitOccupyingSquare.unitType;
					defensiveBonus = value.unitOccupyingSquare.defensiveBonus;

					if (value.unitOccupyingSquare.unitType == UnitType.Army) {
						numMoves = 1;
					} else if (value.unitOccupyingSquare.unitType == UnitType.Spy) {
						numMoves = 2;
					} else {
						numMoves = 0;
					}
				} else {
					 allegiance = Mark.None;
					 coords = key;
					 strength = 0;
					 unitType = UnitType.None;
					numMoves = 0;
					defensiveBonus = 0;
				}

				CmdBroadcastSquareDictionary (key, squareOccupied, allegiance, coords, strength, unitType, numMoves, defensiveBonus);
				//Debug.Log ("Game.LoopThroughUnitDictionary: coords are " + key + " and unit occupying square is " + value.squareOccupied);
			} 
		}
	}

	void EndGame(object args){
		Mark winner = (Mark)args;

		CmdEndGame (winner);
	}

	[Command]
	void CmdEndGame(Mark winner){
		RpcEndGame (winner);
	}

	[ClientRpc]
	void RpcEndGame(Mark winner){
		gameController.EndGame (winner);
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
	void CmdBroadcastSquareDictionary(string key, bool squareOccupied, Mark allegiance, string coords, int strength, UnitType unitType, int numMoves, int defensiveBonus){
		RpcBroadcastSquareDictionary (key, squareOccupied, allegiance, coords, strength, unitType, numMoves, defensiveBonus);
	}

	[ClientRpc]
	void RpcBroadcastSquareDictionary (string key, bool squareOccupied, Mark allegiance, string coords, int strength, UnitType unitType, int numMoves, int defensiveBonus){

		//gameController.game.squareDictionary [key].squareOccupied = squareOccupied;

		if (squareOccupied) {
			gameController.game.squareDictionary [key].squareOccupied = true;
			gameController.game.squareDictionary [key].unitOccupyingSquare = new RUnit (); 
			gameController.game.squareDictionary [key].unitOccupyingSquare.allegiance = allegiance;
			gameController.game.squareDictionary [key].unitOccupyingSquare.coords = coords;
			gameController.game.squareDictionary [key].unitOccupyingSquare.strength = strength;
			gameController.game.squareDictionary [key].unitOccupyingSquare.unitType = unitType;
			gameController.game.squareDictionary [key].unitOccupyingSquare.numMoves = numMoves;
			gameController.game.squareDictionary [key].unitOccupyingSquare.defensiveBonus = defensiveBonus;
		
			if (gameController.game.squareDictionary [key].isCitySquare) {
				gameController.game.squareDictionary [key].cityOccupyingSquare.occupiedBy = allegiance;
			}
		
		} else if(gameController.game.squareDictionary[key].squareOccupied) {
			
			gameController.game.squareDictionary [key].squareOccupied = false;
			gameController.game.squareDictionary [key].unitOccupyingSquare.allegiance = allegiance;
			gameController.game.squareDictionary [key].unitOccupyingSquare.coords = coords;
			gameController.game.squareDictionary [key].unitOccupyingSquare.strength = strength;
			gameController.game.squareDictionary [key].unitOccupyingSquare.unitType = unitType;
			gameController.game.squareDictionary [key].unitOccupyingSquare.numMoves = numMoves;

		}
			

		//gameController.RefreshBoard (null);
	}

	[Command]
		void CmdDefineStartPositions(){

		int[] conStarterSquares = gameController.DefineStartPositions ();
		int[] usStarterSquares = gameController.DefineStartPositions ();

		RpcDefineStartPositions (conStarterSquares, usStarterSquares);
//
//		for (int i = 0; i < 3; i++) {
//			if (i < 2 || conThreeOrTwoArmies) {
//				CONArmiesStrength [i] = Mathf.RoundToInt (Random.Range (0f, 5000f));
//			}
//
//			if (i < 2 || usThreeOrTwoArmies) {
//				USArmiesStrength [i] = Mathf.RoundToInt (Random.Range (0f, 5000f));
//			}

	//	}

	
		//if true = three armies
		//if false = two armies

	//		RpcDefineStartPositions (CONArmiesStrength, USArmiesStrength);
	}


	[ClientRpc]
	void RpcDefineStartPositions(int[] CONArmiesStrength, int[] USArmiesStrength){
		gameController.DestroyAllUnitsInScene ();
		gameController.game.SetStartPositions(CONArmiesStrength, USArmiesStrength);
		gameController.game.ResetGame ();
	}
//	[ClientRpc]
//	void RpcDefineStartPositions(ArrayList conArmiesStrength, ArrayList usArmiesStrength){
//		
//	}

	[Command]
	void CmdBroadcastEnqueuedMovements(string originalPiece, int[] coordsToMoveTo){
		RpcBroadcastEnqueuedMovements(originalPiece, coordsToMoveTo);
	}

	[ClientRpc]
	void RpcBroadcastEnqueuedMovements(string originalPiece, int[] coordsToMoveTo){
		gameController.AddEnqueuedItem (originalPiece, coordsToMoveTo);
	}



	#region DeveloperDebugOnly
	void RequestResetGame(object obj){

		//Debug.Log ("Request Reset Game called");
		CmdDefineStartPositions ();
	}


	#endregion
	
		

}
