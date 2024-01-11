// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using UnityEditor;
// using UnityEngine;
// using FileMode = UnityEditor.VersionControl.FileMode;
//
// class CustomAssetModificationProcessor : UnityEditor.AssetModificationProcessor
// {
//     private static bool CanOpenForEdit(string[] paths, List<string> outNotEditablePaths, StatusQueryOptions statusQueryOptions)
//     {
//         Debug.Log("CanOpenForEdit:");
//         foreach (var path in paths)
//             Debug.Log(path);
//         return true;
//     }
//     
//     private static void FileModeChanged(string[] paths, FileMode mode)
//     {
//         Debug.Log($"{nameof(FileModeChanged)} ({mode}):");
//         foreach (var path in paths)
//             Debug.Log(path);
//     }
//     
//     private static bool IsOpenForEdit(string[] paths, List<string> outNotEditablePaths, StatusQueryOptions statusQueryOptions)
//     {
//         Debug.Log("IsOpenForEdit:");
//         foreach (var path in paths)
//             Debug.Log(path);
//         return true;
//     }
//     
//     private static bool MakeEditable(string[] paths, string prompt, List<string> outNotEditablePaths)
//     {
//         Debug.Log("MakeEditable:");
//         foreach (var path in paths)
//             Debug.Log(path);
//         return true;
//     }
//     
//     private static void OnWillCreateAsset(string assetName)
//     {
//         Debug.Log("OnWillCreateAsset is being called with the following asset: " + assetName + ".");
//     }
//
//     private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
//     {
//         Debug.Log("OnWillDeleteAsset is being called with the following asset: " + assetPath + ", option: " + options + ".");
//         return AssetDeleteResult.DidNotDelete;
//     }
//     
//     private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
//     {
//         Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
//         AssetMoveResult assetMoveResult = AssetMoveResult.DidMove;
//
//         // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.
//
//         return assetMoveResult;
//     }
//     
//     private static string[] OnWillSaveAssets(string[] paths)
//     {
//         Debug.Log("OnWillSaveAssets");
//         foreach (string path in paths)
//             Debug.Log(path);
//         return paths;
//     }
// }
