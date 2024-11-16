using System;
using System.Collections.Generic;
using UnityEngine;
using Addler.Runtime.Core.Pooling;
using Cysharp.Threading.Tasks;

namespace GameCore.Managers
{
    public class PoolManager : MonoBehaviour
    {
        #region Static Fields

        private static Dictionary<PoolType, AddressablePool> PoolsDictionary = new();

        #endregion

        #region Serialized Fields

        [SerializeField] private PoolConfig[] poolConfigs;

        #endregion

        #region Properties

        public UniTaskCompletionSource PoolCreationCompletionSource { get; } = new();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            InitializePools();
        }

        #endregion

        #region Private Methods

        private async void InitializePools()
        {
            foreach (var config in poolConfigs)
            {
                var pool = new AddressablePool(config.name).BindTo(gameObject);
                await pool.WarmupAsync(config.size);
                PoolsDictionary.Add(config.type, pool);
            }

            PoolCreationCompletionSource.TrySetResult();
        }

        #endregion

        #region Static Methods

        public static PooledObject GetPoolObject(PoolType poolType)
        {
            return PoolsDictionary.TryGetValue(poolType, out var pool) ? pool.Use() : null;
        }

        public static void ReturnPoolObject(PoolType poolType, PooledObject pooledObject)
        {
            if (PoolsDictionary.TryGetValue(poolType, out var pool)) { pool.Return(pooledObject); }
            else { Debug.LogError("Pool not found."); }
        }

        #endregion
    }

    [Serializable]
    public class PoolConfig
    {
        public PoolType type;
        public string name;
        public int size;
    }

    public enum PoolType
    {
        Candy
    }
}
