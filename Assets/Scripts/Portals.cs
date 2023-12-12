using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Portal {
    None,
    Exit,
    Entry,
    Both,
    End
}

public class Portals : MonoBehaviour {

    List<GameObject> walls = new List<GameObject> ();
    [SerializeField]
    Player player;
    [SerializeField]
    Slider portalSwitch;
    [SerializeField]
    Image portalSwitchKnob;
    GameObject exitPortal = null;
    GameObject entryPortal = null;
    GameObject endPortal = null;
    Portal portalToCreate = Portal.Entry;

    //subscribe to events
    void Awake () {
        MessageManager.onMessage += MessageReceived;
        MessageManager.onVector3Message += MessageReceived;
    }
    //unsubscribe to events
    void OnDisable () {
        MessageManager.onMessage -= MessageReceived;
        MessageManager.onVector3Message -= MessageReceived;
    }
    // Start is called before the first frame update
    void Start () {
        portalSwitch.value = 3;
        portalSwitchKnob.color = Color.red;
        portalToCreate = Portal.Entry;
    }

    public void PortalToCreate (float portal) {
        if (portal == 1f) {
            portalSwitchKnob.color = Color.green;
            portalToCreate = Portal.Exit;
        } else if (portal == 2f) {
            portalSwitchKnob.color = Color.yellow;
            portalToCreate = Portal.Both;
        } else if (portal == 3f) {
            portalSwitchKnob.color = Color.red;
            portalToCreate = Portal.Entry;
        }

        SoundManager.instance.PlayFX (FX.Switch);
    }

    void PortalPending (Vector3 wallCoordinates) {
        GameObject portalWall = null;
        foreach (GameObject wall in walls) {
            var wallT = wall.transform.localPosition;
            if (wallT.x == wallCoordinates.x && wallT.y == wallCoordinates.y) {
                portalWall = wall;
                break;
            }
        }

        if (portalWall && portalToCreate != Portal.None) {
            if (portalToCreate == Portal.Exit && portalWall != exitPortal) {
                if (portalWall != endPortal) {
                    if (entryPortal == portalWall)
                        entryPortal = null;
                    if (exitPortal)
                        exitPortal.GetComponent<Wall> ().ClosePortal ();
                    portalWall.GetComponent<Wall> ().CreatePortal (Portal.Exit);
                    exitPortal = portalWall;
                    player.SetExitPortalPos (portalWall.transform.localPosition);
                }
            } else if (portalToCreate == Portal.Entry && portalWall != entryPortal) {
                if (portalWall != endPortal) {
                    if (exitPortal == portalWall)
                        exitPortal = null;
                    if (entryPortal)
                        entryPortal.GetComponent<Wall> ().ClosePortal ();
                    portalWall.GetComponent<Wall> ().CreatePortal (Portal.Entry);
                    entryPortal = portalWall;
                }
            } else if (portalToCreate == Portal.Both) {
                if (portalWall != endPortal) {
                    GameObject futureEntryPortal = null;
                    if (entryPortal == portalWall)
                        entryPortal = null;
                    if (exitPortal == portalWall)
                        futureEntryPortal = portalWall.GetComponent<Wall> ().CreatePortal (true);
                    else {
                        if (exitPortal)
                            exitPortal.GetComponent<Wall> ().ClosePortal ();
                        futureEntryPortal = portalWall.GetComponent<Wall> ().CreatePortal (false);
                        exitPortal = portalWall;
                    }
                    player.SetExitPortalPos (exitPortal.transform.localPosition);
                    if (futureEntryPortal != endPortal) {
                        if (entryPortal) {
                            if (entryPortal != futureEntryPortal)
                                entryPortal.GetComponent<Wall> ().ClosePortal ();
                            if (futureEntryPortal) {
                                futureEntryPortal.GetComponent<Wall> ().CreatePortal (Portal.Entry);
                                entryPortal = futureEntryPortal;
                            }
                        } else {
                            if (futureEntryPortal) {
                                futureEntryPortal.GetComponent<Wall> ().CreatePortal (Portal.Entry);
                                entryPortal = futureEntryPortal;
                            }
                        }
                    }
                }
            }
        }
    }

    void MessageReceived (MessageType message, GameObject sender) {
        switch (message) {
            case MessageType.Wall:
                for (int i = 0; i < walls.Count; i++) {
                    if (walls[i] == sender)
                        return;
                }
                walls.Add (sender);
                break;
            case MessageType.EndPortal:
                if (!endPortal) {
                    List<GameObject> potentialWall = new List<GameObject> ();
                    for (int i = 0; i < walls.Count; i++) {
                        if (walls[i] != exitPortal || walls[i] != entryPortal) {
                            if ((walls[i].transform.localPosition.x != 5.5f || walls[i].transform.localPosition.x != -5.5f) && walls[i].transform.localPosition.y != -4.5f)
                                potentialWall.Add (walls[i]);
                        }
                    }

                    GameObject endPortalWall = potentialWall[Random.Range (0, potentialWall.Count)];
                    endPortalWall.GetComponent<Wall> ().CreatePortal (Portal.End);
                    endPortal = endPortalWall;
                }
                break;
            default:
                break;
        }
    }

    void MessageReceived (MessageType message, Vector3 value, GameObject sender) {
        switch (message) {
            case MessageType.PortalPending:
                PortalPending (value);
                break;
            default:
                break;
        }
    }
}