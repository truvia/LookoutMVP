using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityObject : City {

	public Material startMaterial;
	public Material conMaterial;
	public Material usMaterial;

	private Material currentMaterial;
	private Material materialToChangeTo;
	private RGameController gameController;
	private MeshRenderer[] meshrendersOfChildren;

	void Start(){

		meshrendersOfChildren = GetComponentsInChildren<MeshRenderer> ();
		gameController = FindObjectOfType<RGameController> ();
	}

	void Update(){
		if (occupiedBy == RLookout.Mark.None) {
			materialToChangeTo = startMaterial;
		} else if (occupiedBy == RLookout.Mark.CON) {
			
			materialToChangeTo = conMaterial;
		} else if (occupiedBy == RLookout.Mark.USA) {
			materialToChangeTo = usMaterial;
		}

		if (currentMaterial != materialToChangeTo) {
			foreach (MeshRenderer childRenderer in meshrendersOfChildren) {
				childRenderer.material = materialToChangeTo;
			}
		}
	}

//	public void SetOccupiedStatus(){
//		if (gameController.IsSquareOccupied (coords)) {
//			RUnit unitInThisCity = gameController.FindSquareDictionrayUnitByCoords (coords);
//			occupiedBy = unitInThisCity.allegiance;
//		} else {
//			occupiedBy = RLookout.Mark.None;
//		}
//
//	}
//
	//	public void BoostPerTurn(){
	//		if (gameController.IsSquareOccupied (coords)) {
	//			RUnit unitInThisCity = gameController.FindSquareDictionrayUnitByCoords (coords);
	//			unitInThisCity.strength += replenishRatePerTurn;
	//			GameObject sceneUnit = gameController.FindSceneUnitGameObjectBySquareDictionaryRUnit(unitInThisCity);
	//			gameController.SyncSceneUnitToDictionaryUnit (unitInThisCity, sceneUnit);
	//		}
	//	}



}
