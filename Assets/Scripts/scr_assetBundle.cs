using UnityEngine;
using System.Collections;
using System.IO;

public class scr_assetBundle : MonoBehaviour
{

    public static scr_assetBundle m;
    void Awake()
    {
        m = this;
    }
    public Object[] LoadAsset(string bundleName)
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));

        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return null;
        }

        /*var names = assetBundle.GetAllAssetNames();
        for (int i=0; i!= names.Length; i++)
        {
            Debug.Log(names[i]);
        }*/
        return assetBundle.LoadAllAssets();
    }
}
