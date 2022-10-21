using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Events
{
    public static event Action<int> OnIntelligenceChanged = null;
    public static void DoChangeIntelligence(int value) => OnIntelligenceChanged?.Invoke(value);

    public static event Action<float> OnImpactMultiplierChanged = null;
    public static void DoChangeImpactMultiplier(float value) => OnImpactMultiplierChanged?.Invoke(value);

    public static event Action<float> OnBallSpeedChanged = null;
    public static void DoChangeBallSpeed(float value) => OnBallSpeedChanged?.Invoke(value);
}
