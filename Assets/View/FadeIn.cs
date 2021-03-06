﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour {

	public float FadeInTime;
	public Color currentColor;
	
	private Image fadePanel;
	//private Color currentColor = Color.black;
	
	// Use this for initialization
	void Start () {
		fadePanel = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.timeSinceLevelLoad < FadeInTime){
		//Fade in
		float alphaChange = Time.deltaTime / FadeInTime;
		currentColor.a -= alphaChange;
		fadePanel.color = currentColor;

		}else{
		gameObject.SetActive(false);
		
		}
	}
}
