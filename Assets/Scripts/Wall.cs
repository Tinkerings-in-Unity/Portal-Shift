using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalDirection {
    None,
    Down,
    Up,
    Right,
    Left
}

public class Wall : MonoBehaviour {
    Rigidbody2D rgBody;
    BoxCollider2D bxCollider;
    SpriteRenderer renderer;
    bool isPortal = false;
    public Portal portalType;
    PortalDirection portalDirection;
    public bool playerDetected = false;
    bool shouldClose = false;
    GameObject player;
    [SerializeField]
    SpriteRenderer portalLight;
    [SerializeField]
    SpriteRenderer portalLightSides;
    [SerializeField]
    GameObject exitParticles;
    [SerializeField]
    GameObject entryParticles;
    [SerializeField]
    GameObject endParticles;
    int layerMask;

    // Start is called before the first frame update
    void Start () {
        rgBody = gameObject.GetComponent<Rigidbody2D> ();
        bxCollider = gameObject.GetComponent<BoxCollider2D> ();
        renderer = gameObject.GetComponent<SpriteRenderer> ();
        MessageManager.SendMessage (MessageType.Wall, gameObject);
        layerMask = LayerMask.NameToLayer ("Default");
        layerMask = (1 << layerMask);
    }

    int Neighbours () {
        RaycastHit2D[] results = new RaycastHit2D[1];
        int neighbours = 0;
        //check right        
        neighbours = bxCollider.Raycast (Vector2.right, results, 1f);
        if (neighbours == 0)
            //check left
            neighbours = bxCollider.Raycast (-Vector2.right, results, 1f);

        return neighbours;
    }

    public void CreatePortal (Portal portal) {

        //play portal creation fx
        SoundManager.instance.PlayFX (FX.PortalCreation);
        portalType = portal;
        bxCollider.isTrigger = true;
        isPortal = true;
        SpriteRenderer pLight = portalLight;

        int neighbours = Neighbours ();

        if (transform.localPosition.y >= 0.5f && neighbours > 0)
            portalDirection = PortalDirection.Down;
        else if (transform.localPosition.y < 0.5f && neighbours > 0)
            portalDirection = PortalDirection.Up;
        else {
            pLight = portalLightSides;
            if (transform.localPosition.x <= 0.5f)
                portalDirection = PortalDirection.Right;
            else
                portalDirection = PortalDirection.Left;
        }
        if (portalType == Portal.Exit) {
            renderer.color = Color.green;
            pLight.color = Color.green;
            exitParticles.SetActive (true);
            StartCoroutine ("SwitchOffParticles");
        } else if (portalType == Portal.Entry) {
            renderer.color = Color.red;
            pLight.color = Color.red;
            entryParticles.SetActive (true);
            StartCoroutine ("SwitchOffParticles");
        } else if (portalType == Portal.End) {
            renderer.color = new Color (1f, 0f, 1f, 1f);
            pLight.color = new Color (1f, 0f, 1f, 1f);
            endParticles.SetActive (true);
            StartCoroutine ("SwitchOffParticles");
        }

        pLight.gameObject.SetActive (true);
    }

    public GameObject CreatePortal (bool alreadyExists) {

        if (!alreadyExists) {
            //play portal creation fx
            SoundManager.instance.PlayFX (FX.PortalCreation);
            portalType = Portal.Exit;
            bxCollider.isTrigger = true;
            isPortal = true;
            SpriteRenderer pLight = portalLight;

            int neighbours = Neighbours ();

            if (transform.localPosition.y >= 0.5f && neighbours > 0)
                portalDirection = PortalDirection.Down;
            else if (transform.localPosition.y < 0.5f && neighbours > 0)
                portalDirection = PortalDirection.Up;
            else {
                pLight = portalLightSides;
                if (transform.localPosition.x <= 0.5f)
                    portalDirection = PortalDirection.Right;
                else
                    portalDirection = PortalDirection.Left;
            }

            renderer.color = Color.green;
            pLight.color = Color.green;
            exitParticles.SetActive (true);
            StartCoroutine ("SwitchOffParticles");
            pLight.gameObject.SetActive (true);
        }

        int wallsHit = 0;
        Vector2 dir = Vector2.zero;
        if (portalDirection == PortalDirection.Down)
            dir = -Vector2.up;
        if (portalDirection == PortalDirection.Up)
            dir = Vector2.up;
        if (portalDirection == PortalDirection.Right)
            dir = Vector2.right;
        if (portalDirection == PortalDirection.Left)
            dir = -Vector2.right;

        RaycastHit2D[] results = new RaycastHit2D[1];
        wallsHit = bxCollider.Raycast (dir, results, Mathf.Infinity, layerMask);
        return results[0].collider.gameObject;
    }

    public void ClosePortal () {
        if(portalType != Portal.End)
        {
            portalType = Portal.None;
            isPortal = false;

            shouldClose = false;
            if (playerDetected) {
                playerDetected = false;
                if (portalType == Portal.Exit)
                    player.GetComponent<Player> ().EnteredExitPortal (portalDirection);
                if (portalType == Portal.Entry)
                    player.GetComponent<Player> ().EnteredEntryPortal ();
            }
            bxCollider.isTrigger = false;
            renderer.color = Color.white;
            portalLight.gameObject.SetActive (false);
            portalLightSides.gameObject.SetActive (false);
        }
    }

    IEnumerator SwitchOffParticles () {
        yield return new WaitForSeconds (1.5f);
        exitParticles.SetActive (false);
        entryParticles.SetActive (false);
        endParticles.SetActive (false);
    }

    void OnTriggerEnter2D (Collider2D col) {
        switch (col.gameObject.name) {
            case "PlayerObject":
                player = col.gameObject;
                if (!playerDetected) {
                    playerDetected = true;
                    if (portalType == Portal.Exit)
                        player.GetComponent<Player> ().EnteredExitPortal (portalDirection);
                    if (portalType == Portal.Entry)
                        player.GetComponent<Player> ().EnteredEntryPortal ();
                    if (portalType == Portal.End)
                        player.GetComponent<Player> ().EnteredEndPortal ();

                    if (shouldClose)
                        Invoke ("ClosePortal", 0.4f);
                }
                break;
            default:
                break;
        }
    }

    void OnTriggerStay2D (Collider2D col) {
        switch (col.gameObject.name) {
            case "PlayerObject":
                player = col.gameObject;
                if (!playerDetected) {
                    playerDetected = true;
                    if (portalType == Portal.Exit)
                        player.GetComponent<Player> ().EnteredExitPortal (portalDirection);
                    if (portalType == Portal.Entry)
                        player.GetComponent<Player> ().EnteredEntryPortal ();
                    if (portalType == Portal.End)
                        player.GetComponent<Player> ().EnteredEndPortal ();

                    if (shouldClose)
                        Invoke ("ClosePortal", 0.4f);
                }
                break;
            default:
                break;
        }
    }

    void OnTriggerExit2D (Collider2D col) {
        switch (col.gameObject.name) {
            case "PlayerObject":
                playerDetected = false;
                if (portalType == Portal.Entry) { }
                break;
            default:
                break;
        }
    }
}