#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        UnityEditor.BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", UnityEditor.BuildAssetBundleOptions.None, BuildTarget.N3DS);
    }
}
#endif