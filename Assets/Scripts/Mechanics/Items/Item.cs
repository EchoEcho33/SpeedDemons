using UnityEngine;

public abstract class Item : MonoBehaviour
{
    private enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }

    [Header("Item Attributes")]
    [SerializeField]
    private string _itemId;

    [SerializeField]
    private Rarity _rarity;

    [SerializeField]
    private Sprite _icon;
    public abstract void Use();
}
