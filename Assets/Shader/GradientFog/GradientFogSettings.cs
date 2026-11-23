using System;
using UnityEngine;

[Serializable]
public struct GradientFogSettings
{
    public float StartDistance;
    public float EndDistance;
    public Color NearColor;
    public Color MiddleColor;
    public Color FarColor;
        
    [Range(0f, 1f)]
    public float FogStart;
        
    [Range(0f, 1f)]
    public float FogMax;
        
    [Range(0f, 1f)]
    public float FirstColorThreshold;
        
    [Range(0f, 1f)]
    public float SecondColorThreshold;

    [Range(0f, 1f)]
    public float MaxOpacity;
    
    public static GradientFogSettings LerpGradients(GradientFogSettings a, GradientFogSettings b, float t)
    {
        GradientFogSettings newGradientFogSettings = new GradientFogSettings
        {
            StartDistance = Mathf.Lerp(a.StartDistance, b.StartDistance, t),
            EndDistance = Mathf.Lerp(a.EndDistance, b.EndDistance, t),
            
            NearColor = Color.Lerp(a.NearColor, b.NearColor, t),
            MiddleColor = Color.Lerp(a.MiddleColor, b.MiddleColor, t),
            FarColor = Color.Lerp(a.FarColor, b.FarColor, t),
            
            FogStart = Mathf.Lerp(a.FogStart, b.FogStart, t),
            FogMax = Mathf.Lerp(a.FogMax, b.FogMax, t),
            FirstColorThreshold = Mathf.Lerp(a.FirstColorThreshold, b.FirstColorThreshold, t),
            SecondColorThreshold = Mathf.Lerp(a.SecondColorThreshold, b.SecondColorThreshold, t),
            MaxOpacity = Mathf.Lerp(a.MaxOpacity, b.MaxOpacity, t)
        };

        return newGradientFogSettings;
    }
}
