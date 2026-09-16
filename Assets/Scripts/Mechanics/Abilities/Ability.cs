using Unity.VisualScripting;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract int _triggerCost();

    public abstract void TriggerAbility();

    public string abilityName;
    public string description;
    public Sprite icon;
}
