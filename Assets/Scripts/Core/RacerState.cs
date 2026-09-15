using UnityEngine;

public class RacerState : MonoBehaviour
{
    public RacerController RacerController { get; private set; }
    
    public TrackCheckpoint CurrCheckpoint { get; private set;}

    private PlayerRaceUI playerUI;

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

        if (RacerController.GetType().Equals(typeof(PlayerController)))
        {
            UIManager ui = GameManager.Instance.UI;
            playerUI = new PlayerRaceUI(ui.primaryItemIcon, ui.secondaryItemIcon, ui.playerLaps, ui.progressBar, ui.backgroundImage);
        }
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
        updateBarUI(2);

        CurrCheckpoint = checkpoint;
        if (checkpoint.GetType().Equals(typeof(StartFinishCheckpoint)))
        {
            currLap++;

            //applies from all racers, change after getting Driver to connect with certain racerStates
            if (playerUI == null) return;

            playerUI.UpdateLapUI(currLap);
                
        }
    }

    //filler method for ability bar, im guessing the bar updates additively/subtractively
    public void updateBarUI(int add)
    {

        abilityBar = Mathf.Clamp(0, abilityBar + add, abilityMaxValue);
        if (playerUI == null) return;

        playerUI.UpdateBar(abilityBar, abilityMaxValue);

    }
}
