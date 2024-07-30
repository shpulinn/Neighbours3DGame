using System.Collections;
using _Neighbours.Scripts.UI;
using UnityEngine;

namespace _Neighbours.Scripts.Interactables
{
    public class Painting : MonoBehaviour, IInventoryInteractable
    {
        [SerializeField] private Item usingItem;
        [SerializeField] private GameObject textPopUpPrefab;
        [SerializeField] private string popUpMessageBefore;
        [SerializeField] private string popUpMessageAfter;
        [SerializeField] private float interactionTime;
        private TextPopUp _textPopUp;
        private float _interactionProgress;
        private bool _wasCorrupted = false;

        private void Start()
        {
            InitializeThoughtBubble();
        }
        
        private void InitializeThoughtBubble()
        {
            GameObject thoughtBubbleObject = Instantiate(textPopUpPrefab, transform);
            _textPopUp = thoughtBubbleObject.GetComponent<TextPopUp>();
            thoughtBubbleObject.transform.localPosition = Vector3.right * 2f;
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
            //
        }

        public void TerminateInteraction()
        {
            throw new System.NotImplementedException();
        }

        public float InteractionDuration { get; }
        public float InteractionProgress { get; }
        public void InteractWithAnItem(Inventory inventory)
        {
            // check for an appropriate item
            if (!usingItem)
            {
                Debug.Log("There is no using item added to: " + this.name);
                return;
            }

            if (_wasCorrupted)
            {
                ShowThought(popUpMessageAfter);
                return;
            }
            if (usingItem == inventory.HasItem(usingItem))
            {
                // make some action and remove item
                StartCoroutine(InteractionCoroutine());
                inventory.RemoveItem(usingItem);
            }
            else
            {
                ShowThought(popUpMessageBefore);
            }
        }

        private IEnumerator InteractionCoroutine()
        {
            _interactionProgress = 0;
            while (_interactionProgress <= interactionTime)
            {
                // doing smthng
                _interactionProgress += Time.deltaTime;
                // showing progress and play animation
                yield return null;
            }
            
            // change the sprite of the painting to corrupted
            
            _wasCorrupted = true;
        }
    }
}