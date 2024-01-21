using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI healthValueText = null;
        private Health playerHealth;

        private void Awake()
        {
            playerHealth = FindObjectOfType<PlayerTag>().GetComponent<Health>();
        }

        private void Update()
        {
            healthValueText.text = string.Format("{0:0}/{1:0}", playerHealth.GetHealth(), playerHealth.GetMaxHealth());
        }
    }
}