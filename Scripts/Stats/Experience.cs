using Newtonsoft.Json.Linq;
using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        public event Action OnExperienceGain;
        [SerializeField]private float experiencePoints = 0;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            OnExperienceGain();
        }

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        #region IJsonSaveable
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }
        #endregion
    }
}