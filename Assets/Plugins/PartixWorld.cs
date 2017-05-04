using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PartixWorld : MonoBehaviour {
    public float deltaTime;

    IntPtr nativeWorld = IntPtr.Zero;

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        nativeWorld = PartixDll.CreateWorld();
        PartixDll.SetGravity(nativeWorld, new Vector3(0, -9.8f, 0));
    }

    void OnDestroy() {
        PartixDll.DestroyWorld(nativeWorld);
    }

    void Update() {
        if (nativeWorld == IntPtr.Zero) { return; }
        PartixDll.UpdateWorld(nativeWorld, deltaTime);
    }

    public IntPtr CreateSoftVolume(
        string tcf, Vector3 p, float scale, float mass) {
        return PartixDll.CreateSoftVolume(nativeWorld, tcf, p, scale, mass);
    }

    public IntPtr CreateVehicle(
        string tcf, Vector3 p, float scale, float mass) {
        return PartixDll.CreateVehicle(nativeWorld, tcf, p, scale, mass);
    }

    public IntPtr CreateSoftShell(
        int vertex_count, [In] Vector3[] vertices,
        int triangle_count, [In] int[] triangles,
        int threshold, Vector3 location, float scale, float mass) {
        return PartixDll.CreateSoftShell(
            nativeWorld, 
            vertex_count, vertices,
            triangle_count, triangles, 
            threshold, location, scale, mass);
    }

    public IntPtr CreatePlane(Vector3 p, Vector3 n) {
        return PartixDll.CreatePlane(nativeWorld, p, n);
    }

    public Vector3 GetPosition(IntPtr body) {
        Vector3 v;
        PartixDll.GetPosition(nativeWorld, body, out v);
        return v;
    }

    public Matrix4x4 GetOrientation(IntPtr body) {
        Matrix4x4 m;
        PartixDll.GetOrientation(nativeWorld, body, out m);
        return m;
    }

    public Vector3[] GetWireFrameVertices(IntPtr body) {
        int c = PartixDll.GetWireFrameVertexCount(nativeWorld, body);
        Vector3[] a = new Vector3[c];
        PartixDll.GetWireFrameVertices(nativeWorld, body, a);
        return a;
    }

    public int[] GetWireFrameIndices(IntPtr body) {
        int c = PartixDll.GetWireFrameIndexCount(nativeWorld, body);
        int[] a = new int[c];
        PartixDll.GetWireFrameIndices(nativeWorld, body, a);
        return a;
    }

    public VehiclePointLoad[] GetPointLoads(IntPtr body) {
        int c = PartixDll.GetWireFrameVertexCount(nativeWorld, body);
        VehiclePointLoad[] a = new VehiclePointLoad[c];
        PartixDll.GetPointLoads(nativeWorld, body, a);
        return a;
    }

    public void SetPointLoads(IntPtr body, VehiclePointLoad[] loads) {
        PartixDll.SetPointLoads(nativeWorld, body, loads);
    }

    public void AnalyzeVehicle(
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        Vector3             prev_velocity,
        Matrix4x4           prev_orientaion,
        Matrix4x4           curr_orientaion,
        float               sensory_balance,
        ref VehicleAnalyzeData ad) {
        PartixDll.AnalyzeVehicle(
            nativeWorld,
            b,
            ref vehicleParameter,
            deltaTime,
            prev_velocity,
            prev_orientaion,
            curr_orientaion,
            sensory_balance,
            ref ad);
    }

    public void AccelerateVehicle(
        IntPtr              b,
        ref VehicleParameter vehicleParameter,
        Vector3             accel) {
        PartixDll.AccelerateVehicle(
            nativeWorld,
            b,
            ref vehicleParameter,
            accel);
    }

    public void RotateEntity(IntPtr b, Quaternion q) {
        PartixDll.RotateEntity(
            nativeWorld,
            b,
            q.w, q.x, q.y, q.z);
    }



}
