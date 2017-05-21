using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

public class PartixTerrain : MonoBehaviour {
    PartixWorld world;
    IntPtr nativePartixSoftShell = IntPtr.Zero;

    Terrain terrain;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        terrain = GetComponent<Terrain>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        var mesh = BuildTriangleMesh();

        Vector3[] vertices = mesh.vertices;
        int vertex_count = mesh.vertices.Length;
        int[] triangles = mesh.triangles;
        int triangle_count = mesh.triangles.Length / 3;

        Stopwatch sw = new Stopwatch();
        sw.Start();
        nativePartixSoftShell = world.CreateSoftShell(
            vertex_count, vertices,
            triangle_count, triangles,
            128, transform.position, transform.lossyScale.x, 1.0f);
        sw.Stop();
        UnityEngine.Debug.Log(sw.ElapsedMilliseconds+"ms");
    }

    Mesh BuildWireFrameMesh() {
        var d = terrain.terrainData;

        var w = d.heightmapWidth;
        var h = d.heightmapHeight;

        var mesh = new Mesh();
        var vertices = BuildVertices();
        mesh.vertices = vertices;

        int [] indices = new int[((w-1)*h + (h-1)*w + (w-1)*(h-1)) * 2];

        // 横線
        int i = 0;
        for (int y = 0 ; y < h ; y++) {
            for (int x = 0 ; x < w-1 ; x++) {
                indices[i++] = y*w+x;
                indices[i++] = y*w+x+1;
            }
        }

        // 縦線
        for (int y = 0 ; y < h-1 ; y++) {
            for (int x = 0 ; x < w ; x++) {
                indices[i++] = y*w+x;
                indices[i++] = (y+1)*w+x;
            }
        }

        // 斜め線(短い方を選択)
        for (int y = 0 ; y < h-1 ; y++) {
            for (int x = 0 ; x < w-1 ; x++) {
                int i0 = y*w+x;
                int i1 = (y+1)*w+(x+1);
                int i2 = y*w+(x+1);
                int i3 = (y+1)*w+x;
                Vector3 v0 = vertices[i0];
                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];
                var la = (v1 - v0).magnitude;
                var lb = (v3 - v2).magnitude;
                if (lb < la) {
                    indices[i++] = i2;
                    indices[i++] = i3;
                } else {
                    indices[i++] = i0;
                    indices[i++] = i1;
                }
            }
        }

        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        return mesh;
    }

    Mesh BuildTriangleMesh() {
        var d = terrain.terrainData;
        var th = d.size.y;

        var w = d.heightmapWidth;
        var h = d.heightmapHeight;

        var mesh = new Mesh();
        var vertices = BuildVertices();
        mesh.vertices = vertices;

        Color[] colors = new Color[vertices.Length];
        for (int j = 0 ; j < colors.Length ; j++) {
            var c = Color.Lerp(Color.yellow, Color.green, vertices[j].y / th);
            colors[j] = c;
        }
        mesh.colors = colors;

        int [] indices = new int[(w-1)*(h-1) * 2 * 3];
        int i = 0;
        for (int y = 0 ; y < h-1 ; y++) {
            for (int x = 0 ; x < w-1 ; x++) {
                int i0 = y*w+x;
                int i1 = (y+1)*w+(x+1);
                int i2 = y*w+(x+1);
                int i3 = (y+1)*w+x;
                Vector3 v0 = vertices[i0];
                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];
                var la = (v1 - v0).magnitude;
                var lb = (v3 - v2).magnitude;

                if (lb < la) {
                    indices[i++] = i0;
                    indices[i++] = i1;
                    indices[i++] = i2;
                    indices[i++] = i0;
                    indices[i++] = i3;
                    indices[i++] = i1;
                } else {
                    indices[i++] = i3;
                    indices[i++] = i2;
                    indices[i++] = i0;
                    indices[i++] = i3;
                    indices[i++] = i1;
                    indices[i++] = i2;
                }
            }
        }

        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        return mesh;
    }

    Vector3[] BuildVertices() {
        var d = terrain.terrainData;
        var tw = d.size.x;
        var tl = d.size.z;
        var th = d.size.y;

        var w = d.heightmapWidth;
        var h = d.heightmapHeight;
        var heights = d.GetHeights(0, 0, w, h);

        var vertices = new Vector3[w * h];
        for (int y = 0 ; y < h ; y++) {
            for (int x = 0 ; x < w ; x++) {
                var v = new Vector3(
                    tw / (w - 1) * x,
                    heights[y,x] * th,
                    tl / (h - 1) * y);
                vertices[y*w+x] = v;
            }
        }

        return vertices;
    }
}
