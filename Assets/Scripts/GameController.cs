using UnityEngine;

using System.Collections;

public class GameController : MonoBehaviour {
	[SerializeField] private GUIText _timerText;
	[SerializeField] private GUIText _pointsText;

	[SerializeField] private AudioClip _ambientMusicClip;

	private GameObject _sphere;
	private GameObject _explosionPs;

	private int _points;

	private int _difficultyLevel = 1;

	private Color _currentColor;

	private readonly Texture2D[] _textures = new Texture2D[4];

	private Camera _mainCamera;
	private Vector2 _topRightScreenPos;

	private IEnumerator Start() {
		_mainCamera = Camera.main;

		yield return StartCoroutine(LoadingResources());

		SoundController.I.PlayAmbient(_ambientMusicClip);

		_sphere.AddComponent<Sphere>();

		CreateTextures();

		_topRightScreenPos = new Vector2(Screen.width, Screen.height);
		StartCoroutine(SpawningSpheres());
		StartCoroutine(UpdateTimer());
	}

	private IEnumerator UpdateTimer() {
		while (true) {
			_timerText.text = string.Format("Time: {0}", (int)Time.timeSinceLevelLoad);
			yield return new WaitForSeconds(1f);
		}
	}

	private IEnumerator LoadingResources() {
		AssetBundleRequest request = AssetBundleLoader.LoadedBundle.LoadAssetAsync("ExplosionPs", typeof (GameObject));
		yield return request;
		_explosionPs = request.asset as GameObject;

		request = AssetBundleLoader.LoadedBundle.LoadAssetAsync("Sphere", typeof (GameObject));
		yield return request;
		_sphere = request.asset as GameObject;

		AssetBundleLoader.LoadedBundle.Unload(false);
	}

	private IEnumerator SpawningSpheres() {
		while (true) {
			yield return new WaitForSeconds(1f - _difficultyLevel * 0.05f );
			CreateSphere();
		}
	}

	private void CreateSphere() {
		int randomSize = Random.Range(30, 100);
		float halfSize = randomSize * 0.5f;
		_sphere.transform.localScale = Vector3.one * randomSize;

		float leftXpos = _mainCamera.ScreenToWorldPoint(Vector3.zero).x + halfSize;
		Vector3 screenToWorldPos = _mainCamera.ScreenToWorldPoint(_topRightScreenPos);
		float rightXpos = screenToWorldPos.x - halfSize;
		float yPos = screenToWorldPos.y + halfSize;
		float randomXpos = Random.Range(leftXpos, rightXpos);
		GameObject newSphere = Instantiate(_sphere, new Vector3(randomXpos, yPos, 500), Quaternion.identity) as GameObject;
		
		Material material = newSphere.GetComponent<Renderer>().material;
		material.shader = Shader.Find("Unlit/Texture");
		if (randomSize < 40) {
			material.mainTexture = _textures[0];
		} else if (randomSize < 60) {
			material.mainTexture = _textures[1];
		} else if (randomSize < 80) {
			material.mainTexture = _textures[2];
		} else {
			material.mainTexture = _textures[3];
		}

		Sphere sphere = newSphere.GetComponent<Sphere>();
		sphere.Color = _currentColor;
		sphere.CalculateSpeedAndPoints(randomSize, _difficultyLevel);
		sphere.Destroyed += OnSphereDestroyed;
	}

	private void OnSphereDestroyed(int points, Vector3 pos, Color color) {
		_explosionPs.GetComponent<ParticleSystem>().startColor = color;
		GameObject explosion = Instantiate(_explosionPs, pos, Quaternion.identity) as GameObject;
		Destroy(explosion, 1f);
		SoundController.I.PlayExplosion();

		ChangePoints(points);

		int newDifficultyLevel = _points / 100 + 1;
		if (_difficultyLevel != newDifficultyLevel) {
			_difficultyLevel = newDifficultyLevel;
			SoundController.I.PlayLevelRaised();
			CreateTextures();
		}
	}

	private void ChangePoints(int points) {
		_points += points;
		_pointsText.text = string.Format("Points: {0}", _points);
	}

	private void CreateTextures() {
		_currentColor = MakeRandomColor();

		_textures[0] = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		_textures[1] = new Texture2D(64, 64, TextureFormat.ARGB32, false);
		_textures[2] = new Texture2D(128, 128, TextureFormat.ARGB32, false);
		_textures[3] = new Texture2D(256, 256, TextureFormat.ARGB32, false);

		foreach (Texture2D texture in _textures) {
			int y = 0;
			while (y < texture.height) {
				int x = 0;
				while (x < texture.width) {
					if (x == y || x + y == texture.height) {
						texture.SetPixel(x, y, Color.black);
					} else {
						texture.SetPixel(x, y, _currentColor);
					}

					x++;
				}
				y++;
			}
			texture.Apply();
		}
	}

	private static Color MakeRandomColor() {
		const float min = 0.3f;
		const float max = 1f;
		float r = Random.Range(min, max);
		float g = Random.Range(min, max);
		float b = Random.Range(min, max);
		return new Color(r, g, b);
	}
}
