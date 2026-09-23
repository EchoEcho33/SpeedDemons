using System;
using UnityEngine;

public class RacerState : MonoBehaviour
{
    public RacerController RacerController { get; private set; }
    
    public TrackCheckpoint CurrCheckpoint { get; private set;}
    
    public Action<TrackCheckpoint> OnReachedCheckpoint;
    
    public Action<StartFinishCheckpoint> OnReachedStartFinishCheckpoint;

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

        GameManager.Instance.UIManager.UpdateLap(1);
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
    public void InitialStartFinishCrossing(StartFinishCheckpoint startFinishCheckpoint)
    {
        OnReachedStartFinishCheckpoint?.Invoke(startFinishCheckpoint);
    }

    public void ReachCheckpoint(TrackCheckpoint checkpoint)
    {
        OnReachedCheckpoint?.Invoke(checkpoint);
        updateBar(2);

        CurrCheckpoint = checkpoint;
        if (checkpoint is StartFinishCheckpoint startFinishCheckpoint)
        {
            OnReachedStartFinishCheckpoint?.Invoke(startFinishCheckpoint);
            currLap++;
            
            //applies from all racers, change after getting Driver to connect with certain racerStates
            GameManager.Instance.UIManager.UpdateLap(currLap);
        }
    }

    //filler method for ability bar, im guessing the bar updates additively/subtractively
    public void updateBar(int add)
    {
        abilityBar = Mathf.Clamp(0, abilityBar + add, abilityMaxValue);

        GameManager.Instance.UIManager.UpdateBar(abilityBar, abilityMaxValue);
    }
}
