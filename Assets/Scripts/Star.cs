using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {
    [SerializeField]
    float maxSize = 1.5f;
    [SerializeField]
    float expansionDelay = 0.05f;
    [SerializeField]
    float finalYPosOffset = 3f;
    [SerializeField]
    GameObject particles;
    [SerializeField]
    GameObject light;
    bool collected = false;
    // Start is called before the first frame update
    void Start () {

    }

    void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.name == "PlayerObject") {
            if(!collected)
                StartCoroutine ("Collected");
        }
    }

    IEnumerator Collected () {
        collected = true;
        //play star sound fx
        SoundManager.instance.PlayFX (FX.Star);
        particles.SetActive (true);
        light.SetActive (true);
        while (transform.localScale.x < maxSize) {
            yield return new WaitForSeconds (expansionDelay);
            transform.localScale += new Vector3 (0.1f, 0.1f, 0f);
        }
        //switch off particles
        // particles.SetActive (false);
        float finalYPos = transform.localPosition.y + finalYPosOffset;
        while (transform.localPosition.y < finalYPos) {
            yield return new WaitForSeconds (expansionDelay);
            transform.Rotate (new Vector3 (0f, 0f, 20f));
            transform.localPosition += new Vector3 (0f, 0.1f, 0f);
        }
        MessageManager.SendMessage(MessageType.Star,gameObject);
        gameObject.SetActive (false);
    }
}