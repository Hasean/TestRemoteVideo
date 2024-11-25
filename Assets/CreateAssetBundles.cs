using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
#if UNITY_EDITOR
    [MenuItem("Prototype/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";

        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);

        BuildPipeline.BuildAssetBundles(
            assetBundleDirectory,
            BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle,
            BuildTarget.Android
        );
    }
    
    [MenuItem("Prototype/Open Persistent Data Folder")]
    public static void OpenPersistentDataFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
#endif
}