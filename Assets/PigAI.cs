using UnityEngine;
using System;
using System.Collections;
using InControl;

public class PigAI : MonoBehaviour {
    [SerializeField] PigController pigController;
    [SerializeField] Transform target;

    void Update() {
        Vector3 a = target.position - transform.position;
        pigController.UpdateAccel(a.normalized);
    }
}
