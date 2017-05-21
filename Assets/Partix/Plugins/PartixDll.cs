using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public struct Triangle {
    int i0;
    int i1;
    int i2;
};

[System.Serializable]
public struct VehicleParameter {
    public float total_grip_coefficient;
    public float front_grip_coefficient;
    public float rear_grip_coefficient;
    public float sensory_balance_speed;
    public float sensory_balance_max;
    public float sensory_balance_decrease;
    public float balance_angle_max;
    public float turning_angle_max;
    public float backbend_angle_factor;
    public float bank_ratio;
    public float balance_ratio;
    public float turning_ratio;
    public float backbend_ratio;
    public float brake_angle;
    public float friction_factor;
    public float accel_factor;
    public float turbo_threshold;
    public float turbo_multiplier;
};

[System.Serializable]
public struct VehicleAnalyzeData {
    public float   total_grip;
    public float   front_grip;
    public float   rear_grip;
    public float   left_grip;
    public float   right_grip;
    public float   bottom_grip;
    public float sensory_balance;
    public Vector3  curr_velocity;
    public Vector3  prev_velocity;
    public float   speed;
    public float   front_speed;
    public Matrix4x4  com;
    public Matrix4x4  pom;
    public Vector3  right;
    public Vector3 back;
    public Vector3 front;
    public Vector3 old_right;
    public Vector3 old_back;
    public Vector3 old_front;

    public Vector3 bbmin;
    public Vector3 bbmax;
    public float   mass;
};

[System.Serializable]
public struct VehiclePointLoad {
    public float front_grip;
    public float rear_grip;
    public float left_grip;
    public float right_grip;
    public float accel;
    public float jump;
    public float weight;
    public float friction;
    public int fix_target;              // boolだとinteroperabilityが怪しい

}

[System.Serializable]
public struct EntityFeatures {
    public float stretch_factor;
    public float restore_factor;
    public int alive;                  // 本当はbool, interoperabilityの都合
    public int positive;               // 本当はbool, interoperabilityの都合
    public int influential;            // 本当はbool, interoperabilityの都合
    public int auto_freezing;          // 本当はbool, interoperabilityの都合
    public int frozen;                 // 本当はbool, interoperabilityの都合
}

public class PartixDll : MonoBehaviour {
    [NonSerialized] public static bool initialized = false;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugLogDelegate(string str);

    [DllImport("partix_dll")]
    public static extern void SetDebugLog(IntPtr p);

    [DllImport("partix_dll")]
    public static extern void DebugLogTest();

    [DllImport("partix_dll")]
    public static extern IntPtr CreateWorld();

    [DllImport("partix_dll")]
    public static extern void DestroyWorld(IntPtr p);

    [DllImport("partix_dll")]
    public static extern void UpdateWorld(IntPtr p, float delta);

    [DllImport("partix_dll")]
    public static extern void SetGravity(IntPtr p, Vector3 g);

    [DllImport("partix_dll")]
    public static extern IntPtr CreateSoftVolume(
        IntPtr world, string tcf, Vector3 position, float scale, float mass);

    [DllImport("partix_dll")]
    public static extern IntPtr CreateVehicle(
        IntPtr world, string tcf, Vector3 position, float scale, float mass);

    [DllImport("partix_dll")]
    public static extern IntPtr CreateSoftShell(
        IntPtr world, 
        int vertex_count, [In] Vector3[] vertices,
        int triangle_count, [In] int[] trangles,
        int threshold, Vector3 location, float scale, float mass);

    [DllImport("partix_dll")]
    public static extern IntPtr CreatePlane(
        IntPtr world, Vector3 position, Vector3 normal);

    [DllImport("partix_dll")]
    public static extern void GetPosition(
        IntPtr world, IntPtr body, out Vector3 normal);

    [DllImport("partix_dll")]
    public static extern void GetOrientation(
        IntPtr world, IntPtr body, out Matrix4x4 m);

    [DllImport("partix_dll")]
    public static extern int GetWireFrameVertexCount(
        IntPtr world, IntPtr b);
    [DllImport("partix_dll")]
    public static extern int GetWireFrameIndexCount(
        IntPtr world, IntPtr b);
    [DllImport("partix_dll")]
    public static extern void GetWireFrameVertices(
        IntPtr world, IntPtr b, [In, Out] Vector3[] buffer);
    [DllImport("partix_dll")]
    public static extern void GetWireFrameIndices(
        IntPtr world, IntPtr b, [In, Out] int[] buffer);
    [DllImport("partix_dll")]
    public static extern void GetPointLoads(
        IntPtr world, IntPtr b, [In, Out] VehiclePointLoad[] buffer);
    [DllImport("partix_dll")]
    public static extern void SetPointLoads(
        IntPtr world, IntPtr b, [In, Out] VehiclePointLoad[] buffer);

    [DllImport("partix_dll")]
    public static extern void AnalyzeVehicle(
        IntPtr              world,
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        float               dt,
        Vector3             prev_velocity,
        Matrix4x4           prev_orientaion,
        Matrix4x4           curr_orientaion,
        float               sensory_balance,
        ref VehicleAnalyzeData ad);

    [DllImport("partix_dll")]
    public static extern void AccelerateVehicle(
        IntPtr              world,
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        Vector3             accel);

    [DllImport("partix_dll")]
    public static extern void RotateEntity(
        IntPtr world, IntPtr b, float w, float x, float y, float z);
    [DllImport("partix_dll")]
    public static extern void RotateEntityWithPivot(
        IntPtr world, IntPtr b,
        float w, float x, float y, float z, Vector3 pivot);
    [DllImport("partix_dll")]
    public static extern void FixEntity(
        IntPtr world, IntPtr b, Vector3 origin);
    [DllImport("partix_dll")]
    public static extern void AddForce(
        IntPtr world, IntPtr b, Vector3 v);
    [DllImport("partix_dll")]
    public static extern void SetEntityFeatures(
        IntPtr world, IntPtr b, EntityFeatures ef);

    void Awake() {
        SetDebugLog();
        DebugLogTest();
        initialized = true;
    }

    void OnEnable() {
        SetDebugLog();
    }

    void OnDisable() {
        UnsetDebugLog();
    }

    void OnDestroy() {
        UnsetDebugLog();
    }

    void SetDebugLog() {
        System.Action<string> logFunc = (text) => Debug.Log(text);
        SetDebugLog(Marshal.GetFunctionPointerForDelegate(logFunc));
    }

    void UnsetDebugLog() {
        SetDebugLog(IntPtr.Zero);
    }
}
