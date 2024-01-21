using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]private TextMeshProUGUI damageText;

        public void SetText(float damageAmount)
        {
            damageText.text = string.Format("{0:0}", damageAmount);
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}