using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Managers
{
    public static class AddressableManager
    {
        private static readonly Dictionary<object, UniTask<object>> LoadingTasks = new();
        private static readonly Dictionary<object, object> LoadedAssets = new();

        public static async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference) where T : class
        {
            var runtimeKey = assetReference.RuntimeKey;

            if (LoadedAssets.TryGetValue(runtimeKey, out var cachedAsset))
            {
                return cachedAsset as T;
            }

            if (LoadingTasks.TryGetValue(runtimeKey, out var ongoingTask))
            {
                return await ongoingTask as T;
            }

            var taskCompletion = new UniTaskCompletionSource<object>();
            LoadingTasks[runtimeKey] = taskCompletion.Task;

            try
            {
                var handle = assetReference.LoadAssetAsync<T>();
                await handle.Task;

                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Failed to load asset: {runtimeKey}");
                    return null;
                }

                LoadedAssets[runtimeKey] = handle.Result;
                taskCompletion.TrySetResult(handle.Result);
                return handle.Result;
            }
            finally
            {
                LoadingTasks.Remove(runtimeKey);
            }
        }

        public static void ReleaseAsset(AssetReference assetReference)
        {
            var runtimeKey = assetReference.RuntimeKey;

            if (!LoadedAssets.TryGetValue(runtimeKey, out var asset)) return;

            Addressables.Release(asset);
            LoadedAssets.Remove(runtimeKey);
        }

        public static void ReleaseAllAssets()
        {
            foreach (var asset in LoadedAssets.Values)
            {
                Addressables.Release(asset);
            }

            LoadedAssets.Clear();
        }
    }
}
