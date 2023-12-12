using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        SoundManager.instance.PlayMusic(Track.Menu);
    }

    public void OnPlay () {
        SoundManager.instance.PlayFX (FX.Click);
        StartCoroutine ("LoadAsyncScene");
    }

    IEnumerator LoadAsyncScene () {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ("LevelSelect");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}