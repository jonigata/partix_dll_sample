using UnityEngine;
using System.Collections.Generic;

public struct Breathe {
    public float power;
    public float range;
    public Vector3 origin;
    public Vector3 direction;
}

public class BreathePubSub : PubSub<Breathe>{
}
