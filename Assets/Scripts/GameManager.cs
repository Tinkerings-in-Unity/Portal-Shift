﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//namespace PlayTheHappiness
//{
using System.Collections.Generic; //Allows us to use Lists. 
using UnityEngine.UI; //Allows us to use UI.

public class GameManager : MonoBehaviour {
	//Static instance of GameManager which allows it to be accessed by any other script.							
	public static GameManager instance = null;
	public int numLevels = 1;
	public List<int> completedLevels = new List<int> ();
	public List<int> unlockedLevels = new List<int> ();
	public int selectedLevel = 0;

	//Awake is always called before any Start functions
	void Awake () {
		//Check if instance already exists
		if (instance == null)
			//if not, set instance to this
			instance = this;
		//If instance already exists and it's not this:
		else if (instance != this)
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy (gameObject);

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad (gameObject);
	}

	//this is called only once, and the paramter tell it to be called only after the scene was loaded
	//(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
	static public void CallbackInitialization () {
		//register the callback to be called everytime the scene is loaded
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	//This is called each time a scene is loaded.
	static private void OnSceneLoaded (Scene arg0, LoadSceneMode arg1) {
		// instance.level++;
		// instance.InitGame();
	}
}
//}