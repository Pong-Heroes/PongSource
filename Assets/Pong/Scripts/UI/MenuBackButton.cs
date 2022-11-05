
using UnityEngine;
using System.Collections;

public class MenuBackButton : MonoBehaviour 
{
	public GameObject Panel;
	public void OnClickBackButton()
	{
		AudioManager.Audio.PlayClickSound ();
		Panel.SetActive (false);
		MainMenu.Instance.menuButtons.SetActive (true);
		PlayerPrefs.SetFloat ("Sound", AudioManager.Audio.soundSource.volume);
		PlayerPrefs.SetFloat ("Music", AudioManager.Audio.musicSource.volume);
	}
}
