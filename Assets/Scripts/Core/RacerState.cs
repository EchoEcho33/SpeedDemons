using UnityEngine;

public class RacerState : MonoBehaviour
{
    public RacerController RacerController { get; private set; }
    
    public TrackCheckpoint CurrCheckpoint { get; private set;}

    public int currLap { get; private set;}

    //arbitrary max value
    public int abilityMaxValue = 10;
    public int abilityBar { get; private set; }
    
    public void AssignController(RacerController newRacerController)
    {
        RacerController = newRacerController;
    }

    public void StartRace(StartFinishCheckpoint startFinishCheckpoint)
    {
        CurrCheckpoint = startFinishCheckpoint;
        currLap = 1;
        abilityBar = 0;

        GameManager.Instance.UI.UpdateLap(1);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || RacerController is not PlayerController) return;

        if (CurrCheckpoint == null) return;

        BoxCollider box = CurrCheckpoint.nextCheckpoint.box;
        if (box == null) return;
        
        Gizmos.matrix = box.transform.localToWorldMatrix;

        if (CurrCheckpoint.nextCheckpoint is StartFinishCheckpoint nextCheckpoint)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
            Gizmos.DrawCube(box.center, box.size);
        
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(box.center, box.size);
        }
        else
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
            Gizmos.DrawCube(box.center, box.size);
        
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
#endif

    public void ReachCheckpoint(TrackCheckpoint checkpoint)
    {
        updateBar(2);

        CurrCheckpoint = checkpoint;
        if (checkpoint.GetType().Equals(typeof(StartFinishCheckpoint)))
        {
            currLap++;
            
            //applies from all racers, change after getting Driver to connect with certain racerStates
            GameManager.Instance.UI.UpdateLap(currLap);
        }
    }

    //filler method for ability bar, im guessing the bar updates additively/subtractively
    public void updateBar(int add)
    {
        abilityBar = Mathf.Clamp(0, abilityBar + add, abilityMaxValue);

        GameManager.Instance.UI.UpdateBar(abilityBar, abilityMaxValue);
    }
}
