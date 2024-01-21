using RPG.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Inventory/Drop Library")]
public class DropLibrary : ScriptableObject
{
    [SerializeField] private DropConfig[] potentialDrops;
    [SerializeField] private float[] dropChancePercentage;
    [SerializeField] private int[] minDrop;
    [SerializeField] private int[] maxDrop;

    [Serializable]
    private class DropConfig
    {
        public InventoryItem item;
        public float[] relativeChance;
        public int[] minNumber;
        public int[] maxNumber;
        public int GetRandomNumber(int level)
        {
            if (!item.IsStackable())
            {
                return 1;
            }
            int min = GetByLevel(minNumber, level);
            int max = GetByLevel(maxNumber, level);
            return UnityEngine.Random.Range(min, max + 1);
        }
    }

    public struct Dropped
    {
        public InventoryItem item;
        public int number;
    }
    public IEnumerable<Dropped> GetRandomDrops(int level)
    {
        if (!ShouldRandomDrop(level))
        {
            yield break;
        }
        for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
        {
            yield return GetRandomDrop(level);
        }
    }

    private static T GetByLevel<T>(T[] values, int level)
    {
        if(values.Length == 0)
        {
            return default;
        }
        if (level > values.Length)
        {
            return values[values.Length - 1];
        }
        if( level <= 0)
        {
            return default;
        }
        return values[level - 1];
    }

    private bool ShouldRandomDrop(int level)
    {
        return UnityEngine.Random.Range(0, 100) < GetByLevel(dropChancePercentage, level);
    }

    private int GetRandomNumberOfDrops(int level)
    {
        return UnityEngine.Random.Range(GetByLevel(minDrop, level), GetByLevel(maxDrop, level));
    }

    private Dropped GetRandomDrop(int level)
    {
        var drop = SelectRandomItem(level);
        var result = new Dropped();
        result.item = drop.item;
        result.number = drop.GetRandomNumber(level);
        return result;
    }

    private DropConfig SelectRandomItem(int level)
    {
        float totalChance = GetTotalChance(level);
        float randomRoll = UnityEngine.Random.Range(0, totalChance);
        float chanceTotal = 0;
        foreach (var drop in potentialDrops)
        {
            chanceTotal += GetByLevel(drop.relativeChance, level);
            if(chanceTotal > randomRoll)
            {
                return drop;
            }
        }
        return null;
    }

    private float GetTotalChance(int level)
    {
        float totalChance = 0;
        foreach (var drop in potentialDrops)
        {
            totalChance += GetByLevel(drop.relativeChance, level);
        }
        return totalChance;
    }
}
