using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Display : MonoBehaviour {

	public const string DidRequestEndTurn = "Display.DidBeginGameNotification";
	public GameObject NetworkingHUD;
	public GameObject GameplayHUD;
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
		HideNetworkButtons ();
		ShowGameplayButtons ();
	}

	void HideNetworkButtons(){
		NetworkingHUD.SetActive (false);
	}

	void ShowNetworkButtons(object obj){
		NetworkingHUD.SetActive (true);
	}

	void ShowGameplayButtons(){
		GameplayHUD.SetActive (true);
	}


	/* Developer Code Only */
	public void RequestResetGame(){
		EventManager.TriggerEvent (DidRequestResetGame);
	}

	void GreyOutEndTurnButton(){
	}

}