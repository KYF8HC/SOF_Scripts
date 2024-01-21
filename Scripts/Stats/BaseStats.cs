using RPG.Utils;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        public event Action OnLevelUp;
        [Range(0f, 99f)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] ProgressionSO progressionSO = null;
        [SerializeField] private GameObject levelUpParticleEffect;
        [SerializeField] private bool shouldUseModifiers = false;


        private LazyValue<int> currentLevel;
        private Experience experience;

        #region Unity default
        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }
        private void OnEnable()
        {
            if (experience != null)
            {
                experience.OnExperienceGain += Experience_OnExperienceGain;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.OnExperienceGain -= Experience_OnExperienceGain;
            }
        }
        #endregion

        #region Private

        private void Experience_OnExperienceGain()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                OnLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers)
            {
                return 0;
            }
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float totalModifier = 0;

            foreach (IModifierProvider provider in providers)
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    totalModifier += modifier;
                }
            }
            return totalModifier;
        }
        private float GetPercentageModifiers(Stat stat)
        {
            if (!shouldUseModifiers)
            {
                return 0;
            }
            IModifierProvider[] providers = GetComponents<IModifierProvider>();
            float totalModifier = 0;

            foreach (IModifierProvider provider in providers)
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    totalModifier += modifier;
                }
            }
            return totalModifier;
        }

        private float GetBaseStat(Stat stat)
        {
            return progressionSO.GetStat(stat, characterClass, GetLevel());
        }

        private int CalculateLevel()
        {
            if (experience == null)
            {
                return startingLevel;
            }
            float currentExperience = experience.GetExperiencePoints();
            int penultimateLevel = progressionSO.GetLevels(Stat.ExperienceToLevel, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float experienceToLevelUp = progressionSO.GetStat(Stat.ExperienceToLevel, characterClass, level);
                if (currentExperience < experienceToLevelUp)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
        #endregion

        #region Public

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        #endregion
    }
}