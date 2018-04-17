using UnityEngine;
using System.Collections;


public class RMouseMover : MonoBehaviour {

	public Color highlightColor;
	private RSelector selector;

	private Vector3 currentTileCoord;
	private RBoard board;

	// Use this for initialization
	void Start () {
		selector = FindObjectOfType<RSelector> ();
		board = FindObjectOfType<RBoard> ();
	}

	// Update is called once per frame
	void Update () {


		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;

		if (GetComponent<Collider> ().Raycast (ray, out hitInfo, Mathf.Infinity)) {
			if (!selector.gameObject.activeInHierarchy) {
				selector.gameObject.SetActive(true);
			}
			int x = Mathf.FloorToInt (hitInfo.point.x / 1);
			int z = Mathf.FloorToInt (hitInfo.point.z / 1);
			//Debug.Log ("Tile " + x + "," + z);

			currentTileCoord.x = x + 0.5f;
			currentTileCoord.z = z + 0.5f;


			selector.transform.position = currentTileCoord;

		} else {
			if (selector.gameObject.activeInHierarchy) {
				selector.ReturnPieceToOriginalPosition ();
				//selector.gameObject.SetActive (false);
				board.ClearAllSelectorSquares ();
			}
		}

	}



}
