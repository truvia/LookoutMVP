using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;

public class BattleTest : MonoBehaviour {
	

	public float attackerStrength;
	public float defenderStrength;

	public float attackerRandomizer;
	public float defenderRandomizer;

	public float attackAdvantageMultiplier = 1f;
	public float defenderAdvantageMultiplier = 4f / 3f ;

	public float defenderOdds;
	public float attackerOdds;


	public void Battle(){


		//current algorythm strongly favours defender when attacker is weaker, but doesn't sufficiently create an even battle when the defnder is outnumbered. appply different attack/defend logic based on which is stronger?

		// e.g. an attacker of strength just 500 less than the deffender will be obliterated. but a defender of the 1000 weaker than the attacker will face certain defeat. the randomizers are therefore having both a too great and too little effect. 
		// at equal strength the defender always wins. 


		int attackerWins = 0;
		int defenderWins = 0;

		for (int i = 0; i < 100; i++) {

			attackerRandomizer = Random.Range (1f, 10f);
			defenderRandomizer = Random.Range (1f, 10f);

			attackerOdds = ((0.5f * (attackerStrength / defenderStrength)) * attackAdvantageMultiplier) * attackerRandomizer;
			defenderOdds = ((0.5f * (defenderStrength / attackerStrength)) * defenderAdvantageMultiplier) * defenderRandomizer;

			print ("Attacker odds are: " + attackerOdds + " but defender odds are " + defenderOdds);
			if (attackerOdds > defenderOdds) {
				attackerWins++;
				print ("Attacker wins!");
			} else {
				defenderWins++;
				print ("Defender wins!");
			}

			float totalOddsValue = attackerOdds + defenderOdds;
			float attackerPercent = (attackerOdds / totalOddsValue);
			float defenderPercent = (defenderOdds / totalOddsValue);

//			if (attackerPercent < 0.1) {
//				attackerStrength = 0f;
//			}
//
//			if (defenderPercent < 0.1) {
//				defenderStrength = 0f;
//			}

			print ("Attacker Percentage: " + (attackerPercent * 100f) + "%; Defender Percentage: " + (defenderPercent * 100f) + "%");

			float attackerNewStrength = attackerStrength * attackerPercent;
			float defenderNewStrength = defenderStrength * defenderPercent;
			print ("Attacker new strength: " + attackerNewStrength + "; Defender new strength: " + defenderNewStrength);




		}

		float totalWins = attackerWins + defenderWins;
		print ("Attacker win percentage was: " + ((attackerWins / totalWins) * 100f) + "% (" + attackerWins + ")");
		print ("defender win percentage was: " + (defenderWins / totalWins * 100f) + "% (" + defenderWins + ")");
	}
}

