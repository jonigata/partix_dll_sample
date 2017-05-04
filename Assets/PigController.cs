using UnityEngine;
using System;
using System.Collections;
using InControl;

public class PigController : MonoBehaviour {
    [SerializeField] PartixWorld world;
    [SerializeField] PartixSoftVolume sv;
    [SerializeField] Vector3 accel;
    [SerializeField] Vector3 velocity;
    [SerializeField] float accelFactor = 2.0f;
    [SerializeField] ManualCamera manualCamera;

    const float epsilon = 1.0e-6f;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        sv = GetComponent<PartixSoftVolume>();

        accel = Vector3.zero;
        velocity = Vector3.zero;
    }

    IEnumerator Start() {
        while (!sv.Ready()) { yield return null; }

        Vector3[] vertices = sv.GetWireFrameVertices();
        VehiclePointLoad[] loads = sv.GetPointLoads();

        // boundingbox
        var mv = Single.MaxValue;
        Vector3 bbmin = new Vector3(mv, mv, mv);
        Vector3 bbmax = new Vector3(-mv, -mv, -mv);

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            if (v.x < bbmin.x) { bbmin.x = v.x; }
            if (v.y < bbmin.y) { bbmin.y = v.y; }
            if (v.z < bbmin.z) { bbmin.z = v.z; }
            if (bbmax.x < v.x) { bbmax.x = v.x; }
            if (bbmax.y < v.y) { bbmax.y = v.y; }
            if (bbmax.z < v.z) { bbmax.z = v.z; }
        }

        Vector3 bbw = bbmax - bbmin;

        // weightÝ’è
        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            float w = 1.0f;
            float z = 1.0f - (v.z - bbmin.z) / bbw.z;
            float y = 1.0f - (v.y - bbmin.y) / bbw.y;
            w += z * y * 4.0f;
            loads[i].weight = w * 0.2f;
            loads[i].friction = 0.8f;
        }
        sv.SetPointLoads(loads);
    }

    void Update() {
        if (!sv.Ready()) { return; }
        
        Vector2 i = InputManager.ActiveDevice.LeftStick;
        Vector3 a = new Vector3(i.x, 0, i.y);

        a = manualCamera.rotateConsideringCamera(a);

        float grip = 
            (sv.vehicleAnalyzeData.front_grip + 
             sv.vehicleAnalyzeData.rear_grip) * 0.5f;

        UpdateAccel(a);
        sv.AccelerateVehicle(a * accelFactor * grip);

        sv.Rotate(GetBalanceRotation());
        sv.Rotate(GetTurningRotation());
    }

    void UpdateAccel(Vector3 input) {
/*
        // ‹}ù‰ñ‚ÍƒuƒŒ[ƒL 
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

        Vector3 a = Vector3.zero;
        float len0 = a.magnitude;
        if (epsilon < len0) {
            a = accel.normalized;
        } else {
            a = sv.vehicleAnalyzeData.front;
        }
        float len1 = input.magnitude;

        // ‹}Œƒ‚È•Ï‰»–hŽ~
        float vr = world.deltaTime * 5.0f;
        float tr = world.deltaTime * 20.0f;

        // is•ûŒü‚ð‹…–Ê•âŠ®
        if (epsilon < len1) {
            Quaternion q = Quaternion.FromToRotation(a, input);
            Quaternion qq = Quaternion.Slerp(Quaternion.identity, q, tr);
            a = qq * a;
        }
        accel = a * Mathf.Lerp(len0, len1, vr);
    }


    Quaternion GetBalanceRotation() {
        var a = sv.vehicleAnalyzeData;
        
        // Žp¨‚ª•œŒ³’†‚¾‚Á‚½‚ç‰½‚à‚µ‚È‚¢
        float pa = Vector3.Dot(Vector3.up, a.old_back);
        float ca = Vector3.Dot(Vector3.up, a.back);
        if (pa < ca) { return Quaternion.identity; }

        float angle = AxisAngleOnAxisPlane(
            Vector3.zero, a.back, Vector3.up, a.front);

        if (a.left_grip == 0 && 0 < a.right_grip) {
            // ¶‘«‚ª•‚‚¢‚Ä‚é‚Æ‚«‚Í‰E‚É‚ÍŒX‚©‚È‚¢
            if (0 < angle) {
                return Quaternion.identity;
            }
        } else if (a.right_grip == 0 && 0 < a.left_grip) {
            // ‰E‘«‚ª•‚‚¢‚Ä‚é‚Æ‚«‚Í¶‚É‚ÍŒX‚©‚È‚¢
            if (angle < 0) {
                return Quaternion.identity;
            }
        }


        float angleMax =
            sv.vehicleParameter.balance_angle_max * world.deltaTime;
        angle = Mathf.Clamp(angle, -angleMax, angleMax);
        return Quaternion.AngleAxis(angle, a.front);
    }

    Quaternion GetTurningRotation() {
        var a = sv.vehicleAnalyzeData;
        
        // Žp¨‚ª•œŒ³’†‚¾‚Á‚½‚ç‰½‚à‚µ‚È‚¢
        Vector3 an = accel.normalized;
        float pa = Vector3.Dot(an, a.old_front);
        float ca = Vector3.Dot(an, a.front);
        if (pa < ca) { return Quaternion.identity; }

        Quaternion q =  Quaternion.FromToRotation(
            sv.vehicleAnalyzeData.front, accel.normalized);
        return limitRotation(
            q, sv.vehicleParameter.turning_angle_max * world.deltaTime);
    }

    // http://tiri-tomato.hatenadiary.jp/entry/20121013/1350121871
    float AxisAngleOnAxisPlane( Vector3 origin, Vector3 fromDirection, Vector3 toDirection, Vector3 axis ) {
	fromDirection.Normalize();
	axis.Normalize();
	Vector3 toDirectionProjected = toDirection - axis * Vector3.Dot(axis,toDirection);
	toDirectionProjected.Normalize();
	return 
            Mathf.Acos(Mathf.Clamp(Vector3.Dot(fromDirection,toDirectionProjected),-1f,1f)) *
            (Vector3.Dot(Vector3.Cross(axis,fromDirection), toDirectionProjected) < 0f ? -Mathf.Rad2Deg : Mathf.Rad2Deg);
    }

    Quaternion limitRotation(Quaternion q, float max) {
        float angle;
        Vector3 axis;
        q.ToAngleAxis(out angle, out axis);
        if (max < angle) {
            return Quaternion.AngleAxis(max, axis);
        } else {
            return q;
        }
    }
}

