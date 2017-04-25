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

    PartixWorld world;
    PartixSoftVolume volume;
    MeshFilter meshFilter;
    Mesh mesh;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        volume = GetComponent<PartixSoftVolume>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
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
        meshFilter.mesh.SetIndices(indices, MeshTopology.Lines, 0);
    }

}
