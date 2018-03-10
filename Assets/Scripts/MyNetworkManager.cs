using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MyNetworkManager : NetworkManager  {

	public void MyStartHost(){
		Debug.Log ("Starting Host at " + Time.timeSinceLevelLoad);
		StartHost ();

	}

	public override void OnStartHost(){
		Debug.Log ("Host Started at " + Time.timeSinceLevelLoad);


	}


	public void MyStartClient(){
		Debug.Log ("starting Host at " + Time.timeSinceLevelLoad);
		StartClient ();

	}

	public override void OnStartClient(NetworkClient myClient){
		Debug.Log ("Client start requested at " + Time.timeSinceLevelLoad);
		InvokeRepeating ("PrintDots", 0f, 1f);

	}

	public override void OnClientConnect(NetworkConnection con){
		Debug.Log ("Client is connected to " + con.address + " at "  + Time.timeSinceLevelLoad);
		CancelInvoke ();

	}

	void PrintDots(){
		Debug.Log (".");
	
	}




}
