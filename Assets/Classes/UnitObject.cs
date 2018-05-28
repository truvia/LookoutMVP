using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitObject : MonoBehaviour {


	private RGameController gameController;
	private UIController uiController;
	private RBoard board;

	//parameters used solely for game View (e.g. moving and selecting the piece)
	private float[] moveToCoords = new float[2];
	public bool startMoving = false;
	public float speed = 3f;
	public GameObject unitMount;

	// Use this for initialization
	void Start () {
		gameController = FindObjectOfType<RGameController> ();
		uiController = FindObjectOfType<UIController> ();
		board = FindObjectOfType<RBoard> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
