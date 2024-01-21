using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "ProgressionSO", menuName = "Stats/New Progression")]
    public class ProgressionSO : ScriptableObject
    {
        #region Classes
        [Serializable]
        private class ProgressionCharacterClass
        {
            [SerializeField] CharacterClass characterClass;
            [SerializeField] ProgressionStat[] stats;

            public CharacterClass GetClass() 
            { 
                return characterClass; 
            }
            public ProgressionStat[] GetStat()
            {
                return stats;
            }
        }

        [Serializable]
        private class ProgressionStat
        {
            [SerializeField] private Stat stat;
            [SerializeField] private float[] levels;

            public Stat GetProgressionStat()
            {
                return stat;
            }
            public float[] GetLevels()
            {
                return levels;
            }
        }
        #endregion


        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookUpTable = null;
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookUp();
            float[] levels = lookUpTable[characterClass][stat];
            if(levels.Length < level)
            {
                return 0;
            }
            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookUp();
            float[] levels = lookUpTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookUp()
        {
            if(lookUpTable != null)
            {
                return;
            }
            lookUpTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
            foreach(ProgressionCharacterClass progressionClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();
                foreach(ProgressionStat stat in progressionClass.GetStat())
                {
                    statLookupTable[stat.GetProgressionStat()] = stat.GetLevels();
                }
                lookUpTable[progressionClass.GetClass()] = statLookupTable;
            }
        }
    }
}