using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceProperties", menuName = "ScriptableObjects/DiceProperties", order = 1)]
public class DicePropertiesScriptableObject : ScriptableObject
{
    [Header("Force/Rolling")]
    public float forceMultiplier;
    public float forceHeightOffsetBase;
    public float forceHeightOffsetMax;
    public float velocityStopThreshold;
    [Header("Animation")]
    public float slingshotRisingAnimationEndPercentage;
    public float slingshotRisingAnimationMaxHeight;
    public float slingshotRollingAnimationStartPercentage;
    public float slingshotRollingAnimationMaxSpeed;
}
