using UnityEngine;

namespace _Neighbours.Scripts.Interactables
{
    public class HidingCloset : MonoBehaviour, IHidable
    {
        public void Interact()
        {
            //throw new System.NotImplementedException();
        }

        public void TerminateInteraction()
        {
            //hrow new System.NotImplementedException();
        }

        public float InteractionDuration { get; }
        public float InteractionProgress { get; }
        
        public void HideInteraction(PlayerController playerController)
        {
            playerController.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
