using System.ComponentModel;
using System.Diagnostics;
//using System.Threading.Tasks.Dataflow;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

    public GameObject itemObject;
    public abstract void Use();

    public string getItemId()
    {
        return this._itemId;
    }

    public void spawnItem()
    {
        this.itemObject = new GameObject(this._itemId);
        BoxCollider box = this.itemObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        SpriteMask spriteMask = this.itemObject.AddComponent<SpriteMask>();
        spriteMask.sprite = this._icon;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(_itemId + " collided with " + other);
    }
}
