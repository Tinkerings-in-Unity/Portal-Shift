using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    Rigidbody2D rb;
    Vector2 currVelocity = Vector2.zero;
    Vector3 exitPortalPos = Vector3.zero;
    Vector3 prevExitPortalPos = Vector3.zero;
    bool isColliding = false;
    bool calledAlready = false;
    bool enteredExit = false;
    bool moveToExitPending = false;
    [SerializeField]
    Vector2 velocityClamp = new Vector2 (-20f, 20f);
    [SerializeField]
    float normalGravityScale = 0.1f;
    [SerializeField]
    float modifiedGravityScale = 0.3f;
    [SerializeField]
    float normaliseGravityDelay = 0.4f;
    [SerializeField]
    Vector2 addedForce = new Vector2 (1.5f, 2f);
    [SerializeField]
    TrailRenderer trail;
    [SerializeField]
    GameObject particles;
    [SerializeField]
    GameObject cam;
    Vector3 origCamPosition;
    [SerializeField]
    float portalExitDelay = 0.5f;
    [SerializeField]
    float endDelay = 1f;
    float duration = 0f;
    float amount = 0f;
    float decreaseFactor = 0.5f;

    //subscribe to events
    void Awake () {
        MessageManager.onPortalMessage += MessageReceived;
    }
    //unsubscribe to events
    void OnDisable () {
        MessageManager.onPortalMessage -= MessageReceived;
    }
    // Start is called before the first frame update
    void Start () {
        rb = gameObject.GetComponent<Rigidbody2D> ();
        rb.gravityScale = normalGravityScale;
        origCamPosition = cam.gameObject.transform.localPosition;
    }

    public void SetExitPortalPos (Vector3 pos) {
        prevExitPortalPos = exitPortalPos;
        exitPortalPos = pos;
        if (moveToExitPending) {
            moveToExitPending = false;
            Invoke ("MoveToExit", portalExitDelay);
        }

        if (transform.localPosition.x == prevExitPortalPos.x && transform.localPosition.y == prevExitPortalPos.y)
            Invoke ("MoveToExit", portalExitDelay);
    }

    void Update () {

        if (duration > 0) {
            cam.gameObject.transform.localPosition = origCamPosition + Random.insideUnitSphere * amount;
            duration -= Time.deltaTime * decreaseFactor;
        } else {
            duration = 0f;
            cam.gameObject.transform.localPosition = origCamPosition;
        }
    }

    public void EnteredEntryPortal () {
        if (!calledAlready) {
            //play into entry fx
            SoundManager.instance.PlayFX (FX.IntoEntry);
            calledAlready = true;
        }
        trail.Clear ();
        currVelocity = new Vector2 (Mathf.Clamp (rb.velocity.x, velocityClamp.x, velocityClamp.y),
            Mathf.Clamp (rb.velocity.y, velocityClamp.x, velocityClamp.y));
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.SetActive (false);
        Invoke ("MoveToExit", portalExitDelay);
    }

    void MoveToExit () {
        if (exitPortalPos != Vector3.zero && exitPortalPos != prevExitPortalPos) {
            transform.localPosition = new Vector3 (exitPortalPos.x, exitPortalPos.y, 1f);
            gameObject.SetActive (true);
        } else
            moveToExitPending = true;
    }

    public void EnteredExitPortal (PortalDirection portalDirection) {
        calledAlready = false;
        enteredExit = true;
        particles.SetActive (true);
        StartCoroutine ("SwitchOffParticles");
        if (portalDirection != PortalDirection.Down) {
            if (portalDirection == PortalDirection.Up)
                currVelocity = new Vector2 (currVelocity.x, Mathf.Abs (currVelocity.y / 2) + Random.Range (addedForce.x, addedForce.y));
            else if (portalDirection == PortalDirection.Right)
                currVelocity = new Vector2 (Mathf.Abs (currVelocity.y) + Random.Range (addedForce.x, addedForce.y), 0f);
            else
                currVelocity = new Vector2 ((-1 * Mathf.Abs (currVelocity.y)) - Random.Range (addedForce.x, addedForce.y), 0f);

            rb.gravityScale = modifiedGravityScale;
            if (gameObject.activeSelf)
                StartCoroutine ("NormaliseGravity");
        }
        rb.velocity = currVelocity;
        rb.bodyType = RigidbodyType2D.Dynamic;
        Invoke ("OutOfExit", 0.2f);
    }

    void OutOfExit () {
        //play from exit fx
        SoundManager.instance.PlayFX (FX.FromExit);
        enteredExit = false;
    }

    public void EnteredEndPortal () {
        if (!calledAlready) {
            //play into entry fx
            SoundManager.instance.PlayFX (FX.IntoEntry);
            calledAlready = true;
        }
        trail.Clear ();
        currVelocity = new Vector2 (Mathf.Clamp (rb.velocity.x, velocityClamp.x, velocityClamp.y),
            Mathf.Clamp (rb.velocity.y, velocityClamp.x, velocityClamp.y));
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.SetActive (false);
        Invoke ("End", endDelay);
    }

    void End () {
        SoundManager.instance.PlayFX(FX.LevelClear);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ("LevelComplete");
    }

    IEnumerator LoadAsyncScene () {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync ("LevelComplete");
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

    private void OnCollisionEnter2D (Collision2D other) {
        if (other.gameObject.name == "Wall") {
            if (!enteredExit) {
                // float amount = 0f;
                if (currVelocity.x > currVelocity.y)
                    amount = currVelocity.x / 100f;
                else if (currVelocity.y > currVelocity.x)
                    amount = currVelocity.y / 100f;
                duration = 0.1f;
                SoundManager.instance.PlayFX (FX.BlockBounce);
                particles.SetActive (true);
                if (gameObject.activeSelf)
                    StartCoroutine ("SwitchOffParticles");
            }
        }
    }

    IEnumerator NormaliseGravity () {
        yield return new WaitForSeconds (0.4f);
        rb.gravityScale = normalGravityScale;
    }

    IEnumerator SwitchOffParticles () {
        yield return new WaitForSeconds (0.7f);
        particles.SetActive (false);
    }

    void MessageReceived (MessageType message, Portal value, GameObject sender) {
        switch (message) {
            case MessageType.PortalCreated:
                if (value == Portal.Exit) {
                    print ("Msg recieved");
                    exitPortalPos = sender.transform.localPosition;
                }
                break;
            default:
                break;
        }
    }
}