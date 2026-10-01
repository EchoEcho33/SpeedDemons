using Unity.Behavior;
using UnityEngine;

public class RaceChoreographer : MonoBehaviour
{
    public BehaviorGraph graph;

    [SerializeField]
    public NavigationNetwork network;
    
    public static RaceChoreographer Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
