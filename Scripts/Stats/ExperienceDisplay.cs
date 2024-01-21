using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI experienceValueText = null;
        private Experience playerExperience;

        private void Awake()
        {
            playerExperience = FindObjectOfType<PlayerTag>().GetComponent<Experience>();
        }

        private void Update()
        {
            experienceValueText.text = playerExperience.GetExperiencePoints().ToString();
        }
    }
}