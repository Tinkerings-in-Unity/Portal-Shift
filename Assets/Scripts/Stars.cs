using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour {
    [SerializeField]
    GameObject star1;
    [SerializeField]
    GameObject star2;
    [SerializeField]
    GameObject star3;
    [SerializeField]
    GameObject starEmpty1;
    [SerializeField]
    GameObject starEmpty2;
    [SerializeField]
    GameObject starEmpty3;
    int starsCollected = 0;

    //subscribe to events
    void Awake () {
        MessageManager.onMessage += MessageReceived;
    }
    //unsubscribe to events
    void OnDisable () {
        MessageManager.onMessage -= MessageReceived;
    }

    // Start is called before the first frame update
    void Start () {

    }

    IEnumerator AddStar () {
        GameObject star = null;
        GameObject emptyStar = null;

        if (starsCollected == 1) {
            star = star1;
            emptyStar = starEmpty1;
        } else if (starsCollected == 2) {
            star = star2;
            emptyStar = starEmpty2;
        } else if (starsCollected == 3) {
            star = star3;
            emptyStar = starEmpty3;
        }

        star.transform.localScale = new Vector3(2f,2f,1f);
        emptyStar.SetActive(false);
        star.SetActive(true);

        while(star.transform.localScale != Vector3.one)
        {
            yield return new WaitForSeconds(0.1f);
            star.transform.localScale -= new Vector3(0.1f,0.1f,0f);
        }

        CheckEnd();
    }

    void CheckEnd()
    {
        if (starsCollected == 3)
            MessageManager.SendMessage(MessageType.EndPortal,gameObject);
    }

    void MessageReceived (MessageType message, GameObject sender) {
        switch (message) {
            case MessageType.Star:
                starsCollected++;
                StartCoroutine ("AddStar");
                break;
            default:
                break;
        }
    }
}