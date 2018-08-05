using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class Battle : MonoBehaviour {

	Mark winner;
	int ConLosses;
	int USALosses;
	int turn;
	float dateOfBattle;
	string battleCoords;

	public LossLevels usLossLevel;
	public LossLevels conLossLevel;

	public enum LossLevels{
		Minimal, //<0-10%
		Some, //<11-30%
		Significant, //<31-50%  
		Heavy, //<51-70%
		Pyrrhic//71-100
	}



	public void SetLossLevel(int startStrength, Mark allegiance, LossLevels thisLossLevel){
		int lossesToUse;

		if (allegiance == Mark.CON) {
			lossesToUse = ConLosses;
		} else if (allegiance == Mark.USA) {
			lossesToUse = USALosses;
		} else {
			lossesToUse = ConLosses;
			Debug.LogError ("Trying to set the losses of a a unit of no allegiance");
		}

		if (lossesToUse < (10 / 100 * startStrength)) {
			thisLossLevel = LossLevels.Minimal;
		} else if (lossesToUse < (30 / 100 * startStrength)) {
			thisLossLevel = LossLevels.Some;	
		} else if (lossesToUse < (50 / 100 * startStrength)) {
			thisLossLevel = LossLevels.Significant;
		} else if (lossesToUse < (70 / 100 * startStrength)) {
			thisLossLevel = LossLevels.Heavy;
		} else {
			thisLossLevel = LossLevels.Pyrrhic;
		}
	}

	public void SetWinner(Mark winningPlayer){
		winner = winningPlayer;
	}


	public void SetLosses(Mark allegiance, int losses, int startStrength){
		LossLevels lossLevelToUse;
		if (allegiance == Mark.CON) {
			ConLosses = losses;
			lossLevelToUse = conLossLevel;
		} else if (allegiance == Mark.USA) {
			USALosses = losses;
			lossLevelToUse = usLossLevel;
		} else {
			lossLevelToUse = conLossLevel;
			Debug.LogError ("ERROR: trying to set the losses of no allegiance");
		}
		SetLossLevel (startStrength, allegiance, lossLevelToUse);
	}

	public void SetBattleTime(){
		dateOfBattle = Time.time;
	}


	public LossLevels GetLossLevel(Mark allegiance){
		if (allegiance == Mark.CON) {
			return conLossLevel;
		} else if (allegiance == Mark.USA) {
			return usLossLevel;
		} else {
			Debug.LogError ("Error: you're trying to teh the loss level of something that has no allegiance");
			return LossLevels.Some;
		}
	}
	public void SetCoords(string coords){
		battleCoords = coords;
	}

//	public void setConLosses(int conLosses, int startStrength){
//		ConLosses = conLosses;
//		SetLossLevel (startStrength);
//	}
//
//	public void setUSLosses(int usLosses, int startStrength){
//		USL = usLosses;
//		SetLossLevel (startStrength);
//	}






}
