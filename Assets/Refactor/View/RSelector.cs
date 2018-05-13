using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RSelector : MonoBehaviour {


	public GameObject selectedPiece;
	//public GameObject pieceAtThisCoord;
	//public bool anObjectSelected = false;
	private RGameController gameController;
	public GameObject originalParent;
	public Vector3 originalPosition;
	public GameObject unitMount;

	void Start(){
		gameController = FindObjectOfType<RGameController> ();
		}


	public void ReturnPieceToOriginalPosition(){
	//	Debug.Log ("return to initial position called");
		if (selectedPiece) {
			selectedPiece.transform.position = originalPosition;
			selectedPiece.transform.SetParent (originalParent.transform);
			DeselectPiece ();
		}
	}


	void DeselectPiece(){
		unitMount.SetActive (false);
		selectedPiece = null;
		originalPosition = transform.position;
		originalParent = null;

	
	}

//	void OnTriggerEnter(Collider collider){
//		//Debug.Log ("collider is " + collider);
//
//		if (collider.GetComponent<RUnit>() && collider.GetComponent<RUnit>().allegiance == gameController.game.control) {
//
//			pieceAtThisCoord = collider.gameObject;
//		}
//	}
//
//	void OnTriggerExit(Collider collider){
//		//Debug.Log ("Have exited " + collider);
//		if (collider.GetComponent<RUnit> ()) {
//			
//			pieceAtThisCoord = null;
//		}
//
//
//	}

	public void SelectPiece(GameObject pieceToSelect){
		originalParent = pieceToSelect.transform.parent.gameObject;
		originalPosition = pieceToSelect.transform.position;
		selectedPiece = pieceToSelect;
		selectedPiece.transform.parent = this.transform;
		unitMount.SetActive (true);
		unitMount.transform.position = pieceToSelect.transform.position;

		unitMount.transform.SetParent(selectedPiece.transform);
	}

	public void PlacePiece(){
		if (selectedPiece) {
			selectedPiece.transform.SetParent (originalParent.transform);
			DeselectPiece ();
		}
	}
		

	public void KillSelectedPiece(){
		Destroy (selectedPiece);

	}

}
