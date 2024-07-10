using UnityEngine;
using Cinemachine;

namespace _Neighbours.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float cameraSpeed = 10f;
        [SerializeField] private float leftBoundary = -10f;
        [SerializeField] private float rightBoundary = 10f;
        [SerializeField] private float edgeSize = 50f;
        [SerializeField] private float returnDelay = 5f;
        [SerializeField] private float returnSpeed = 2f;

        private CinemachineTransposer _transposer;
        private float _returnTimer;

        void Start()
        {
            _transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            _returnTimer = returnDelay;
        }

        void Update()
        {
            Vector3 cameraPosition = _transposer.m_FollowOffset;

            if (Input.mousePosition.x <= edgeSize)
            {
                cameraPosition.x += cameraSpeed * Time.deltaTime;
                _returnTimer = returnDelay; // Reset timer
            }
            else if (Input.mousePosition.x >= Screen.width - edgeSize)
            {
                cameraPosition.x -= cameraSpeed * Time.deltaTime;
                _returnTimer = returnDelay; // Reset timer
            }

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, leftBoundary, rightBoundary);
            _transposer.m_FollowOffset = cameraPosition;

            if (_returnTimer > 0)
            {
                _returnTimer -= Time.deltaTime;
            }
            else
            {
                ReturnToPlayer();
            }
        }

        void ReturnToPlayer()
        {
            Vector3 targetPosition = new Vector3(playerTransform.position.x, _transposer.m_FollowOffset.y, _transposer.m_FollowOffset.z);
            _transposer.m_FollowOffset = Vector3.Lerp(_transposer.m_FollowOffset, targetPosition, returnSpeed * Time.deltaTime);
        }
    }
}