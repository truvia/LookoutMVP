using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : MonoBehaviour {


	public GameObject NetworkingHUD;
	public GameObject GameplayHUD;
	public GameObject PromptUserHUD;
	public GameObject UnitHUD;
	public GameObject BasicInfoPopup;
	public GameObject preventUserInputPanel;
	public Button EndTurnButton;
	public Text controlText;
	public Text strengthText;

	public Text unitHUDStrength;
	public Text unitHUDType;
	public Text unitHUDMoves;
	public Text unitHUDDefensiveBonus;


	private RGameController gameController;
	private MyNetworkManager myNetworkManager;
	private RMouseMover mouseMover;

	private UnityAction<System.Object> didStartLocalPlayerNotificationAction;

	public Button myYesButton;
	public Button myNoButton;

	//Developer Only Code
	public const string DidRequestResetGame = "Display.DidRequestResetGame"; 
	public const string DidRequestSplitUnit = "UIController.DidREquestSplitUnit";

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

		strengthText.text = gameController.game.conArmiesStartLocations [0].ToString () + " , " + gameController.game.conArmiesStartLocations [1].ToString () + " , " + gameController.game.conArmiesStartLocations [2].ToString () + " , " + gameController.game.usArmiesStartLocations [0].ToString () + " , " + gameController.game.usArmiesStartLocations [1].ToString () + " , " + gameController.game.usArmiesStartLocations [2].ToString ();

	}

	public void ChangeTurnText(){
		gameController.RequestChangeTurn ();

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
		HideHUD (PromptUserHUD);
		gameController.MergeUnits ();
		mouseMover.trackMouseMove = true;
	}

	public void CancelInput(){
		Debug.Log ("InputCancelled cancelled");
		HideHUD (PromptUserHUD);
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
		PromptUserHUD.GetComponentInChildren<Text> ().text = question;
		mouseMover.trackMouseMove = false;
		ShowHUD (PromptUserHUD);

		myYesButton.onClick.RemoveAllListeners ();
		myNoButton.onClick.RemoveAllListeners ();

		myYesButton.onClick.AddListener (myYesButtonPressed);
		myNoButton.onClick.AddListener (myNoButtonPressed);
	}


	public void SetUnitHUDValues(RUnit unit){
		unitHUDType.text = unit.unitType.ToString ();
		unitHUDStrength.text = "Strength: " + unit.strength.ToString ();
		unitHUDMoves.text = "Moves Left: " + unit.numMoves.ToString ();
		unitHUDDefensiveBonus.text = "Defensive Bonus: " + unit.defensiveBonus.ToString ();
		
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

	public void DestroyPiece(string coords){
		
		RUnit squareUnit = gameController.game.squareDictionary [coords].unitOccupyingSquare;
		RUnit[] allunits = FindObjectsOfType<RUnit> ();
		GameObject theUnit = null;

		foreach (RUnit unit in allunits) { 
			if (unit.coords == squareUnit.coords) {
				theUnit = unit.gameObject;
			}
	
		}

		Destroy (theUnit);

	}

	public void SetBasicInfoText(string descritionToChange, string buttonTextToChange){
		Text description = BasicInfoPopup.GetComponentInChildren<Text> ();
		Text buttonText = BasicInfoPopup.GetComponentInChildren<Button> ().GetComponentInChildren<Text> ();

		description.text = descritionToChange;
		buttonText.text = buttonTextToChange;
	}

	public void ToggleShowPreventUserInputPanel(string notificationText){
		Text panelText = preventUserInputPanel.GetComponentInChildren<Text> ();
		panelText.text = notificationText;
		preventUserInputPanel.SetActive(preventUserInputPanel.activeSelf ? false : true);

	}

	public void RequestSplitUnit(){

		Debug.Log ("uiController.RequestSplitUnit");
		//unit to spliy is gameController.selectedUnit;
		RBoard board = FindObjectOfType<RBoard>();

		if (gameController.selectedUnit.strength < 1500) {
			SetBasicInfoText ("Your unit is too small to split", "okey-dokey");
			ShowHUD (BasicInfoPopup);
		} else if (gameController.selectedUnit.numMoves < 1){
			SetBasicInfoText ("Not enough moves to split", "Darn");
			ShowHUD (BasicInfoPopup);
		}else if (board.possibleMovementCoords.Count > 0) {
			string question = "Do you want to split this unit in half?";
			UnityAction yesAction = new UnityAction (() => {
				SplitAction ();
			});
			UnityAction noAction = new UnityAction (() => {
				CancelInput ();
			});


			PromptUser (question, yesAction, noAction);	
		} else {
			SetBasicInfoText ("Sorry, you don't have any space to split your army", "FINE!");
			ShowHUD (BasicInfoPopup);
		}
		
		}

	private void SplitAction(){
		HideHUD (PromptUserHUD);
		gameController.selectedGameObject.GetComponent<RUnit> ().SplitUnit ();
	}

	public void PromptUser(string question, UnityAction yesAction, UnityAction noAction){
		//mehtod to ask the user what action they want to take, using uiController; common methods include uiController.CancelInput for no; 
		WaitForUser (question, yesAction, noAction);
	}
}

