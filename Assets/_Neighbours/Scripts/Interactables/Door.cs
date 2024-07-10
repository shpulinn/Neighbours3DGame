using _Neighbours.Scripts;
using _Neighbours.Scripts.Interactables;
using UnityEngine;

public class Door : MonoBehaviour, IInventoryInteractable
{
    [SerializeField] private Item requiredItem;
    
    public void Interact()
    {
        Debug.Log("Дверь закрыта, нужен ключ");
    }

    public void TerminateInteraction()
    {
        throw new System.NotImplementedException();
    }

    public float InteractionDuration { get; }
    public float InteractionProgress { get; }
    
    public void InteractWithAnItem(Inventory inventory)
    {
        if (requiredItem == null)
            return;
        if (inventory.HasItem(requiredItem))
        {
            Debug.Log("Открыл дверь ключом");
            inventory.RemoveItem(requiredItem);
            gameObject.SetActive(false);
        }
        else
        {
            Interact();
        }
    }
}
