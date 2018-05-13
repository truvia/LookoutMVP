using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class RUnit : MonoBehaviour {

	public UnitType unitType;
	public int strength;
	public Mark allegiance;
	public string coords;
	public int numMoves = 0;
	private RGameController gameController;
	private RSelector selector;
	private UIController uiController;
	private RBoard board;
	public GameObject unitMount;

	
	void Start () {
		gameController = FindObjectOfType<RGameController> ();
		selector = FindObjectOfType<RSelector> ();
		uiController = FindObjectOfType<UIController> ();
		board = FindObjectOfType<RBoard> ();
	}
	


	void OnMouseDown(){
		if (allegiance == gameController.localPlayerController.myAllegiance) {
			SelectThisUnit ();
		}
	}
	void SelectThisUnit(){
		RUnit squareDictionaryRunit = gameController.game.squareDictionary [coords].unitOccupyingSquare;
		board.ClearAllSelectorSquares ();
		InstantiateUnitMount ();
		gameController.selectedUnit = squareDictionaryRunit;
		gameController.unitSelected = true;
		gameController.selectedGameObject = this.gameObject;
		uiController.SetUnitHUDValues(squareDictionaryRunit);
		uiController.ShowHUD (uiController.UnitHUD);

		if (numMoves > 0) {
			int[] intCoords =	gameController.ConvertStringToArray (squareDictionaryRunit.coords, 2);
			board.ShowPossibleSquares (intCoords, squareDictionaryRunit);

		} else {
			StartCoroutine (uiController.MakeTextFlashRed (uiController.unitHUDMoves));
			Debug.Log ("You don't have enough moves left for this piece");
		}

	}

	void InstantiateUnitMount(){
		unitMount = GameObject.FindGameObjectWithTag ("unitMount");
		if (unitMount != null) {
			Destroy (unitMount);

		}

			GameObject newMount = Instantiate (board.unitMount, this.transform.position, Quaternion.identity, this.transform);
			newMount.tag = "unitMount";

	}

//	void RequestMergePiece(){
//		if (numMoves > 0) {
//			
//		} else {
//			StartCoroutine (uiController.MakeTextFlashRed (uiController.unitHUDMoves));
//			Debug.Log ("You don't have enough moves left for this piece");
//		}
//	}
//
//	void DoBattle(){
//		if (numMoves > 0) {
//		
//		
//		} else{
//		StartCoroutine (uiController.MakeTextFlashRed (uiController.unitHUDMoves));
//		Debug.Log ("You don't have enough moves left for this piece");
//		}
//	}

}
