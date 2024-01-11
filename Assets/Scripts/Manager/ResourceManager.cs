using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dobrozaur.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Manager
{
    public ResourceManager()
    {
        
    }
    
    public async UniTaskVoid Init()
    {
        var r = await Addressables.InitializeAsync();
        
        foreach (var VARIABLE in r.Keys)
        {
            Debug.Log(VARIABLE);
        }

        
        // var g = await Addressables.LoadAssetAsync<StagesForm>(3);
        // var form = await Addressables.InstantiateAsync(g);
    }

    public async UniTask<T> LoadAssetAsync<T>(string path)
    {
        return await Addressables.LoadAssetAsync<T>(path);
    }

    public override void Initialize()
    {
        Init().Forget();
    }
}
