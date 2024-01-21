using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health healthComponent;
        [SerializeField] private RectTransform forgroundTransform;

        private Canvas rootCanvas;

        private void Awake()
        {
            rootCanvas = GetComponentInChildren<Canvas>();
        }

        private void Update()
        {
            float healthFraction = healthComponent.GetFraction();
            if (Mathf.Approximately(healthFraction, 0) ||
                Mathf.Approximately(healthFraction, 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            forgroundTransform.localScale = new Vector3(healthFraction, 1, 1);
        }
    }
}