using UnityEngine;

namespace _Neighbours.Scripts
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Transform openPosition;
        private bool _isOpen = false;

        public void OpenDoor()
        {
            if (!_isOpen)
            {
                transform.position = openPosition.position;
                _isOpen = true;
            }
        }

        public void CloseDoor()
        {
            if (_isOpen)
            {
                // Вернуть дверь в закрытое положение
                // transform.position = закрытое положение
                _isOpen = false;
            }
        }
    }

}