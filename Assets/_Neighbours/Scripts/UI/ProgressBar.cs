using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Neighbours.Scripts.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private Canvas canvas;

        private float _duration;
        private float _remainingTime;
        private bool _isActive = false;

        private void Start()
        {
            if (canvas != null && Camera.main != null)
            {
                canvas.worldCamera = Camera.main;
            }
            HideProgressBar();
        }

        private void Update()
        {
            if (_isActive)
            {
                _remainingTime -= Time.deltaTime;
                UpdateProgressBar();

                if (_remainingTime <= 0)
                {
                    CompleteAction();
                }
            }

            UpdateRotation();
        }
        
        private void UpdateRotation()
        {
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }
        }

        public void StartAction(string actionName, float actionDuration)
        {
            _duration = actionDuration;
            _remainingTime = actionDuration;
            if (actionText != null)
            {
                actionText.text = actionName;
            }
            _isActive = true;
            ShowProgressBar();
        }

        public void StopAction()
        {
            CompleteAction();
        }

        private void UpdateProgressBar()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = 1 - (_remainingTime / _duration);
            }
        }

        private void CompleteAction()
        {
            _isActive = false;
            HideProgressBar();
        }

        private void ShowProgressBar()
        {
            if (canvas != null)
            {
                canvas.enabled = true;
            }
        }

        private void HideProgressBar()
        {
            if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
        
        private void OnDisable()
        {
            // Очистка при отключении объекта
            _isActive = false;
        }

        private void OnDestroy()
        {
            // Очистка при уничтожении объекта
            _isActive = false;
            canvas = null;
            fillImage = null;
            actionText = null;
        }
    }
}