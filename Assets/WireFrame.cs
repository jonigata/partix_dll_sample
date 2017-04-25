using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshFilter))]

public class WireFrame : MonoBehaviour {
    void Start() {
        var mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(0, 1f),
            new Vector3(1f, -1f),
            new Vector3(-1f, -1f),
        };
        int [] indices = new int[] {
            0, 1, 2, 0,
        };

        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        Debug.Log(filter.mesh.GetIndices(0).Length);
        filter.mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
    }
}
