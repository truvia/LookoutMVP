using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnqueueSystem : MonoBehaviour {




	public Dictionary<string, int[]> enquedPieceMovement = new Dictionary<string, int[]>();
	public bool preventInput = false;

	private RGameController gameController;
	private UIController uiController;


	// Use this for initialization
	void Start () {
		gameController = FindObjectOfType<RGameController> ();	
		uiController = FindObjectOfType<UIController> ();
	}
	
	void EnqueuePieceMovement(string originalCoords, int[] moveToCoords){
		enquedPieceMovement.Add (originalCoords, moveToCoords);
	}



	IEnumerator WaitAndMove(float waitTime, GameObject unitGameObject, int[] intCoords){
		yield return new WaitForSeconds (waitTime);
		unitGameObject.GetComponent<UnitObject> ().MoveTowardsAPlace (intCoords);
		//print("WaitAndMOve " + Time.time + " unit coords are " + unitGameObject.GetComponent<RUnit>().coords);
		//enquedPieceMovement.Remove (unit.GetComponent<RUnit> ().coords);
	}

	public void CallDequeuePieces(){
		preventInput = true;
		uiController.ToggleShowPreventUserInputPanel ("Showing Enemy Movements...");
		StartCoroutine(DequeuePieces ());

	}

	IEnumerator DequeuePieces(){
		foreach (KeyValuePair<string, int[]> keyValue in enquedPieceMovement) {
			
			string originalCoords = keyValue.Key;
			int[] coordsToMoveTo = keyValue.Value;
			Debug.Log ("dequeue pieces" + "original coords: " + originalCoords + " , coordsToMoveTo: " + coordsToMoveTo[0] + " , " + coordsToMoveTo[1]);
			RUnit unit = gameController.game.squareDictionary [originalCoords].unitOccupyingSquare;
			GameObject gameObjectOfUnit = gameController.FindSceneUnitGameObjectBySquareDictionaryRUnit (unit);
			yield return StartCoroutine (WaitAndMove (1.0f, gameObjectOfUnit, coordsToMoveTo));
		}
		ClearEnqueuedItems ();
		yield return StartCoroutine (WaitAndRefreshBoard (1.0f));
		uiController.ToggleShowPreventUserInputPanel (""); //Should toggle off
	}


	public void ClearEnqueuedItems(){
		enquedPieceMovement.Clear ();
	}

	IEnumerator WaitAndRefreshBoard(float waitfortime){
		yield return new WaitForSeconds (waitfortime);
		gameController.RefreshBoard (null);
		preventInput = false;
	}
}
