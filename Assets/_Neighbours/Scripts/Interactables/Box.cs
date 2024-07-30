using System;
using System.Collections;
using _Neighbours.Scripts;
using _Neighbours.Scripts.UI;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject textPopUpPrefab;
    [SerializeField] private string popUpMessage;
    [SerializeField] private float interactionTime;
    [SerializeField] private Item itemToGive;
    private float _interactionProgress;
    
    public float InteractionDuration => interactionTime;
    public float InteractionProgress => _interactionProgress;

    private bool _wasOpened = false;

    private TextPopUp _textPopUp;

    private void Start()
    {
        InitializeThoughtBubble();
    }
    
    private void InitializeThoughtBubble()
    {
        GameObject thoughtBubbleObject = Instantiate(textPopUpPrefab, transform);
        _textPopUp = thoughtBubbleObject.GetComponent<TextPopUp>();
        thoughtBubbleObject.transform.localPosition = Vector3.up * 2.5f;
    }

    public void ShowThought(string thought, float duration = -1)
    {
        if (string.IsNullOrEmpty(thought))
        {
            return;
        }
        _textPopUp.ShowThought(thought, duration);
    }

    public void Interact()
    {
        if (_wasOpened)
        {
            ShowThought(popUpMessage, 2f);
            TerminateInteraction();
            return;
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
