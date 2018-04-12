﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RSelector : MonoBehaviour {


	public GameObject selectedPiece;
	public GameObject pieceAtThisCoord;
	//public bool anObjectSelected = false;
	private RGameController gameController;
	public GameObject originalParent;
	public Vector3 originalPosition;
	//private Board board;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
		}


	void Update(){
		if (Input.GetMouseButton(0)) {
			if (pieceAtThisCoord != null) {
				selectedPiece = pieceAtThisCoord;
				selectedPiece.transform.parent = this.transform;

			}

		}
	}



	public void ReturnPieceToOriginalPosition(){
		Debug.Log ("return to initial position called");
		if (selectedPiece) {
			selectedPiece.transform.position = originalPosition;
			selectedPiece.transform.SetParent (originalParent.transform);
			selectedPiece = null;
			originalPosition = transform.position;
			DeselectPiece ();
		}
	}


	private void DeselectPiece(){

		//anObjectSelected = false;
		originalParent = null;
	
	}

	void OnTriggerEnter(Collider collider){
		//Debug.Log ("collider is " + collider);

		if (collider.GetComponent<RUnit>() && collider.GetComponent<RUnit>().allegiance == gameController.game.control) {

			pieceAtThisCoord = collider.gameObject;
					

		}
	}

	void OnTriggerExit(Collider collider){
		//Debug.Log ("Have exited " + collider);
		if (collider.GetComponent<RUnit> ()) {
			
			pieceAtThisCoord = null;
			//originalParent = null;

		}


	}



	public void KillSelectedPiece(){
		Destroy (selectedPiece);

	}

}
