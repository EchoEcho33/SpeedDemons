using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemBox : MonoBehaviour
{
    [SerializeField]
    private Item item;

    // When a player enters the collider, picks up the item and adds it to the UI
    // When an AI enters the collider, TODO 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit!");
                GameManager.Instance.UIManager.PickUpItem(item);
            }

            RacerController controller = other.gameObject.GetComponent<RacerController>();
            controller.RacerState.PickUpItem(item);
            
            gameObject.SetActive(false);
        } 
    }
}
