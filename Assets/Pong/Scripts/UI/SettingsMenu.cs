using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

	public Text soundVolumeText,musicVolumeText;
	public Dropdown controllerSelectionDropdown;
	private float deltaSound, deltaMusic;

	[Space(10)]
	public Slider SoundSlider, MusicSlider;

	void Awake()
	{
		float _music = AudioManager.Audio.GetMusicVolume;

		SoundSlider.value = AudioManager.Audio.GetSoundVolume;
		MusicSlider.value = _music * (1 / AudioManager.MaxMusic);

		soundVolumeText.text = ((int)(AudioManager.Audio.soundSource.volume*100)).ToString()+"%";
		musicVolumeText.text = ((int)(MusicSlider.value * 100)).ToString()+"%";
		controllerSelectionDropdown.value =PlayerPrefs.GetInt ("Input");

		// SoundSlider.AddListener.OnValueChanged(OnSoundUpdate);
		// MusicSlider.AddListener.OnValueChanged(OnMusicUpdate);
	}

	public void OnSoundUpdate(float value)
	{
		AudioManager.Audio.SetSoundFxVolume (value);
		soundVolumeText.text = ((int)(AudioManager.Audio.soundSource.volume*100)).ToString()+"%";
		AudioManager.Audio.PlayClickSound ();
	}

	public void OnMusicUpdate(float value)
	{
		AudioManager.Audio.SetSoundMusicVolume (value);
		musicVolumeText.text = ((int)(AudioManager.Audio.musicSource.volume*100)).ToString()+"%";
		AudioManager.Audio.PlayClickSound ();
	}

	public void IncrementSound()
	{
		AudioManager.Audio.SetSoundFxVolume (0.1f);
		soundVolumeText.text = ((int)(AudioManager.Audio.soundSource.volume*100)).ToString()+"%";
		AudioManager.Audio.PlayClickSound ();
		SoundSlider.value = AudioManager.Audio.GetSoundVolume;
	}

	public void DecrementSound()
	{
		AudioManager.Audio.SetSoundFxVolume (-0.1f);
		soundVolumeText.text = ((int)(AudioManager.Audio.soundSource.volume*100)).ToString()+"%";
		AudioManager.Audio.PlayClickSound ();
		SoundSlider.value = AudioManager.Audio.GetSoundVolume;
	}

	public void IncrementMusic()
	{
		if (MusicSlider.value <= 0.9)
		{
			AudioManager.Audio.SetSoundMusicVolume(AudioManager.MaxMusic / 10);
			MusicSlider.value += 0.1f;
			musicVolumeText.text = ((int)(MusicSlider.value * 100)).ToString() + "%";
			AudioManager.Audio.PlayClickSound();
        }else
        {
			AudioManager.Audio.SetSoundMusicVolume(AudioManager.MaxMusic,true);
			MusicSlider.value += 0.1f;
			musicVolumeText.text = ((int)(MusicSlider.value * 100)).ToString() + "%";
			AudioManager.Audio.PlayClickSound();
		}
	}

	public void DecrementMusic()
	{
		if (MusicSlider.value >= 0.1)
		{
			AudioManager.Audio.SetSoundMusicVolume(-AudioManager.MaxMusic/10);
			MusicSlider.value -= 0.1f;
			musicVolumeText.text = ((int)(MusicSlider.value * 100)).ToString() + "%";
			AudioManager.Audio.PlayClickSound();
		}else
        {
			AudioManager.Audio.SetSoundMusicVolume(0,true);
			MusicSlider.value -= 0.1f;
			musicVolumeText.text = ((int)(MusicSlider.value * 100)).ToString() + "%";
			AudioManager.Audio.PlayClickSound();
		}

	}

	public void InputMethodChanged()
	{
		if (controllerSelectionDropdown.value == 0)
			PlayerPrefs.SetInt ("Input",0);
		else if (controllerSelectionDropdown.value == 1)
			PlayerPrefs.SetInt ("Input",1);
		else if (controllerSelectionDropdown.value == 2)
			PlayerPrefs.SetInt ("Input",2);

		Managers.Input.inputType = (InputMethod) PlayerPrefs.GetInt ("Input");
		AudioManager.Audio.PlayClickSound ();
	}

}
