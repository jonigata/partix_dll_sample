using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartixSoftShell : MonoBehaviour {
    PartixWorld world;
    IntPtr nativePartixSoftShell = IntPtr.Zero;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        var meshFilter = GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        

        Vector3[] vertices = mesh.vertices;
        int vertex_count = mesh.vertices.Length;
        int[] triangles = mesh.triangles;
        int triangle_count = mesh.triangles.Length / 3;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        nativePartixSoftShell = world.CreateSoftShell(
            vertex_count, vertices,
            triangle_count, triangles,
            32768, transform.position, transform.lossyScale.x, 1.0f);
        sw.Stop();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds+"ms");
    }


    // Update is called once per frame
    void Update () {
		
    }

}
