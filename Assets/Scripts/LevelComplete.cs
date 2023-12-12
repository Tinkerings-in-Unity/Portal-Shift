using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour {
    [SerializeField]
    float levelCompleteDelay = 2f;
    [SerializeField]
    Transform levelCompleteText;
    [SerializeField]
    Text levelComplete;
    [SerializeField]
    List<Color> levelCompleteColors;
    // Start is called before the first frame update
    void Start () {
        StartCoroutine ("Grow");
        StartCoroutine ("GoToLevelSelection");
        SoundManager.instance.PlayFX(FX.Cheers);
    }

    IEnumerator GoToLevelSelection () {
        if (!GameManager.instance.completedLevels.Contains (GameManager.instance.selectedLevel))
            GameManager.instance.completedLevels.Add (GameManager.instance.selectedLevel);
        yield return new WaitForSeconds (levelCompleteDelay);
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

    IEnumerator Grow () {
        while (levelCompleteText.localScale.x < 1.2f) {
            yield return new WaitForSeconds (0.1f);
            levelCompleteText.localScale += new Vector3 (0.05f, 0.05f, 0f);
        }
        if (levelCompleteColors.Count > 0) {
            int randIndx = Random.Range (0, levelCompleteColors.Count);
            levelComplete.color = levelCompleteColors[randIndx];
            levelCompleteColors.RemoveAt (randIndx);
        }
        StartCoroutine ("Shrink");
    }

    IEnumerator Shrink () {
        while (levelCompleteText.localScale.x > 1f) {
            yield return new WaitForSeconds (0.1f);
            levelCompleteText.localScale -= new Vector3 (0.05f, 0.05f, 0f);
        }
        if (levelCompleteColors.Count > 0) {
            int randIndx = Random.Range (0, levelCompleteColors.Count);
            levelComplete.color = levelCompleteColors[randIndx];
            levelCompleteColors.RemoveAt (randIndx);
        }
        StartCoroutine ("Grow");
    }
}