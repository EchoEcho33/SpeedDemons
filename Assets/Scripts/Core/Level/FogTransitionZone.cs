using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class FogTransitionZone : MonoBehaviour
{
    public BoxCollider box;
    
    public GameObject skyBox;
    
    [Header("A")]
    public GradientFogPreset GradientFogPresetA;

    public CloudsPreset CloudsPresetA;
    
    [Header("B")]
    public GradientFogPreset GradientFogPresetB;
    
    public CloudsPreset CloudsPresetB;
    
    private GradientFogSettings GradientFogSettingsA;
    
    private GradientFogSettings GradientFogSettingsB;

    private GradientFogRenderer localGradientFogRender;
    
    private PlayerController localRacer;
    
    private Material cloudsMaterial;
    
    private readonly int cloudTransitionColorID = Shader.PropertyToID("_CloudTransitionColor");
    
    private readonly int cloudBaseColorID = Shader.PropertyToID("_CloudBaseColor");

    protected virtual void Start()
    {
        if (!box) return;
        box.enabled = true;

        if (!skyBox)
        {
            Debug.LogError($"Assign skybox to FogTransitionZone '{gameObject.name}'");
            return;
        }
        
        MeshRenderer meshRenderer = skyBox.GetComponent<MeshRenderer>();
        cloudsMaterial = meshRenderer?.material;
        if (!cloudsMaterial)
        {
            Debug.LogError($"FogTransitionZone '{gameObject.name}. Incorrect skybox gameobject or skybox is not configured correctly and is missing the cloud material.");
            return;
        }
        
        bool zoneConfigured = true;
        
        if (!GradientFogPresetA)
        {
            zoneConfigured = false;
            Debug.LogError($"FogTransitionZone '{gameObject.name}' requires GradientFogPresetA.");
        }
        
        if (!GradientFogPresetB)
        {
            zoneConfigured = false;
            Debug.LogError($"FogTransitionZone '{gameObject.name}' requires GradientFogPresetB.");
        }

        if (!CloudsPresetA)
        {
            zoneConfigured = false;
            Debug.LogError($"FogTransitionZone '{gameObject.name}' requires CloudsPresetA.");
        }

        if (!CloudsPresetB)
        {
            zoneConfigured = false;
            Debug.LogError($"FogTransitionZone '{gameObject.name}' requires CloudsPresetB.");
        }

        if (!zoneConfigured)
        {
            box.enabled = false;
            return;
        }
        
        GradientFogSettingsA = Instantiate(GradientFogPresetA).gradientFogSettings;
        GradientFogSettingsB = Instantiate(GradientFogPresetB).gradientFogSettings;
        
        UpdateBoxColliderPosition();
    }

    protected void OnTriggerEnter(Collider other)
    {
        Drive drive = other.gameObject.GetComponent<Drive>();
        if (drive == null) return;
        
        RacerController racer = drive.Racer;
        if (racer == null) return;
        
        if (racer is not PlayerController newLocalRacer) return;
        localRacer = newLocalRacer;
        
        enabled = true;
    }
    
    protected void OnTriggerExit(Collider other)
    {
        Drive drive = other.gameObject.GetComponent<Drive>();
        if (drive == null) return;
        
        RacerController racer = drive.Racer;
        if (racer == null) return;
        
        if (racer != localRacer) return;
        
        enabled = false;
        
        Vector3 racerPosition = localRacer.GetKartPosition();
        Vector3 localRacerPosition = transform.InverseTransformPoint(racerPosition);

        if (localRacerPosition.x > box.center.x)
        {
            GradientFogRenderer.Instance.SetSettings(GradientFogSettingsA);
            SetCloudColors(CloudsPresetA);
        }
        else
        {
            GradientFogRenderer.Instance.SetSettings(GradientFogSettingsB);
            SetCloudColors(CloudsPresetB);
        }
        GradientFogRenderer.Instance.SetSettings(localRacerPosition.x > box.center.x ? GradientFogSettingsA : GradientFogSettingsB);
        
        localRacer = null;
    }

    protected void Update()
    {
        if (!localRacer || !box) return;
        
        Vector3 colliderStartPosition = box.transform.TransformPoint(box.center + Vector3.right * box.size.x * 0.5f);

        Vector3 colliderForward = -box.transform.right;
        Vector3 racerPosition = localRacer.GetKartPosition();
        
        float distance = Vector3.Dot(racerPosition - colliderStartPosition, colliderForward);
        float boxLength = box.size.x * box.transform.lossyScale.x;
        
        float t = Mathf.Clamp01(distance / boxLength);

        GradientFogSettings newGradientFogSettings = GradientFogSettings.LerpGradients(GradientFogSettingsA, GradientFogSettingsB, t);
        GradientFogRenderer.Instance.SetSettings(newGradientFogSettings);
        
        Color newCloudTransitionColor = Color.Lerp(CloudsPresetA.CloudTransitionColor, CloudsPresetB.CloudTransitionColor, t);
        Color newCloudBaseColor = Color.Lerp(CloudsPresetA.CloudBaseColor, CloudsPresetB.CloudBaseColor, t);
        SetCloudColors(newCloudTransitionColor, newCloudBaseColor);
    }
    
#if UNITY_EDITOR
    private void OnEnable()
    {
        Undo.postprocessModifications += UpdateBoxCollider;
    }

    private void OnDisable()
    {
        Undo.postprocessModifications -= UpdateBoxCollider;
    }
    
    private UndoPropertyModification[] UpdateBoxCollider(UndoPropertyModification[] undoPropertyModifications)
    {
        foreach (UndoPropertyModification modification in undoPropertyModifications)
        {
            if (modification.currentValue.target is Component component && component.gameObject == gameObject)
            {
                if (component == box) UpdateBoxColliderPosition();
                break;
            }
        }

        return undoPropertyModifications;
    }
    
    protected void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        if (!box) return;
        
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(box.center, box.size);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(box.center, box.size);
        
        Quaternion forwardVectorRotation = transform.rotation * Quaternion.Euler(Vector3.down * 90);
        
        Handles.color = new Color(1f, 1f, 0f, 0.5f);
        Vector3 conePosition = transform.TransformPoint(box.center) + forwardVectorRotation * Vector3.forward * box.size.x / 2 * 1.5f;
        Handles.ConeHandleCap(0, conePosition, forwardVectorRotation, 5.0f, EventType.Repaint);
        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 25;
        style.fontStyle = FontStyle.Bold;
        
        Vector3 colliderStartPosition = box.transform.TransformPoint(box.center + Vector3.right * box.size.x * 0.5f);
        Handles.Label(colliderStartPosition, "A", style);
        Vector3 colliderEndPosition = box.transform.TransformPoint(box.center - Vector3.right * box.size.x * 0.5f);
        Handles.Label(colliderEndPosition, "B", style);
    }
#endif

    private void UpdateBoxColliderPosition()
    {
        box.center = Vector3.up * box.size.y * 0.5f;
    }

    private void SetCloudColors(CloudsPreset cloudsPreset)
    {
        cloudsMaterial.SetColor(cloudTransitionColorID, cloudsPreset.CloudTransitionColor);
        cloudsMaterial.SetColor(cloudBaseColorID, cloudsPreset.CloudBaseColor);
    }
    
    private void SetCloudColors(Color newCloudTransitionColor, Color newCloudBaseColor)
    {
        cloudsMaterial.SetColor(cloudTransitionColorID, newCloudTransitionColor);
        cloudsMaterial.SetColor(cloudBaseColorID, newCloudBaseColor);
    }
}
