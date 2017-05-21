using UnityEngine;
using System.Collections.Generic;

public class BreathePub : MonoBehaviour {
    [SerializeField] BreathePubSub pubSub;
    [SerializeField] float power;
    [SerializeField] float range;

    void Awake() {
        if (pubSub == null) {
            pubSub = FindObjectOfType<BreathePubSub>();
        }
    }
    
    public void Breathe(Vector3 origin, Vector3 direction) {
        Breathe b = new Breathe();
        b.power = power;
        b.range = range;
        b.origin = origin;
        b.direction = direction;
        pubSub.Publish(b);
    }
}
