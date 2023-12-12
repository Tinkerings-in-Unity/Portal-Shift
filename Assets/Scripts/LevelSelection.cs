using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour {
    [SerializeField]
    Transform scrollViewContent;
    [SerializeField]
    GameObject levelButton;

    void Start () {
        if (GameManager.instance.unlockedLevels.Count == 0)
            GameManager.instance.unlockedLevels.Add (1);
        Setup ();
    }

    void Setup () {
        for (int x = 1; x < GameManager.instance.numLevels + 1; x++) {
            GameObject instance = Instantiate (levelButton, scrollViewContent);
            instance.name = "Level_" + x;
            instance.GetComponent<LevelButton> ().level = x;
            instance.GetComponent<LevelButton> ().levelText.text = "" + x;

            if (GameManager.instance.unlockedLevels.Contains (x)) {
                if (GameManager.instance.completedLevels.Contains (x)) {
                    instance.GetComponent<LevelButton> ().completed.SetActive (true);
                    GameManager.instance.unlockedLevels.Add (x + 1);
                    if (x >= 3) 
                        GameManager.instance.unlockedLevels.Add (x + 2);                    
                }
            } else {
                instance.GetComponent<LevelButton> ().locked.SetActive (true);
                instance.GetComponent<Button> ().interactable = false;
            }
        }
    }
}