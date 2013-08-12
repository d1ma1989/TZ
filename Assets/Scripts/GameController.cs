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

    private Material _sphereMaterial;

    private int _points;

    //speed coefficient
    private int _difficultyLevel = 1;

    private Color _currentColor;

    Texture2D[] _textures = new Texture2D[4];

    private IEnumerator Start()
    {
        yield return StartCoroutine(LoadingResources());

        _sphereMaterial = _sphere.renderer.material;

        audio.clip = _backgroundMusic;
        audio.Play();

        _sphere.AddComponent<Sphere>();

        CreateTextures();

        StartCoroutine(SpawningSpheres());
    }

    private void Update()
    {
        //timer
        _timerText.text = "Time: " + (int)Time.timeSinceLevelLoad;
    }

    //spawning spheres with delay depending on difficulty
    private IEnumerator SpawningSpheres()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f / _difficultyLevel);
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

        float xScale = _sphere.transform.localScale.x;

        if (xScale >= 30 && xScale < 40)
            _sphereMaterial.mainTexture = _textures[0];
        else if (xScale >= 40 && xScale < 60)
            _sphereMaterial.mainTexture = _textures[1];
        else if (xScale >= 60 && xScale < 80)
            _sphereMaterial.mainTexture = _textures[2];
        else if (xScale >= 80 && xScale < 100)
            _sphereMaterial.mainTexture = _textures[3];
       
        GameObject newSphere = Instantiate(_sphere, new Vector3(randomXpos, 300, 500), Quaternion.identity) as GameObject;
        print(newSphere.renderer.material.mainTexture.height);

        Sphere sphereComponent = newSphere.GetComponent<Sphere>();
        sphereComponent.CalculateSpeedAndPoints(randomSize, _difficultyLevel);

        //when circle is destroyed we made explositon effect
        sphereComponent.Destroyed += delegate(int points, Vector3 pos)
        {
            //play explosion sound
            AudioSource.PlayClipAtPoint(_destroySound, Vector3.zero);

            //instantiate gameObject with explosion particle system
            _explosionPs.particleSystem.startColor = _currentColor;
            GameObject explosion = Instantiate(_explosionPs, pos, Quaternion.identity) as GameObject;
            Destroy(explosion, 1f);

            //give points and increase game difficulty
            Points += points;

            int newDifficulty = Points / 100 + 1;
            if (newDifficulty > _difficultyLevel)
            {
                _difficultyLevel = newDifficulty;
                CreateTextures();
            }
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

    private void CreateTextures()
    {
        _currentColor = MakeRandomColor();
        print(_currentColor);

        _textures[0] = new Texture2D(32, 32, TextureFormat.ARGB32, false);
        _textures[1] = new Texture2D(64, 64, TextureFormat.ARGB32, false);
        _textures[2] = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        _textures[3] = new Texture2D(256, 256, TextureFormat.ARGB32, false);

        foreach (Texture2D texture in _textures)
        {
            int y = 0;
            while (y < texture.height)
            {
                int x = 0;
                while (x < texture.width)
                {
                    if (x == y || x + y == texture.height)
                        texture.SetPixel(x, y, Color.black);
                    else
                        texture.SetPixel(x, y, _currentColor);

                    x++;
                }
                y++;
            }
            texture.Apply();
        }
    }
}
