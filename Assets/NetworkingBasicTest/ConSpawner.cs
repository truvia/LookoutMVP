using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConSpawner : NetworkBehaviour {
	public GameObject conPrefab;
	public int numberofConArmies;
	public Transform[] spawnPoints;


	public override void OnStartServer(){
		for (int i = 0; i < numberofConArmies; i++) {
			Vector3 spawnPosition = new Vector3 (0.5f, 0.0f, i);

			GameObject newConArmy = (GameObject)Instantiate (conPrefab, spawnPoints[i].position, Quaternion.identity);
			NetworkServer.Spawn (newConArmy);
		}
		
	}
}
