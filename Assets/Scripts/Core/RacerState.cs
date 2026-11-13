using System;
using System.Collections;
using UnityEngine;

public class RacerState : MonoBehaviour
{
    public RacerController RacerController { get; private set; }
    
    public TrackCheckpoint CurrCheckpoint { get; private set;}
    
    public Action<TrackCheckpoint> OnReachedCheckpoint;
    
    public Action<StartFinishCheckpoint> OnReachedStartFinishCheckpoint;
    private bool isPlayer;
    public int currLap { get; private set;}

    public int currAbilityCharge {get; private set;}

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
        isPlayer = RacerController.GetType() == typeof(PlayerController);
    }

    public void StartRace(StartFinishCheckpoint startFinishCheckpoint)
    {
        CurrCheckpoint = startFinishCheckpoint;
        currLap = 1;
        currAbilityCharge = 0;

        if (RacerController is PlayerController)
        {
            PlayerController playerController = RacerController as PlayerController;
            playerController.playerUI.UpdateLap(1);
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
    public void InitialStartFinishCrossing(StartFinishCheckpoint startFinishCheckpoint)
    {
        OnReachedStartFinishCheckpoint?.Invoke(startFinishCheckpoint);
    }

    public void ReachCheckpoint(TrackCheckpoint checkpoint)
    {
        OnReachedCheckpoint?.Invoke(checkpoint);
        if (isPlayer)
            UpdateBar(2);

        CurrCheckpoint = checkpoint;
        if (checkpoint is StartFinishCheckpoint startFinishCheckpoint)
        {
            OnReachedStartFinishCheckpoint?.Invoke(startFinishCheckpoint);
            currLap++;
            
            //applies from all racers, change after getting Driver to connect with certain racerStates
            
            if (RacerController is PlayerController)
            {
                PlayerController playerController = RacerController as PlayerController;
                playerController.playerUI.UpdateLap(currLap);
            }
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
        if (RacerController.respawning)
        {
            yield break;
        }
        // this may seem like a strange edit, but disabling a gameobject unpairs its inputdevice.
        // when respawning, the character is invisible (scale of 0) but still active, so a flag must be set to prevent multiple respawns from 1 contact
        // -gianluca
        RacerController.respawning = true;
        var savedScale = racerAndCar.transform.localScale; 
        racerAndCar.transform.localScale = new Vector3(0,0,0);
        // Pause the racer while respawning
        racerAndCarRigidbody.constraints = RigidbodyConstraints.FreezePosition;
        racerAndCarRigidbody.useGravity = false;
        RacerController.preventMovement = true;
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;

        Transform model = racerAndCar.transform.Find("Model");
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(2.0f);

        // Resumes the racer
        racerAndCar.transform.localScale = savedScale;
        racerAndCarRigidbody.constraints = RigidbodyConstraints.None;
        racerAndCarRigidbody.linearVelocity = Vector3.zero;
        racerAndCarRigidbody.angularVelocity = Vector3.zero;
        racerAndCarRigidbody.useGravity = true;
        RacerController.preventMovement = false;

        // Places racer back on the track
        Drive driveScript = RacerController.Character.characterObject.GetComponentInParent<Drive>();
        driveScript.switchMode(0);
        RacerController racerController = driveScript.Racer;
        TrackCheckpoint currentCheckpoint = racerController.RacerState.CurrCheckpoint;
        racerAndCar.transform.position = currentCheckpoint.transform.position + Vector3.up * RacerController.Character._spawnVerticalDisplacement;
        racerAndCar.transform.rotation = currentCheckpoint.GetTrackForwardDirection();

        if (RacerController is AIController aiController)
        {
            RaceChoreographer.Instance.SendRacerRespawnEvent(aiController, currentCheckpoint);
        }
        
        RacerController.respawning = false;
    }

    public void UpdateBar(int add)
    {
        currAbilityCharge = Mathf.Clamp(0, currAbilityCharge + add, RacerController.Character.abilityCharge);
        if (RacerController is PlayerController)
        {
            PlayerController playerController = RacerController as PlayerController;
            playerController.playerUI.UpdateBar(currAbilityCharge, RacerController.Character.abilityCharge);
        }
    }

    public void ResetBar()
    {
        currAbilityCharge = 0;
        if (RacerController is PlayerController)
        {
            PlayerController playerController = RacerController as PlayerController;
            playerController.playerUI.UpdateBar(currAbilityCharge, RacerController.Character.abilityCharge);
        }
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
        if (RacerController is PlayerController)
        {
            PlayerController playerController = RacerController as PlayerController;
            playerController.playerUI.PickUpItem(item);
        }
    }

    public bool UseItem()
    {
        if (!_onItemCooldown && _primaryItem != null)
        {
            AudioManager.instance.PlayOneShot(AudioEvents.instance.useItem, this.transform.position);
            _onItemCooldown = true;
            Item instantiatedItem = Instantiate(_primaryItem, RacerController.transform);
            instantiatedItem.Use(this);
            
            if (RacerController is PlayerController)
            {
                PlayerController playerController = RacerController as PlayerController;
                playerController.playerUI.UseItem();
            }
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

    public void GrassSlowdown(float multiplier)
    {
        if (RacerController.Drive.Multiplier() <= 0)
            RacerController.Drive.OverrideSlowdown(multiplier);
    }

    public void RemoveSpeedDebuff()
    {
        if (RacerController.Drive.Multiplier() < 0)
            RacerController.Drive.ResetMultipliers();
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
