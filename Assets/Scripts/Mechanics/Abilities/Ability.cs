using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour
{
    public abstract int _triggerCost();

    public abstract void TriggerAbility();

    [SerializeField]
    public string Description;
}
