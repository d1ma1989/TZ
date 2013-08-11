using UnityEngine;
using System.Collections;

public class StartSceneController : MonoBehaviour
{
    [SerializeField]
    private AssetBundleLoader _bundleLoader;

    [SerializeField]
    private GUIText _loadingText;

    private bool _bundleIsLoaded;
	
	void Start() 
    {
	    _bundleLoader.Loaded += delegate
	    {
	        _loadingText.text = "Press any key to start";
	        StartCoroutine(BlinkingText());

            //after bundle is loaded we can start game
	        _bundleIsLoaded = true;
	    };
	}
	
	void Update()
	{
	    if (!_bundleIsLoaded)
	        return;

        //starting game by any key being down
        if (Input.anyKeyDown)
            Application.LoadLevel("Play");
	}

    //after loading bundle text on screen begins blinking
    private IEnumerator BlinkingText()
    {
        GameObject textGobj = _loadingText.gameObject;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);  
            textGobj.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            textGobj.SetActive(true);
        }
    }
}
