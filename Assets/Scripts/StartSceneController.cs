using UnityEngine;

using System.Collections;

public class StartSceneController : MonoBehaviour {
	[SerializeField] private AssetBundleLoader _bundleLoader;
	[SerializeField] private GUIText _loadingText;
	[SerializeField] private GameObject _spaceBackgroundGO;

	private bool _bundlesLoaded;

	private void Start() {
		DontDestroyOnLoad(_spaceBackgroundGO);
		DontDestroyOnLoad(Camera.main);
		_bundleLoader.Loaded += () => {
			_loadingText.text = "Press space to start";
			_bundlesLoaded = true;
			StartCoroutine(BlinkingText());
		};

		StartCoroutine(_bundleLoader.DownloadAndCache());
	}

	private IEnumerator BlinkingText()
	{
		GameObject textGobj = _loadingText.gameObject;
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			textGobj.SetActive(!textGobj.activeInHierarchy);
		}
	}

	private void Update() {
		if (_bundlesLoaded && Input.GetKeyUp(KeyCode.Space)) {
			Application.LoadLevel("PlayScene");
		}
	}
}
