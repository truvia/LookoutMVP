using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class RUnit : MonoBehaviour {

	public UnitType unitType;
	public int strength;
	public Mark allegiance;
	public string coords;
	bool startMoving = false;
	public int numMoves = 0;

	private float moveTowardsX;
	private float moveTowardsz;

	// Use this for initialization


	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {
//		if (startMoving) {
//			Vector3 newVector3 = new Vector3 (moveTowardsX, 0f, moveTowardsz);
//			transform.Translate (newVector3 * Time.deltaTime);
//
//			if(transform.position
//		}

		//input.getMOuseDown? rather than selector? 

//		if(Input.GetMouseButtonDown(0)){
//			RaycastHit hitInfo = new RaycastHit();
//
//			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)){
//					Debug.Log("clicked on " + this.coords);
//			}
//		}

	}

	public void MovePiece(int[] intCoordsToMoveTo){

		moveTowardsX = (intCoordsToMoveTo[0] + 0.5f) - transform.position.x;
		moveTowardsz = (intCoordsToMoveTo [1] + 0.5f) - transform.position.z;
		transform.Translate (moveTowardsX, 0f, moveTowardsz);
		Debug.Log (moveTowardsX + " move " + moveTowardsz);
	}
}
