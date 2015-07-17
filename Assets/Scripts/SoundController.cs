using UnityEngine;

public class SoundController : MonoBehaviour {
	[SerializeField] private AudioSource _ambientMusicSource;
	[SerializeField] private AudioSource[] _effectsSources;

	public static SoundController I { get; private set; }

	private void Awake() {
		I = this;
		DontDestroyOnLoad(gameObject);
	}

	public void PlayAmbient(AudioClip clip) {
		_ambientMusicSource.clip = clip;
		_ambientMusicSource.loop = true;
		_ambientMusicSource.Play();
	}

	public void PlayEffect(AudioClip clip) {
		foreach (AudioSource audioSource in _effectsSources) {
			if (!audioSource.isPlaying) {
				audioSource.clip = clip;
				audioSource.Play();
				return;
			}
		}
		_effectsSources[0].clip = clip;
		_effectsSources[0].Play();
	}
}
