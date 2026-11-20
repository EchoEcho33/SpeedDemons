using UnityEngine;

[CreateAssetMenu(fileName = "NewCloudsPreset", menuName = "Scriptable Objects/Shader/Clouds Preset")]
public class CloudsPreset : ScriptableObject
{
    [SerializeField]
    public Color CloudTransitionColor;
    
    [SerializeField]
    public Color CloudBaseColor;
}
