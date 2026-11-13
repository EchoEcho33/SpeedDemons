using UnityEngine;

public class BlightGlob : MonoBehaviour
{
    [SerializeField]
    private float scalar;
    [SerializeField]
    private float duration;
    
    private float decel = 0.8f;
    private RacerState player;

    private void Awake() {
        gameObject.transform.localScale = new Vector3(scalar, scalar, scalar);
    }
    
    private void OnTriggerEnter(Collider other) {
        Transform parentTransform = other.gameObject.transform.root;

        if (!(parentTransform.CompareTag("Player") || parentTransform.CompareTag("AI"))) {
            return;
        }

        if (parentTransform.TryGetComponent<Drive>(out var drive)) {
            drive.Racer.RacerState.ApplyDebuff(RacerState.Debuff.SPEEDREDUCTION, duration, decel);
            Destroy(gameObject);
        }
    }

    public void SetPlayerInitializer(RacerState player)
    {
        this.player = player;
    }
}
