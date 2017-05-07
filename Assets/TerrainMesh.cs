using UnityEngine;

public class TerrainMesh : MonoBehaviour {
    public Terrain terrain;

    void Start() {
        SetUpWireFrame();
    }

    void SetUpWireFrame() {
        var d = terrain.terrainData;

        var w = d.heightmapWidth;
        var h = d.heightmapHeight;

        var mesh = new Mesh();
        var vertices = BuildVertices();
        mesh.vertices = vertices;

        int [] indices = new int[((w-1)*h + (h-1)*w + (w-1)*(h-1)) * 2];

        // â°ê¸
        int i = 0;
        for (int y = 0 ; y < h ; y++) {
            for (int x = 0 ; x < w-1 ; x++) {
                indices[i++] = y*w+x;
                indices[i++] = y*w+x+1;
            }
        }

        // ècê¸
        for (int y = 0 ; y < h-1 ; y++) {
            for (int x = 0 ; x < w ; x++) {
                indices[i++] = y*w+x;
                indices[i++] = (y+1)*w+x;
            }
        }

        // éŒÇﬂê¸(íZÇ¢ï˚ÇëIë)
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

        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        filter.mesh.SetIndices(indices, MeshTopology.Lines, 0);
    }

    void SetUpTriangles() {
        var d = terrain.terrainData;

        var w = d.heightmapWidth;
        var h = d.heightmapHeight;

        var mesh = new Mesh();
        var vertices = BuildVertices();
        mesh.vertices = vertices;

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

        var filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;
        filter.mesh.SetIndices(indices, MeshTopology.Triangles, 0);
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
                Debug.Log(v);
            }
        }
        return vertices;
    }
}
