using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ManualCamera : MonoBehaviour {
    public float panScale;
    public float tiltScale;

    public float pan   = 0;
    public float tilt  = 0;

    void Update () {
        Vector2 i = InputManager.ActiveDevice.RightStick;
        pan += i.x * panScale;
        tilt += i.y * tiltScale;

        transform.localRotation = Quaternion.Euler(tilt, pan, 0);
    }

    public Vector3 rotateConsideringCamera(Vector3 v) {
        return Quaternion.Euler(0, pan, 0) * v;
        // return v;
    }
}
