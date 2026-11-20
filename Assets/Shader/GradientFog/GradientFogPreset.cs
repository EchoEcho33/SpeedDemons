using UnityEngine;

[CreateAssetMenu(fileName = "NewGradientFogPreset", menuName = "Scriptable Objects/Shader/Gradient Fog Preset")]
public class GradientFogPreset : ScriptableObject
{
    [SerializeField]
    public GradientFogSettings gradientFogSettings;

    public void SetSettings(GradientFogSettings newGradientFogSettings)
    {
        gradientFogSettings = newGradientFogSettings;
    }
    
    public void OnValidate()
    {
        gradientFogSettings.StartDistance = gradientFogSettings.StartDistance < 0 ? 0 : gradientFogSettings.StartDistance;
        gradientFogSettings.EndDistance = gradientFogSettings.EndDistance < gradientFogSettings.StartDistance ? gradientFogSettings.StartDistance + 0.001f : gradientFogSettings.EndDistance;
            
        gradientFogSettings.FogStart = Mathf.Clamp01(gradientFogSettings.FogStart);
        gradientFogSettings.FirstColorThreshold = Mathf.Clamp01(gradientFogSettings.FirstColorThreshold);
            
        gradientFogSettings.FogMax = Mathf.Max(gradientFogSettings.FogStart + 0.001f, gradientFogSettings.FogMax);
        gradientFogSettings.FogMax = Mathf.Min(1f, gradientFogSettings.FogMax);
            
        gradientFogSettings.SecondColorThreshold = Mathf.Max(gradientFogSettings.FirstColorThreshold + 0.001f, gradientFogSettings.SecondColorThreshold);
        gradientFogSettings.SecondColorThreshold = Mathf.Min(1f, gradientFogSettings.SecondColorThreshold);
    }
}
