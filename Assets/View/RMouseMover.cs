using UnityEngine;
using System.Collections;


public class RMouseMover : MonoBehaviour {

	//public Color highlightColor;
	public bool trackMouseMove = true;
	//private RSelector selector;

	//private Vector3 currentTileCoord;
	//private RBoard board;
	//private RGameController gameController;

	public GameObject pieceToDrag;

	// Use this for initialization
	void Start () {
	//	selector = FindObjectOfType<RSelector> ();
	//	board = FindObjectOfType<RBoard> ();
	//	gameController = FindObjectOfType<RGameController> ();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButton (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hitInfo;

			if (trackMouseMove) {
				if (GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {
//				if (!selector.gameObject.activeInHierarchy) {
//					selector.gameObject.SetActive (true);
//				}

					pieceToDrag.transform.position = new Vector3 (hitInfo.point.x, 0.5f, hitInfo.point.z);

					int x = Mathf.FloorToInt (hitInfo.point.x / 1);
					int z = Mathf.FloorToInt (hitInfo.point.z / 1);
					//Debug.Log ("Tile " + x + "," + z);
//
//				currentTileCoord.x = x + 0.5f;
//				currentTileCoord.z = z + 0.5f;


					//selector.transform.position = currentTileCoord;

				} else {
//				if (selector.gameObject.activeInHierarchy) {
//					selector.ReturnPieceToOriginalPosition ();
//					gameController.selectedUnit = null;
//					gameController.unitSelected = false;
//					//selector.gameObject.SetActive (false);
//					board.ClearAllSelectorSquares ();
//				}
				}
			}
		}
	}


}
