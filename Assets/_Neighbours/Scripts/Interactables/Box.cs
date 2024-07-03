using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Seeking in a box");
    }
}
