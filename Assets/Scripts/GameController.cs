using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GUIText _timerText;

    [SerializeField]
    private GUIText _pointsText;

    //variables for copying loaded resources
    private GameObject _sphere;
    private AudioClip _backgroundMusic;
    private AudioClip _destroySound;
    private GameObject _explosionPs;

    private int _points;

    //speed coefficient
    private int _difficultyLevel = 1;

    private IEnumerator Start()
    {
        yield return StartCoroutine(LoadingResources());

        audio.clip = _backgroundMusic;
        audio.Play();

        _sphere.AddComponent<Sphere>();

        StartCoroutine(SpawningSpheres());
    }

    private void Update()
    {
        //timer
        _timerText.text = "Time: " + (int)Time.timeSinceLevelLoad;
    }

    private IEnumerator SpawningSpheres()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f / _difficultyLevel);
            CreateSphere();
        }
    }

    //create new sphere with random size and color at random position
    private void CreateSphere()
    {
        int randomSize = Random.Range(30, 101);
        int leftXpos = -300 + (int)(randomSize * 0.5f);
        int rightXpos = 300 - (int)(randomSize * 0.5f);
        int randomXpos = Random.Range(leftXpos, rightXpos);
        _sphere.transform.localScale = new Vector3(randomSize, randomSize, 1);
       
        GameObject newSphere = Instantiate(_sphere, new Vector3(randomXpos, 300, 500), Quaternion.identity) as GameObject;

        newSphere.renderer.material.color = MakeRandomColor();

        Sphere sphereComponent = newSphere.GetComponent<Sphere>();
        sphereComponent.CalculateSpeedAndPoints(randomSize, _difficultyLevel);

        //when circle is destroyed we made explositon effect
        sphereComponent.Destroyed += delegate(int points, Color color, Vector3 pos)
        {
            //play explosion sound
            AudioSource.PlayClipAtPoint(_destroySound, Vector3.zero);

            //instantiate gameObject with explosion particle system
            _explosionPs.particleSystem.startColor = color;
            GameObject explosion = Instantiate(_explosionPs, pos, Quaternion.identity) as GameObject;
            Destroy(explosion, 1f);

            //give points and increase game difficulty
            Points += points;
            _difficultyLevel = (Points / 100) + 1;
        };
    }

    private int Points
    {
        get { return _points; }

        set
        { 
            _points = value;
            _pointsText.text = "Points: " + _points;
        }
    }

    private static Color MakeRandomColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        Color randomColor = new Color(r, g, b);

        return randomColor;
    }

    //Loading objects from an AssetBundles asynchronously
    private IEnumerator LoadingResources()
    {
        AssetBundleRequest request = AssetBundleLoader.LoadedBundle.LoadAsync("ExplosionPs", typeof(GameObject));
        yield return request;
        _explosionPs = request.asset as GameObject;

        request = AssetBundleLoader.LoadedBundle.LoadAsync("Blast", typeof(AudioClip));
        yield return request;
        _destroySound = request.asset as AudioClip;

        request = AssetBundleLoader.LoadedBundle.LoadAsync("Sphere", typeof(GameObject));
        yield return request;
        _sphere = request.asset as GameObject;

        request = AssetBundleLoader.LoadedBundle.LoadAsync("Mario", typeof(AudioClip));
        yield return request;
        _backgroundMusic = request.asset as AudioClip;

        AssetBundleLoader.LoadedBundle.Unload(false);
    }
}
