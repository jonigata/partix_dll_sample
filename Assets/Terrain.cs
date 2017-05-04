using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour {
    void Start () {
        int n = 0;
        foreach (MeshFilter mf in GetComponentsInChildren<MeshFilter>()) {
            var mesh = mf.mesh;
            n += mesh.vertices.Length;
            Debug.Log(mesh.vertices.Length);
        }
        Debug.Log(n);
    }
}
