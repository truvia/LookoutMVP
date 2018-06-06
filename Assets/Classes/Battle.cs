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

	public LossLevels lossLevel;
	public enum LossLevels{
		Minimal, //<0-10%
		Some, //<11-30%
		Significant, //<31-50%  
		Heavy, //<51-70%
		Pyrrhic//71-100
	}



	public void SetLossLevel(int startStrength){
		int lossesToUse;
		if (winner == Mark.CON) {
			lossesToUse = ConLosses;
		} else {
			lossesToUse = USALosses;
		}

		if (lossesToUse < (10 / 100 * startStrength)) {
			lossLevel = LossLevels.Minimal;
		} else if (lossesToUse < (30 / 100 * startStrength)) {
			lossLevel = LossLevels.Some;	
		} else if (lossesToUse < (50 / 100 * startStrength)) {
			lossLevel = LossLevels.Significant;
		} else if (lossesToUse < (70 / 100 * startStrength)) {
			lossLevel = LossLevels.Heavy;
		} else {
			lossLevel = LossLevels.Pyrrhic;
		}
	}

	public void SetWinner(Mark winningPlayer){
		winner = winningPlayer;
	}


	public void SetLosses(Mark allegiance, int losses, int startStrength){
		if (allegiance == Mark.CON) {
			ConLosses = losses;
		} else if (allegiance == Mark.USA) {
			USALosses = losses;
		}
		SetLossLevel (startStrength); 
	}

	public void SetBattleTime(){
		dateOfBattle = Time.time;
	}


	public LossLevels GetLossLevel(){
		return lossLevel;
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
