using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RLookout;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TesterClick : MonoBehaviour, IPointerClickHandler {

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData){
		if (eventData.button == PointerEventData.InputButton.Right) {
			
			Vector3 pos = eventData.pointerCurrentRaycast.worldPosition;
			int x = Mathf.FloorToInt (pos.x);
			int z = Mathf.FloorToInt (pos.z);

			Debug.Log ("clicked : " + x + " , " + z);
		}
	}
}
