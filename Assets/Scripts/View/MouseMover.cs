using UnityEngine;
using System.Collections;


public class MouseMover : MonoBehaviour {

	public Color highlightColor;
	private Selector selectionCube;

	private Vector3 currentTileCoord;
	private Board board;

	// Use this for initialization
	void Start () {
		selectionCube = FindObjectOfType<Selector> ();
		board = FindObjectOfType<Board> ();
	}

	// Update is called once per frame
	void Update () {


		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;

		if (GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {
			if (!selectionCube.gameObject.activeInHierarchy) {
				selectionCube.gameObject.SetActive(true);
			}
			int x = Mathf.FloorToInt (hitInfo.point.x / 1);
			int z = Mathf.FloorToInt (hitInfo.point.z / 1);
			//Debug.Log ("Tile " + x + "," + z);

			currentTileCoord.x = x + 0.5f;
			currentTileCoord.z = z + 0.5f;


			selectionCube.transform.position = currentTileCoord;
		} else {
			if (selectionCube.gameObject.activeInHierarchy) {

				selectionCube.ReturnPieceToOriginalPosition ();
				selectionCube.gameObject.SetActive (false);
				board.ClearAllSelectorSquares ();
			}
		}

	}



}
