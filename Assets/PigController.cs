using UnityEngine;
using System;
using System.Collections;
using InControl;

public class PigController : MonoBehaviour {
    [SerializeField] Vector3 accel;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 liftUp;

    const float epsilon = 1.0e-6f;

    PartixWorld world;
    PartixSoftVolume sv;

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

        // weight設定
        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            Vector3 np = new Vector3(
                (v.x - bbmin.x) / bbw.x,
                (v.y - bbmin.y) / bbw.y,
                (v.z - bbmin.z) / bbw.z);

            float w = 1.0f;
            float z = 1.0f - np.z;
            float y = 1.0f - np.y;
            w += z * z * y * 6.0f;
            loads[i].weight = w * 0.2f;
            loads[i].friction = w * 0.2f;
            loads[i].fix_target = 0;
        }
        sv.SetPointLoads(loads);
    }

    void Update() {
        if (!sv.Ready()) { return; }
        
        float grip = 
            (sv.vehicleAnalyzeData.front_grip + 
             sv.vehicleAnalyzeData.rear_grip) * 0.5f;

        sv.AccelerateVehicle(
            accel * sv.vehicleParameter.accel_factor * grip +
            accel.magnitude * liftUp);

        sv.Rotate(GetBalanceRotation());
        // sv.Rotate(GetTurningRotation());
    }

    public void UpdateAccel(Vector3 input) {
/*
        // 急旋回はブレーキ 
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

        Vector3 a = accel;
        float len0 = a.magnitude;
        if (epsilon < len0) {
            a = accel.normalized;
        } else {
            a = sv.vehicleAnalyzeData.front;
        }
        float len1 = input.magnitude;

        // 急激な変化防止
        float vr = world.deltaTime * 5.0f;
        float tr = world.deltaTime * 5.0f;

        // 進行方向を球面補完
        if (epsilon < len1) {
            Quaternion q = Quaternion.FromToRotation(a, input);
            Quaternion qq = Quaternion.Slerp(Quaternion.identity, q, tr);
            a = qq * a;
        }
        accel = a * Mathf.Lerp(len0, len1, vr);
    }


    Quaternion GetBalanceRotation() {
        var a = sv.vehicleAnalyzeData;
        
        // 姿勢が復元中だったら何もしない
        float pa = Vector3.Dot(Vector3.up, a.old_back);
        float ca = Vector3.Dot(Vector3.up, a.back);
        if (pa < ca) { return Quaternion.identity; }

        float angle = AxisAngleOnAxisPlane(
            Vector3.zero, a.back, Vector3.up, a.front);

        if (a.left_grip == 0 && 0 < a.right_grip) {
            // 左足が浮いてるときは右には傾かない
            if (0 < angle) {
                return Quaternion.identity;
            }
        } else if (a.right_grip == 0 && 0 < a.left_grip) {
            // 右足が浮いてるときは左には傾かない
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
        
        // 姿勢が復元中だったら何もしない
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
        Debug.LogFormat("{0}, {1}", angle, max);
        if (max < angle) {
            return Quaternion.AngleAxis(max, axis);
        } else {
            return q;
        }
    }
}

