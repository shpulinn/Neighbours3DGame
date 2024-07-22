using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Neighbours.Scripts.UI
{
    public class TextPopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI thoughtText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float defaultDuration = 3f;
        [SerializeField] private AnimationCurve fadeCurve;

        private Canvas _canvas;
        private Coroutine _activeCoroutine;
        private Transform _cameraTransform;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            if (_canvas != null)
            {
                _canvas.worldCamera = Camera.main;
            }
            _cameraTransform = Camera.main.transform;
            HideThought();
        }

        public void ShowThought(string thought, float duration = -1)
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
            }
            
            thoughtText.text = thought;
            _activeCoroutine = StartCoroutine(ShowThoughtCoroutine(duration < 0 ? defaultDuration : duration));
        }

        private IEnumerator ShowThoughtCoroutine(float duration)
        {
            // Показываем мысль
            yield return StartCoroutine(FadeCoroutine(0, 1));

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                UpdateRotation();
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Скрываем мысль
            yield return StartCoroutine(FadeCoroutine(1, 0));

            HideThought();
        }

        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
        {
            float elapsedTime = 0;
            Color textColor = thoughtText.color;
            Color bgColor = backgroundImage.color;

            while (elapsedTime < fadeDuration)
            {
                UpdateRotation();
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / fadeDuration;
                float evaluatedTime = fadeCurve.Evaluate(normalizedTime);
                float alpha = Mathf.Lerp(startAlpha, endAlpha, evaluatedTime);

                textColor.a = alpha;
                bgColor.a = alpha * 0.5f; // Фон немного прозрачнее

                thoughtText.color = textColor;
                backgroundImage.color = bgColor;

                yield return null;
            }
        }

        private void UpdateRotation()
        {
            if (_cameraTransform != null)
            {
                transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward,
                    _cameraTransform.rotation * Vector3.up);
            }
        }

        private void HideThought()
        {
            thoughtText.text = "";
            Color textColor = thoughtText.color;
            Color bgColor = backgroundImage.color;
            textColor.a = 0;
            bgColor.a = 0;
            thoughtText.color = textColor;
            backgroundImage.color = bgColor;
        }

        private void OnDisable()
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
            }
        }
    }
}