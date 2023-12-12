using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {
    public int level;
    public Text levelText;
    public GameObject locked;
    public GameObject completed;

    void Start () {

    }

    public void OnClick () {
        //play click sound fx
        SoundManager.instance.PlayFX(FX.Click);
        GameManager.instance.selectedLevel = level;
        SoundManager.instance.PlayMusic(Track.Main);
        StartCoroutine ("LoadAsyncScene");
    }

    IEnumerator LoadAsyncScene () {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ("Level_" + level);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}