using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FourLegs : MonoBehaviour {
    public float    TotalGripCoefficient = 15.0f;
    public float    FrontGripCoefficient = 30.0f;
    public float    RearGripCoefficient = 30.0f;
    public float    SensoryBalanceSpeed = 0.02f;
    public float    SensoryBalanceMax = 15f;
    public float    SensoryBalanceDecrease = 0.9f;
    public float    BalanceAngleMax = 720.0f;
    public float    TurningAngleMax = 5400.0f;
    public float    BackbendAngleFactor = 0.15f;
    public float    BankRatio = 0.3f;
    public float    BalanceRatio = 0.1f;
    public float    TurningRatio = 0.2f;
    public float    BackbendRatio = 0.2f;
    public float    BrakeAngle = 150.0f;
    public float    FrictionFactor = 1.3f;
    public float    AccelFactor = 9.0f;
    public float    TurboThreshold = 25.0f;
    public float    TurboMultiplier = 4.0f;
}
    