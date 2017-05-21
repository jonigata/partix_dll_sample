using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(PartixSoftVolume))]
public class PartixWireFrame : MonoBehaviour {
    public Material material;
    public bool renderWeight;
    public bool renderAccel;
    public bool renderFriction;

    PartixWorld world;
    PartixSoftVolume volume;
    MeshFilter meshFilter;
    Mesh mesh;

    Color[] colors;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        volume = GetComponent<PartixSoftVolume>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }


    void Update() {
/*
        mesh.vertices = new Vector3[] {
            new Vector3(0, 1f),
            new Vector3(1f, -1f),
            new Vector3(-1f, -1f),
        };
        int [] indices = new int[] {
            0, 1, 2, 0,
        };
*/
        if (!volume.Ready()) { return; }
        

        int[] indices = volume.GetWireFrameIndices();
        mesh.vertices = volume.GetWireFrameVertices();
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        VehiclePointLoad[] vpl = volume.GetPointLoads();
        colors = new Color[vpl.Length];
        for (int i = 0 ; i < colors.Length ; i++) {
            var c = new Color(0, 0, 0);
            if (renderWeight) c.r = vpl[i].weight;
            if (renderFriction) c.g = vpl[i].friction;
            if (renderAccel) c.b = vpl[i].accel;
            colors[i] = c;
        }
        mesh.colors = colors;
    }

}
