using System.Collections;
using _Neighbours.Scripts;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    [SerializeField] private float interactionTime;
    [SerializeField] private Item itemToGive;
    private float _interactionProgress;
    
    public float InteractionDuration => interactionTime;
    public float InteractionProgress => _interactionProgress;

    private bool _wasOpened = false;
    
    public void Interact()
    {
        if (_wasOpened)
        {
            Debug.Log("Тут больше ничего нет");
            TerminateInteraction();
        }
        StartCoroutine(InteractionCoroutine());
    }

    public void TerminateInteraction()
    {
        StopAllCoroutines();
        Debug.Log("Interaction terminated");
    }

    private IEnumerator InteractionCoroutine()
    {
        _interactionProgress = 0;
        while (_interactionProgress <= interactionTime)
        {
            // doing smthng
            _interactionProgress += Time.deltaTime;
            Debug.Log("Seeking in a box");
            yield return null;
        }
        
        Debug.Log("Interaction completed");
        if (itemToGive == null)
        {
            Debug.Log("Тут больше ничего нет");
            yield return null;
        }
        // bad code
        FindObjectOfType<Inventory>().AddItem(itemToGive);
        _wasOpened = true;
    }
}
