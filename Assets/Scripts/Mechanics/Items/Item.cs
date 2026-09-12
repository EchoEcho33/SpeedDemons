using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }

    private Rarity rarity;
    public abstract void Use();
}
