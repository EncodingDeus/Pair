using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Dobrozaur.Utility
{
    [Serializable]
    public class AssetReference<TObject> : AssetReferenceGameObject where TObject : Component
    {
        public AssetReference(string guid) : base(guid)
        {
        }

        public new async UniTask<TObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return (await base.InstantiateAsync(parent, instantiateInWorldSpace)).GetComponent<TObject>();
        }
    }
}
