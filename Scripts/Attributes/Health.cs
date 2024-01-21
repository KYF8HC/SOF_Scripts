using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using RPG.Utils;
using UnityEngine.Events;
using System;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        [SerializeField] private UnityEvent onDie;
        [SerializeField] private TakeDamageEvent takeDamage;
        private LazyValue<float> healthPoints;
        private BaseStats stats;


        private void Awake()
        {
            stats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            //Force Initializing healthPoints if it has not been initialized before
            healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            stats.OnLevelUp += BaseStats_OnLevelUp;
        }

        private void OnDisable()
        {
            stats.OnLevelUp -= BaseStats_OnLevelUp;
        }

        #region Private
        private float GetInitialHealth()
        {
            return stats.GetStat(Stat.Health);
        }

        private void BaseStats_OnLevelUp()
        {
            healthPoints.value = stats.GetStat(Stat.Health);
        }

        private void UpdateState()
        {
            if (IsDead())
            {
                Die();
            }
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }
            experience.GainExperience(stats.GetStat(Stat.ExperienceReward));
        }
        #endregion

        #region Public

        public void Heal(float healthToRestore)
        {
            healthPoints.value += healthToRestore;
            float maxHealth = stats.GetStat(Stat.Health);
            if (healthPoints.value > maxHealth)
            {
                healthPoints.value = maxHealth;
            }
        }

        public void Die()
        {
            onDie.Invoke();
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            if (IsDead())
            {
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }
        public float GetHealthPercentage()
        {
            return GetFraction() * 100;
        }

        public float GetFraction()
        {
            return healthPoints.value / stats.GetStat(Stat.Health);
        }

        public float GetHealth()
        {
            return healthPoints.value;
        }

        public float GetMaxHealth()
        {
            return stats.GetStat(Stat.Health);
        }

        public bool IsDead()
        {
            return healthPoints.value <= 0;
        }
        #endregion

        #region IJsonSaveable

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(healthPoints.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            healthPoints.value = state.ToObject<float>();
            UpdateState();
        }

        #endregion
    }
}