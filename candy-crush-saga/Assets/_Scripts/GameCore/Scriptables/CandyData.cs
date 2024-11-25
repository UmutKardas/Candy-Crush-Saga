using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.Scriptables
{
    [CreateAssetMenu(fileName = "CandyData", menuName = "Scriptable Objects/CandyData")]
    public class CandyData : ScriptableObject
    {
        public Candy[] candies;


        public CandyType GetRandomCandyType()
        {
            var randomIndex = UnityEngine.Random.Range(0, candies.Length);
            return candies[randomIndex].type;
        }
    }

    [Serializable]
    public struct Candy
    {
        public string name;
        public AssetReference sprite;
        public CandyType type;
    }

    public enum CandyType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Orange
    }
}
