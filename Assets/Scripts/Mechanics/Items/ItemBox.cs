using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemBox : MonoBehaviour
{

    // When a player enters the collider, picks up the item and adds it to the UI
    // When an AI enters the collider, TODO 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("AI"))
        {
            Item randomItem = GameManager.Instance.GetRandomItem();

            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit!");
                GameManager.Instance.UIManager.PickUpItem(randomItem);
            }

            Drive controller = other.gameObject.GetComponent<Drive>();
            controller.Racer.RacerState.PickUpItem(randomItem);
            
            Destroy(gameObject);
        } 
    }
}
