﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {


	public float autoLoadNextLevelAfter;
	
	void Start(){
	if(autoLoadNextLevelAfter <= 0){
		Debug.Log ("Level Autoload disabled - use a positive number in seconds to get the next level to load after this time");
	}else {
		Invoke("LoadNextLevel", autoLoadNextLevelAfter);
	}
	}
	
	
	public void LoadLevel(string sceneName){
	Debug.Log("Level Load Requested for: " + name);
		SceneManager.LoadScene(sceneName);
		//SceneManager.LoadScene(string scenePath)
	}
	
	public void QuitRequest(){
	Debug.Log ("Quit Request");
	Application.Quit();
	}
	
	public void LoadNextLevel(){
		Application.LoadLevel(Application.loadedLevel +1);
	}
	
	
}