using UnityEngine;
using System;
using System.Collections;
using InControl;

public class PigInput : MonoBehaviour {
    [SerializeField] PigController pigController;
    [SerializeField] BreathePub breathePub;
    [SerializeField] ManualCamera manualCamera;

    PartixSoftVolume entity;

    void Awake() {
        entity = GetComponent<PartixSoftVolume>();
    }

    void Update() {
        Vector2 i = InputManager.ActiveDevice.LeftStick;
        Vector3 a = new Vector3(i.x, 0, i.y);

        a = manualCamera.rotateConsideringCamera(a);

        pigController.UpdateAccel(a);

        if (InputManager.ActiveDevice.Action1.IsPressed) {
            var dir = entity.vehicleAnalyzeData.front;
            breathePub.Breathe(transform.position, dir);
        }
    }
}
