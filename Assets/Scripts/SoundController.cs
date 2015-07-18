using UnityEngine;

public class SoundController : MonoBehaviour {
	[SerializeField] private AudioSource _ambientMusicSource;
	[SerializeField] private AudioSource _explosionAudioSource;
	[SerializeField] private AudioSource _levelRaisedAudioSource;

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

	public void PlayExplosion() {
		_explosionAudioSource.Play();
	}

	public void PlayLevelRaised() {
		_levelRaisedAudioSource.Play();
	}
}
