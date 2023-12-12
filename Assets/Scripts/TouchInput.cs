using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {

    Vector2 touchOrigin = -Vector2.one;
    Camera cam;
    bool touched = false;
    // Start is called before the first frame update
    void Start () {
        cam = Camera.main;
    }

    float ConvertCoordinate (float coordinate) {
        if (coordinate >= 0.0f)
            return Mathf.Floor (coordinate) + 0.5f;
        else
            return (Mathf.Floor (coordinate) + 1.0f) - 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate () {

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        if (Input.GetMouseButtonDown (0) && !touched) {
            touched = true;

            Vector3 point = cam.ScreenToWorldPoint (Input.mousePosition);
            point.x = ConvertCoordinate (point.x);
            point.y = ConvertCoordinate (point.y);
            MessageManager.SendMessage (MessageType.PortalPending, point, gameObject);

        } else if (Input.GetMouseButtonUp (0)) {
            touched = false;
        }

        //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0) {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began && !touched) {
                touched = true;
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
                Vector3 point = cam.ScreenToWorldPoint (touchOrigin);
                point.x = ConvertCoordinate (point.x);
                point.y = ConvertCoordinate (point.y);
                MessageManager.SendMessage (MessageType.PortalPending, point, gameObject);
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended) {
                touched = false;
            }
        }
#endif
    }
}