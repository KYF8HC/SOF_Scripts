using RPG.Attributes;
using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDispaly : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI healthValueText = null;
        private Health health;
        private Fighter playerFighter;

        private void Awake ()
        {
            playerFighter = FindObjectOfType<PlayerTag>().GetComponent<Fighter>();
        }

        private void Update()
        {
            if (playerFighter.GetTarget() == null)
            {
                healthValueText.text = "N/A";
                return;
            }
            health = playerFighter.GetTarget();
            healthValueText.text = string.Format("{0:0}/{1:0}", health.GetHealth(), health.GetMaxHealth());
        }
    }
}