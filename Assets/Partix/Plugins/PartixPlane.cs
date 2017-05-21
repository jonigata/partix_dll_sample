using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PartixPlane : MonoBehaviour {
    PartixWorld world;
    IntPtr nativePartixPlane;

    void Awake() {
        world = FindObjectOfType<PartixWorld>();
    }

    IEnumerator Start() {
        yield return new WaitUntil(() => PartixDll.initialized);

        nativePartixPlane = world.CreatePlane(transform.position, transform.up);
    }

}
