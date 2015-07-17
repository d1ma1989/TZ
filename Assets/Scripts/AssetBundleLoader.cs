using UnityEngine;

using System;
using System.Collections;

public class AssetBundleLoader : MonoBehaviour {
	[SerializeField] private string _path;

    public event Action Loaded;

	public static AssetBundle LoadedBundle { get; private set; }

	public IEnumerator DownloadAndCache() {
		// Wait for the Caching system to be ready
		while (!Caching.ready)
			yield return null;

		// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
		using (WWW www = WWW.LoadFromCacheOrDownload(_path, 0)) {
			yield return www;

			if (www.error != null) {
				throw new Exception("WWW download had an error:" + www.error);
			}

			LoadedBundle = www.assetBundle;

			// Frees the memory from the stream
			www.Dispose();
			
			if (Loaded != null) {
				Loaded();
			}
		}
	}
}
