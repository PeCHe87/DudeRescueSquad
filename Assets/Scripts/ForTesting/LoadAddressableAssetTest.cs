using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DudeResqueSquad
{
    public class LoadAddressableAssetTest : MonoBehaviour
    {
        [SerializeField] private Transform _pivot = null;
        [SerializeField] private AssetReference _assetReference;

        private GameObject _assetToInstantiate = null;

        public void LoadAsset()
        {
            _assetReference.LoadAssetAsync<GameObject>().Completed += OnLoadCompleted;
        }

        public void InstantiateAsset()
        {
            _assetReference.InstantiateAsync().Completed += OnInstantiationCompleted;
        }
        
        private void OnLoadCompleted(AsyncOperationHandle<GameObject> obj)
        {
            // In a production environment, you should add exception handling to catch scenarios such as a null result.
            var result = obj.Result;

            if (result == null)
            {
                return;
            }

            _assetToInstantiate = result;
            
            _assetReference.InstantiateAsync().Completed += OnInstantiationCompleted;
        }

        private void OnInstantiationCompleted(AsyncOperationHandle<GameObject> obj)
        {
            var resultTransform = obj.Result.transform;
            resultTransform.SetParent(_pivot);
            resultTransform.localPosition = Vector3.zero;
            resultTransform.localRotation = quaternion.identity;
        }
    }
}