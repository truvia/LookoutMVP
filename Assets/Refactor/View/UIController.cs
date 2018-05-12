using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour {

	public const string DidRequestEndTurn = "Display.DidBeginGameNotification";
	public GameObject NetworkingHUD;
	public GameObject GameplayHUD;
	public GameObject MergeUnitHUD;
	public GameObject UnitHUD;
	public Button EndTurnButton;
	public Text controlText;
	public Text strengthText;

	public Text unitHUDStrength;
	public Text unitHUDType;
	public Text unitHUDMoves;


	private RGameController gameController;
	private MyNetworkManager myNetworkManager;
	private RMouseMover mouseMover;

	private UnityAction<System.Object> didStartLocalPlayerNotificationAction;

	public Button myYesButton;
	public Button myNoButton;

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
		mouseMover = FindObjectOfType<RMouseMover> ();
	}

	void Update(){

		if (gameController.localPlayerController && gameController.localPlayerController.myAllegiance == gameController.game.control) {
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
		Debug.Log ("Merge Requested");
		HideHUD (MergeUnitHUD);
		gameController.MergeUnits ();
		mouseMover.trackMouseMove = true;

	}

	public void CancelInput(){
		Debug.Log ("Merge cancelled");
		HideHUD (MergeUnitHUD);
		mouseMover.trackMouseMove = true;
	}

//	public void PromptUser(){
//		ShowHUD (MergeUnitHUD);
//		WaitForUser(string question, new UnityAction ( () => {
//			MergeUnits();
//		}), new UnityAction( () => {
//			CancelInput();
//		}));
//	}

	public void WaitForUser(string question, UnityAction myYesButtonPressed, UnityAction myNoButtonPressed){
		MergeUnitHUD.GetComponentInChildren<Text> ().text = question;
		mouseMover.trackMouseMove = false;
		ShowHUD (MergeUnitHUD);

		myYesButton.onClick.RemoveAllListeners ();
		myNoButton.onClick.RemoveAllListeners ();

		myYesButton.onClick.AddListener (myYesButtonPressed);
		myNoButton.onClick.AddListener (myNoButtonPressed);
	}


	public void SetUnitHUDValues(RUnit unit){
		unitHUDType.text = unit.unitType.ToString ();
		unitHUDStrength.text = "Strength: " + unit.strength.ToString ();
		unitHUDMoves.text = "Moves Left: " + unit.numMoves.ToString ();
		
	}
	//DeV options
	public void PrintUnitDictionary(){
		gameController.game.LoopThroughUnitDictionary ();
	}

	public IEnumerator MakeTextFlashRed(Text text){
		text.color = Color.red;
		yield return new WaitForSeconds (.3f);
		text.color = Color.black;
	}



}

