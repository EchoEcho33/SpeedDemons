using System;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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

    private Item _primaryItem = null;
    private Item _secondaryItem = null;
    private float _itemCooldown = 0.5f;
    private bool _onItemCooldown = false;

    public enum Debuff
    {
        SPINOUT,
        SPEEDREDUCTION
    }

    public enum Buff
    {
        BARRIER,
        SPEEDBOOST
    }

    private GameObject currVFX;
    private bool activatedRF = false;

    
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
    
    /// <summary>
    /// Respawns the racer at their last checkpoint.
    /// </summary>
    public void Respawn()
    {
        GameObject racerAndCar = RacerController.GetCharacterAndKart();
        Rigidbody racerAndCarRigidbody = racerAndCar.GetComponent<Rigidbody>();
        StartCoroutine(RespawnDelay(racerAndCar, racerAndCarRigidbody));
    }

    private IEnumerator RespawnDelay(GameObject racerAndCar, Rigidbody racerAndCarRigidbody)
    {
        racerAndCar.SetActive(false);
        // Pause the racer while respawning
        racerAndCarRigidbody.constraints = RigidbodyConstraints.FreezePosition;
        racerAndCarRigidbody.useGravity = false;
        RacerController.preventMovement = true;
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(2.0f);

        // Resumes the racer
        racerAndCar.SetActive(true);
        racerAndCarRigidbody.constraints = RigidbodyConstraints.None;
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        racerAndCarRigidbody.useGravity = true;
        RacerController.preventMovement = false;

        // Places racer back on the track
        RacerController racerController = RacerController.Character.characterObject.GetComponentInParent<Drive>().Racer;
        TrackCheckpoint currentCheckpoint = racerController.RacerState.CurrCheckpoint;
        racerAndCar.transform.position = currentCheckpoint.transform.position + Vector3.up * RacerController.Character._spawnVerticalDisplacement;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();

        if (RacerController is AIController aiController)
        {
            RaceChoreographer.Instance.SendRacerRespawnEvent(aiController, currentCheckpoint);
        }
    }

    //filler method for ability bar, im guessing the bar updates additively/subtractively
    public void updateBar(int add)
    {
        abilityBar = Mathf.Clamp(0, abilityBar + add, abilityMaxValue);

        GameManager.Instance.UIManager.UpdateBar(abilityBar, abilityMaxValue);
    }

    public void PickUpItem(Item item)
    {
        if (_primaryItem == null)
        {
            _primaryItem = item;
        } else if (_secondaryItem == null)
        {
            _secondaryItem = item;
        }
    }

    public bool UseItem()
    {
        if (!_onItemCooldown && _primaryItem != null)
        {
            _onItemCooldown = true;
            Item instantiatedItem = Instantiate(_primaryItem, RacerController.transform);
            instantiatedItem.Use(this);
            GameManager.Instance.UIManager.UseItem();
            _primaryItem = _secondaryItem;
            _secondaryItem = null;
            StartCoroutine(ItemCooldown());
            return true;
        }
        return false;
    }

    private IEnumerator ItemCooldown()
    {
        yield return new WaitForSeconds(_itemCooldown);
        _onItemCooldown = false;
    }

    public void ApplyDebuff(Debuff debuff)
    {
        if (activatedRF)
        {
            activatedRF = false;
            Destroy(currVFX);
            return;
        }

        switch (debuff)
        {
            case Debuff.SPINOUT:
                DebuffSpinout();
                break;
            case Debuff.SPEEDREDUCTION:
                DebuffSpeedReduction(2f, 0.8f);
                break;
        }
    }

    public void ApplyDebuff(Debuff debuff, float timer, float multiplier)
    {
        if (activatedRF)
        {
            activatedRF = false;
            Destroy(currVFX);
            return;
        }

        switch (debuff)
        {
            case Debuff.SPINOUT:
                DebuffSpinout();
                Debug.LogError("Timer and Multiplier not necessary for Spinout.");
                break;
            case Debuff.SPEEDREDUCTION:
                DebuffSpeedReduction(timer, multiplier);
                break;
        }
    }

    private void DebuffSpinout()
    {
        if (RacerController.Drive.gameObject.transform.Find("Model").TryGetComponent<Animator>(out var animator))
        {
            animator.SetBool("SpinOut", true);
        }
        RacerController.SpinOut = true;
        RacerController.SpinOutDuration = 3.0f;
    }

    private void DebuffSpeedReduction(float timer, float multiplier)
    {
        if (multiplier > 1.0f)
            multiplier = 0.8f;

        RacerController.Drive.ApplyMultiplier(timer, multiplier);
    }

    public void ApplyBuff(Buff buff)
    {
        switch (buff)
        {
            case Buff.BARRIER:
                AddRabbitsFoot();
                break;
            case Buff.SPEEDBOOST:
                BuffSpeed(2f, 1.2f);
                break;
        }
    }

    public void ApplyBuff(Buff buff, float timer, float multiplier)
    {
        switch (buff)
        {
            case Buff.BARRIER:
                AddRabbitsFoot();
                Debug.LogError("Timer and Multiplier not necessary for Barrier.");
                break;
            case Buff.SPEEDBOOST:
                BuffSpeed(timer, multiplier);
                break;
        }
    }

    private void AddRabbitsFoot()
    {
        activatedRF = true;
    }

    private void BuffSpeed(float timer, float multiplier)
    {
        if (multiplier < 1.0f)
            multiplier = 1.2f;

        RacerController.Drive.ApplyMultiplier(timer, multiplier);
    }

    public void ApplyVFX(GameObject particleSys)
    {
        currVFX = Instantiate(particleSys, RacerController.Drive.gameObject.transform);
    }
}
