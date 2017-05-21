using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

public class PartixSoftVolume : MonoBehaviour {
    public MeshRenderer[] meshRenderers;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Transform controlTransform;
    public string tcf;
    public bool vehicle;
    public VehicleParameter vehicleParameter;
    public VehicleAnalyzeData vehicleAnalyzeData;
    public float partixScale = 1.0f;
    public float pointMass = 1.0f;

    PartixWorld world;
    IntPtr nativePartixSoftVolume = IntPtr.Zero;
    public Matrix4x4 prevOrientation;
    public Matrix4x4 currOrientation;
    float sensoryBalance = 0;
    Vector3 sourcePosition;
    Vector3 offset;                     // Áâ©ÁêÜ„Å®ÊèèÁîª„ÅÆ„Ç™„Éï„Çª„ÉÉ„Éà

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        sourcePosition = controlTransform.position;
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        var textAsset = Resources.Load<TextAsset>("Tcf/" + tcf);
        Debug.Log(controlTransform.lossyScale.x);
        if (vehicle) {
            nativePartixSoftVolume = world.CreateVehicle(
                textAsset.text, 
                controlTransform.position, 
                partixScale,
                pointMass);
        } else {
            nativePartixSoftVolume = world.CreateSoftVolume(
                textAsset.text, 
                controlTransform.position, 
                partixScale,
                pointMass);
        }
        offset = world.GetPosition(nativePartixSoftVolume);
        // offset = Vector3.zero;
    }

    void Update() {
        if (nativePartixSoftVolume == IntPtr.Zero) { return; }
        var v = world.GetPosition(nativePartixSoftVolume);
        v += offset;
        controlTransform.localPosition = v;

        Matrix4x4 m = world.GetOrientation(nativePartixSoftVolume);
        prevOrientation = currOrientation;
        currOrientation = m;
        m[0,3] = 0;
        m[1,3] = 0;
        m[2,3] = 0;
        m[3,0] = 0;
        m[3,1] = 0;
        m[3,2] = 0;
        AttachDeform(m);

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

    public void Rotate(Quaternion q, Vector3 pivot) {
        world.RotateEntity(nativePartixSoftVolume, q, pivot);
    }

    public void Fix() {
        world.FixEntity(nativePartixSoftVolume, sourcePosition);
    }

    public void AddForce(Vector3 v) {
        world.AddForce(nativePartixSoftVolume, v);
    }

    public void SetFeatures(EntityFeatures ef) {
        world.SetEntityFeatures(nativePartixSoftVolume, ef);
    }

    public void AttachDeform(Matrix4x4 partixOrientation) {
        foreach (MeshRenderer r in meshRenderers) {
            Transform t = r.transform;
            List<Matrix4x4> matrices = new List<Matrix4x4>();
            while (t.gameObject != controlTransform.gameObject) {
                Matrix4x4 mm = Matrix4x4.TRS(
                    t.localPosition, t.localRotation, t.localScale);
                matrices.Add(mm);
                t = t.parent;
            }

            Matrix4x4 m = controlTransform.localToWorldMatrix;
            m *= partixOrientation;

            for (int i = 0 ; i < matrices.Count ; i++) {
                m *= matrices[matrices.Count - 1 - i];
            }
            Material material = r.sharedMaterial;

            // TODO: ñàâÒçÏÇ¡ÇƒÇÈ
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetMatrix("_Deform", m);
            props.SetMatrix("_ShadowDeform", m);
            r.SetPropertyBlock(props);
        }
        foreach (SkinnedMeshRenderer r in skinnedMeshRenderers) {
            Transform t = r.transform;
            List<Matrix4x4> matrices = new List<Matrix4x4>();
            while (t.gameObject != controlTransform.gameObject) {
                Matrix4x4 mm = Matrix4x4.TRS(
                    t.localPosition, t.localRotation, t.localScale);
                matrices.Add(mm);
                t = t.parent;
            }

            Matrix4x4 m = controlTransform.localToWorldMatrix;
            m *= partixOrientation;

            for (int i = 0 ; i < matrices.Count ; i++) {
                m *= matrices[matrices.Count - 1 - i];
            }
            Material material = r.sharedMaterial;

            // TODO: ñàâÒçÏÇ¡ÇƒÇÈ
            MaterialPropertyBlock props = new MaterialPropertyBlock();
            props.SetMatrix("_Deform", m);
            props.SetMatrix("_ShadowDeform", m);
            r.SetPropertyBlock(props);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }

    

}
