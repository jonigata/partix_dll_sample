using UnityEngine;
using System.Collections.Generic;

public class BreatheSub : Sub<Breathe> {
    [SerializeField] PartixSoftVolume entity;

    void Awake() {
        if (entity == null) {
            entity = GetComponent<PartixSoftVolume>();
        }
    }

    public override void Subscribe(Breathe breathe) {
        Vector3 a = (transform.position - breathe.origin).normalized;
        Vector3 b = breathe.direction.normalized;
        float dot = Vector3.Dot(a, b);

        if (dot < 0) { return; }

        float lenSq = a.sqrMagnitude;
        Debug.Log(lenSq);
        if (0.1f < lenSq && lenSq <= breathe.range * breathe.range) {
            entity.AddForce(b * dot * breathe.power / lenSq);
        }
    }
}
