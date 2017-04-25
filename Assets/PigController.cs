using UnityEngine;
using System;
using InControl;

public class PigController : MonoBehaviour {
    [SerializeField] PartixWorld world;
    [SerializeField] PartixSoftVolume sv;
    [SerializeField] Vector3 accel;
    [SerializeField] Vector3 velocity;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        sv = GetComponent<PartixSoftVolume>();

        accel = Vector3.zero;
        velocity = Vector3.zero;
    }

    void Update() {
        if (!sv.Ready()) { return; }
        Vector2 i = InputManager.ActiveDevice.LeftStick;
        Vector3 a = new Vector3(i.x, 0, i.y);
        UpdateAccel(a);
        sv.AccelerateVehicle(a * 2.0f);
    }

    void UpdateAccel(Vector3 input) {
/*
        // ã}ê˘âÒÇÕÉuÉåÅ[ÉL 
        {
            Vector3 v0 = velocity.normalized;
            Vector3 v1 = v.normalized;
            float breakAngle = Mathf.Deg2Rad * sv.vehicleParameter.brake_angle;
            float angle = Mathf.Acos(Vector3.Dot(v0, v1));
            if (breakAngle < Mathf.Abs(angle) && 3.0f < velocity.magnitude) {
                Debug.LogFormat("BREAKING: {0}\n", angle);
                v = Vector3.zero;
            }
        }
*/

        float epsilon = 1.0e-6f;
        Vector3 a = Vector3.zero;
        float len0 = a.magnitude;
        if (epsilon < len0) {
            a = accel.normalized;
        } else {
            a = sv.vehicleAnalyzeData.front;
        }
        float len1 = input.magnitude;

        // ã}åÉÇ»ïœâªñhé~
        float vr = world.deltaTime * 5.0f;
        float tr = world.deltaTime * 20.0f;

        // êiçsï˚å¸ÇãÖñ ï‚äÆ
        if (epsilon < len1) {
            Quaternion q = Quaternion.FromToRotation(a, input);
            Quaternion qq = Quaternion.Slerp(Quaternion.identity, q, tr);
            a = qq * a;
        }
        accel = a * Mathf.Lerp(len0, len1, vr);
        Debug.Log(accel);
    }
}

