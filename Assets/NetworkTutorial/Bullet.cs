using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour {

	void OnCollisionEnter(Collision collision){
		GameObject thingHit = collision.gameObject;
		Health health = thingHit.GetComponent<Health> ();

		if (health != null) {
			health.TakeDamage (10);
		}
		Destroy (gameObject);
	}
}
