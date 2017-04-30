using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public class PartixSoftVolume : MonoBehaviour {
    public Material material;
    public string tcf;
    public bool vehicle;
    public VehicleParameter vehicleParameter;
    public VehicleAnalyzeData vehicleAnalyzeData;

    PartixWorld world;
    IntPtr nativePartixSoftVolume = IntPtr.Zero;
    public Matrix4x4 prevOrientation;
    public Matrix4x4 currOrientation;
    float sensoryBalance = 0;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        var textAsset = Resources.Load<TextAsset>("Tcf/" + tcf);
        Debug.Log(transform.lossyScale.x);
        if (vehicle) {
            nativePartixSoftVolume = world.CreateVehicle(
                textAsset.text, 
                transform.position, 
                transform.lossyScale.x);
        } else {
            nativePartixSoftVolume = world.CreateSoftVolume(
                textAsset.text, 
                transform.position, 
                transform.lossyScale.x);
        }
        world.GetWireFrameVertices(nativePartixSoftVolume);
        world.GetWireFrameIndices(nativePartixSoftVolume);
    }

    void Update() {
        if (nativePartixSoftVolume == IntPtr.Zero) { return; }
        var v = world.GetPosition(nativePartixSoftVolume);
        transform.localPosition = v;

        Matrix4x4 m = world.GetOrientation(nativePartixSoftVolume);
        prevOrientation = currOrientation;
        currOrientation = m;
        m[0,3] = 0;
        m[1,3] = 0;
        m[2,3] = 0;
        m[3,0] = 0;
        m[3,1] = 0;
        m[3,2] = 0;
        material.SetMatrix("_Deform", m);		

        if (vehicle) {
            AnalyzeVehicle();
        }

    }

    public bool Ready() { return nativePartixSoftVolume != IntPtr.Zero; }

    public Vector3[] GetWireFrameVertices() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetWireFrameVertices(nativePartixSoftVolume);
    }

    public int[] GetWireFrameIndices() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetWireFrameIndices(nativePartixSoftVolume);
    }

    public VehiclePointLoad[] GetPointLoads() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        return world.GetPointLoads(nativePartixSoftVolume);
    }

    public void SetPointLoads(VehiclePointLoad[] loads) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.SetPointLoads(nativePartixSoftVolume, loads);
    }

    public void AnalyzeVehicle() {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.AnalyzeVehicle(
            nativePartixSoftVolume,
            ref vehicleParameter,
            Vector3.zero,
            prevOrientation,
            currOrientation,
            sensoryBalance,
            ref vehicleAnalyzeData);
        sensoryBalance = vehicleAnalyzeData.sensory_balance;
    }

    public void AccelerateVehicle(Vector3 accel) {
        Assert.IsTrue(nativePartixSoftVolume != IntPtr.Zero);
        world.AccelerateVehicle(
            nativePartixSoftVolume,
            ref vehicleParameter,
            accel);
    }

    public void Rotate(Quaternion q) {
        world.RotateEntity(nativePartixSoftVolume, q);
    }

}
