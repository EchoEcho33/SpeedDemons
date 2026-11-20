using UnityEngine;

[ExecuteInEditMode]
public class Clouds : MonoBehaviour
{
    public CloudsPreset DefaultCloudsPreset;
    
    private Material cloudsMaterial;

    public void Start()
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        cloudsMaterial = meshRenderer?.material;
        if (!cloudsMaterial)
        {
            Debug.LogError("Skybox is not configured correctly and is missing the cloud material.");
            return;
        }

        if (!DefaultCloudsPreset)
        {
            Debug.LogError("Clouds is missing default clouds preset.");
        }
        
        SetCloudColors();
    }
    
#if UNITY_EDITOR
    public void OnValidate()
    {
        if (DefaultCloudsPreset) SetCloudColors();
    }
#endif

    private void SetCloudColors()
    {
        if (DefaultCloudsPreset) return;
        cloudsMaterial.SetColor("_CloudTransitionColor", DefaultCloudsPreset.CloudTransitionColor);
        cloudsMaterial.SetColor("_CloudBaseColor", DefaultCloudsPreset.CloudBaseColor);
    }
}