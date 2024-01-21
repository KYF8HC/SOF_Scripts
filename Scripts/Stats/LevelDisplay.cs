using RPG.Core;
using RPG.Stats;
using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI experienceValueText = null;
    private BaseStats playerLevel;

    private void Awake()
    {
        playerLevel = FindObjectOfType<PlayerTag>().GetComponent<BaseStats>();
    }

    private void Update()
    {
        experienceValueText.text = playerLevel.GetLevel().ToString();
    }
}
