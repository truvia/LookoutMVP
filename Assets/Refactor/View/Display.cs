using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Display : MonoBehaviour {

	public const string DidRequestEndTurn = "Display.DidBeginGameNotification";
	public GameObject NetworkingHUD;
	public GameObject GameplayHUD;
	public GameObject MergeUnitHUD;
	public Button EndTurnButton;
	public Text controlText;
	public Text strengthText;

	private RGameController gameController;
	private MyNetworkManager myNetworkManager;

	private UnityAction<System.Object> didStartLocalPlayerNotificationAction;



	//Developer Only Code
	public const string DidRequestResetGame = "Display.DidRequestResetGame"; 

	void Awake(){
		didStartLocalPlayerNotificationAction = new UnityAction<System.Object> (OnLocalPlayerStarted); //defines what action that this object should take when the event is triggered

	}

	void OnEnable(){
		EventManager.StartListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);
	}

	void OnDisable(){
		EventManager.StopListening (RPlayerController.DidStartLocalPlayer, didStartLocalPlayerNotificationAction);		
	}

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
		myNetworkManager = FindObjectOfType<MyNetworkManager> ();

	}

	void Update(){

		if (gameController.localPlayerController.myAllegiance == gameController.game.control) {
			EndTurnButton.GetComponentInChildren<Text> ().color = Color.black;
		} else {
			EndTurnButton.GetComponentInChildren<Text> ().color = Color.grey;

		}
		controlText.text = gameController.game.control.ToString();
		strengthText.text = gameController.game.conArmiesStartLocationStrengths [0].ToString () + " , " + gameController.game.conArmiesStartLocationStrengths [1].ToString () + " , " + gameController.game.conArmiesStartLocationStrengths [2].ToString () + " , " + gameController.game.uSAArmiesStartLocationStrengths [0].ToString () + " , " + gameController.game.uSAArmiesStartLocationStrengths [1].ToString () + " , " + gameController.game.uSAArmiesStartLocationStrengths [2].ToString ();

	}

	public void ChangeTurnText(){
		EventManager.TriggerEvent (DidRequestEndTurn, this.gameObject);

	}

	void OnLocalPlayerStarted(object obj){
		HideHUD (NetworkingHUD);
		ShowHUD (GameplayHUD);

	}

	void ShowNetworkButtons(object obj){
		ShowHUD (NetworkingHUD);
	}

	public void HideHUD(GameObject thisHUD){
		thisHUD.SetActive (false);
	}

	public void ShowHUD(GameObject thisHUD){
		thisHUD.SetActive (true);
	}

	/* Developer Code Only */
	public void RequestResetGame(){
		EventManager.TriggerEvent (DidRequestResetGame);
	}

	public void MergeUnits(){
		
	}

	public void CancelInput(){
		
	}

	public void TestMerge(GameObject originalPiece, RUnit mergePiece){

		ShowHUD (MergeUnitHUD);


	}
}

