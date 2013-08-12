using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class AssetBundleLoader : MonoBehaviour
{
    [SerializeField]
    private string _path;

    public event Action Loaded;

    private void Start()
    {
        StartCoroutine(DownloadAndCache());
    }

    private IEnumerator DownloadAndCache()
    {
        // Wait for the Caching system to be ready
        while (!Caching.ready)
            yield return null;

        // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
        using (WWW www = WWW.LoadFromCacheOrDownload(_path, 0))
        {
            yield return www;

            if (www.error != null)
                throw new Exception("WWW download had an error:" + www.error);
           
           LoadedBundle = www.assetBundle;

            // Frees the memory from the stream
            www.Dispose();

            //event is fired after loading asset bundle
            if (Loaded != null)
                Loaded();
        }
    }

    //auto-property to get loaded asset bundle from play scene
    public static AssetBundle LoadedBundle { get; private set; }
}
