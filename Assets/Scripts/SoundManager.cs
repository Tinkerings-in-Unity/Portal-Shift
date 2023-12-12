using System.Collections;
using System.Collections.Generic;
using Code.Utility;
using UnityEngine;

// namespace Completed
// {
public enum Track {
	Menu,
	Main,
	GameOver
}

public enum FX {
	Click,
	PortalCreation,
	FromExit,
	IntoEntry,
	BlockBounce,
	Star,
	Switch,
	LevelClear,
	Cheers
}

public class SoundManager : MonoBehaviour {
	[SerializeField]
	AudioSource fxSource; //Drag a reference to the audio source which will play the sound effects.
	[SerializeField]
	AudioSource fxSource2;
	[SerializeField]
	AudioSource fxSource3;
	[SerializeField]
	AudioSource musicSource; //Drag a reference to the audio source which will play the music.
	public static SoundManager instance = null; //Allows other scripts to call functions from SoundManager.				
	[SerializeField]
	List<AudioClip> tracks;
	[SerializeField]
	AudioClip menu;
	[SerializeField]
	AudioClip gameOver;
	[SerializeField]
	AudioClip portalCreation;
	[SerializeField]
	AudioClip fromExit;
	[SerializeField]
	AudioClip intoEntry;
	[SerializeField]
	AudioClip blockBounce;
	[SerializeField]
	AudioClip star;
	[SerializeField]
	AudioClip portalSwitch;
	[SerializeField]
	AudioClip levelClear;
	[SerializeField]
	AudioClip cheers;
	[SerializeField]
	AudioClip click;

	void Awake () {
		//Check if there is already an instance of SoundManager
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);
	}

	//Used to play music.
	public void PlayMusic (Track track) {
		AudioClip clip = null;
		switch (track) {
			case Track.Menu:
				clip = menu;
				break;
			case Track.Main:
				clip = tracks[Random.Range (0, tracks.Count)];
				break;
			case Track.GameOver:
				clip = gameOver;
				break;
			default:
				break;
		}
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		musicSource.clip = clip;
		//Play the clip.
		musicSource.Play ();
	}

	//Used to play single sound clips.
	public void PlayFX (FX fx) {
		AudioClip clip = null;
		AudioSource source = null;
		switch (fx) {
			case FX.PortalCreation:
				clip = portalCreation;
				source = fxSource;
				break;
			case FX.FromExit:
				clip = fromExit;
				source = fxSource2;
				break;
			case FX.IntoEntry:
				clip = intoEntry;
				source = fxSource3;
				break;
			case FX.BlockBounce:
				clip = blockBounce;
				source = fxSource2;
				break;
			case FX.Star:
				clip = star;
				source = fxSource3;
				break;
			case FX.Switch:
				clip = portalSwitch;
				source = fxSource2;
				break;
			case FX.LevelClear:
				clip = levelClear;
				source = fxSource;
				break;
			case FX.Cheers:
				clip = cheers;
				source = fxSource2;
				break;
			case FX.Click:
				clip = click;
				source = fxSource;
				break;
			default:
				break;
		}
		//Set the clip of our efxSource audio source to the clip passed in as a parameter.
		source.clip = clip;
		//Play the clip.
		source.Play ();
	}

	public void SpeedChange (int value) {
		if (value > 0)
			musicSource.pitch += 0.1f;
		else if (value < 0)
			musicSource.pitch -= 0.1f;
		else
			musicSource.pitch = 1f;
	}
}
// }