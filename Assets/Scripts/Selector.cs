using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Selector : MonoBehaviour {

	public GameObject hoverpiece;
	public GameObject selectedPiece;
	private bool anObjectSelected = false;
	private GameController gameController;
	private GameObject originalParent;
	private Vector3 originalPosition;
	private Board board;

	void Start(){
		gameController = FindObjectOfType<GameController> ();
		board = FindObjectOfType<Board> ();
	}

	// Use this for initialization
	void OnMouseDown(){
		//if no piece is selected
		if (!anObjectSelected) {

			//pick up the piece currently being hovered over (as long as it is your piece)
			if (!anObjectSelected && gameController.game.control == Lookout.Mark.CON && hoverpiece.GetComponent<Unit> ().allegiance == Lookout.Mark.CON || !anObjectSelected && gameController.game.control == Lookout.Mark.USA && hoverpiece.GetComponent<Unit> ().allegiance == Lookout.Mark.USA) {

				selectedPiece = hoverpiece;
				selectedPiece.transform.SetParent (transform);
				transform.rotation = Quaternion.identity;
				anObjectSelected = true;

			} 
		}
			
		else if (anObjectSelected) {
			int[] coords = new int[]{
				Mathf.FloorToInt(this.transform.position.x),
				Mathf.FloorToInt(this.transform.position.z)
			};

			if (!board.possibleMovementCoords.Any(p => p.SequenceEqual(coords))) {
				board.ClearAllSelectorSquares ();
				ReturnPieceToOriginalPosition ();
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

		anObjectSelected = false;
		originalParent = null;

	}

	void OnTriggerEnter(Collider collider){
		Debug.Log ("on trigger enter");
		if (collider.GetComponent<Unit> ()) {

			if (!anObjectSelected) {
				originalParent = collider.gameObject.transform.parent.gameObject;
				originalPosition = collider.gameObject.transform.position;
				hoverpiece = collider.gameObject;
			}

		}
	}

	void OnTriggerExit(Collider collider){
		if (collider.GetComponent<Unit> () && !anObjectSelected) {
			print ("called");
			hoverpiece = null;
			originalParent = null;
			originalPosition = transform.position;

		}
	}


	public void PlacePiece(){

		Vector3 newVector3 = new Vector3 (this.transform.position.x, 0.05f, this.transform.position.z);
		selectedPiece.transform.SetParent (originalParent.transform);
		selectedPiece.transform.position = newVector3;

		selectedPiece = null;
		DeselectPiece ();
	}

}
