using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Lookout;

public class ChangeUILabels : MonoBehaviour {
	public Text localPlayerServerOrClientLabel;
	public Text localPlayerSideLabel;
	private UnityAction<System.Object> getAllegianceOfPlayerAction;

	void Awake(){
		//Listeners - what methods should listen to the notification

		getAllegianceOfPlayerAction = new UnityAction<System.Object> (UpdateLocalPlayerLabel); //defines what action that this object should take when the event is triggered

	}

	void OnEnable(){
		EventManager.StartListening(PlayerController.GetAllegianceOfPlayer, getAllegianceOfPlayerAction);
	}

	void OnDisable(){
		EventManager.StopListening (PlayerController.GetAllegianceOfPlayer, getAllegianceOfPlayerAction);
	}

	void UpdateLocalPlayerLabel(object args){
		//Assumes that the Server is always Confederate and the remote client is always USA
		Mark mySide = (Mark)args;

		if (mySide == Mark.CON) {
			localPlayerServerOrClientLabel.text = "I am the server";

		} else if (mySide == Mark.USA) {
			localPlayerServerOrClientLabel.GetComponent<Text>().text = "I am the client";
		}

		localPlayerSideLabel.text = mySide.ToString();
	
	}
}
