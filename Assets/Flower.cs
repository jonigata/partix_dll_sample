using UnityEngine;
using System;
using System.Collections;
using InControl;

public class Flower : MonoBehaviour {
    PartixWorld world;
    PartixSoftVolume sv;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
        sv = GetComponent<PartixSoftVolume>();
    }

    IEnumerator Start() {
        while (!sv.Ready()) { yield return null; }

        Vector3[] vertices = sv.GetWireFrameVertices();
        VehiclePointLoad[] loads = sv.GetPointLoads();

        // boundingbox
        var mv = Single.MaxValue;
        Vector3 bbmin = new Vector3(mv, mv, mv);
        Vector3 bbmax = new Vector3(-mv, -mv, -mv);

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            if (v.x < bbmin.x) { bbmin.x = v.x; }
            if (v.y < bbmin.y) { bbmin.y = v.y; }
            if (v.z < bbmin.z) { bbmin.z = v.z; }
            if (bbmax.x < v.x) { bbmax.x = v.x; }
            if (bbmax.y < v.y) { bbmax.y = v.y; }
            if (bbmax.z < v.z) { bbmax.z = v.z; }
        }

        Vector3 bbw = bbmax - bbmin;

        for (int i = 0 ; i < vertices.Length ; i++) {
            Vector3 v = vertices[i];
            Vector3 np = new Vector3(
                (v.x - bbmin.x) / bbw.x,
                (v.y - bbmin.y) / bbw.y,
                (v.z - bbmin.z) / bbw.z);
            loads[i].friction = 0;
            loads[i].fix_target = np.y < 0.1f ? 1 : 0;
        }
        sv.SetPointLoads(loads);

        EntityFeatures ef = new EntityFeatures();
        ef.stretch_factor = 0.1f;
        ef.restore_factor = 0.8f;
        ef.alive = 1;
        ef.positive = 1;
        ef.influential = 1;
        ef.auto_freezing = 1;
        ef.frozen = 1;
        sv.SetFeatures(ef);
    }

    void Update() {
        if (!sv.Ready()) { return; }
        sv.Fix();
    }

}

