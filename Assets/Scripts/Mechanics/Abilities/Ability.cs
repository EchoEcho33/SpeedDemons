using Unity.VisualScripting;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract int _triggerCost();

    public abstract void TriggerAbility();

    private string abilityName;
    private string description;
    private Sprite icon;

    public string getAbilityName()
    {
        return abilityName;
    }

    public string getDescription()
    {
        return description;
    }

    public Sprite getIcon()
    {
        return icon;
    }
}
