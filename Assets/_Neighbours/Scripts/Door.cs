using System;
using UnityEngine;

namespace _Neighbours.Scripts
{
    public class Door : MonoBehaviour//, IInteractable
    {
        [SerializeField] private Transform openPosition;
        [SerializeField] private Transform closedPosition;
        private bool _isOpen = false;

        public Transform DoorOpenPositon => openPosition;
        public Transform DoorClosedPosition => closedPosition;

        private Transform _visualTransform;

        private void Start()
        {
            _visualTransform = transform.GetChild(0);
        }

        // public void Interact()
        // {
        //     if (_isOpen)
        //     {
        //         CloseDoor();
        //     }
        //     else
        //     {
        //         OpenDoor();
        //     }
        // }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OpenDoor();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CloseDoor();
            }
        }

        public void OpenDoor()
        {
            if (!_isOpen)
            {
                _visualTransform.position = openPosition.position;
                _isOpen = true;
            }
        }

        public void CloseDoor()
        {
            if (_isOpen)
            {
                _visualTransform.position = closedPosition.position;
                _isOpen = false;
            }
        }
    }


}