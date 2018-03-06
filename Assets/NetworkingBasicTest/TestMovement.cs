using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestMovement : NetworkBehaviour {
	public GameObject bulletPrefab;
	[Command]
	void CmdDoMFire(float lifeTime){
		GameObject bullet = (GameObject)Instantiate (bulletPrefab, transform.position, Quaternion.identity);

		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6;
		NetworkServer.Spawn (bullet);
		Destroy (bullet, lifeTime);


	}
	void Update(){
		if (!isLocalPlayer) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			CmdDoMFire (2.0f);
		}

	}
}
