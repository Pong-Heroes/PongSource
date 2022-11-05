using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AudioManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource soundSource;

    public AudioClip gameMusic;
    public AudioClip loginScreenMusic;
    public AudioClip LobbyMusic;
    public AudioClip menuClickSound;
    public AudioClip hoverSound;
    public AudioClip collisionSound;
    public AudioClip countDownSound;
    public AudioClip loseSound;
    public AudioClip winSound;
    public AudioClip losePointSound;
    public AudioClip winPointSound;
    public AudioClip CyberpunkMap;
    public AudioClip forestMap;
    public AudioClip spaceMap;

    public static float MaxMusic = 0.2f; //0.2f
    public static float HalfMusic = 0.1f;
    public static float MaxSound = 0.5f;//1f

    private static AudioManager _audioManager;
    public static AudioManager Audio
    {
    	get { return _audioManager; }
    }
    private int soundIndex = 0;
    void Awake()
    {
        //PlayerPrefs.DeleteKey("Sound");
        //PlayerPrefs.DeleteKey("Music");

        if (!_audioManager)
        {
            _audioManager = this;
            DontDestroyOnLoad(this.gameObject);
        }else
        {
            Destroy(this.gameObject);
        }


        soundSource.volume = PlayerPrefs.GetFloat("Sound", MaxSound);
        musicSource.volume = PlayerPrefs.GetFloat("Music", HalfMusic);
    }

    public float GetSoundVolume => soundSource.volume;
    public float GetMusicVolume => musicSource.volume;

    #region Sound FX

    public void PlayLoseSound()
    {
        if (!soundSource)
            return;
        StopGameMusic();
        soundSource.clip = loseSound;
        soundSource.Play();
    }

    public void PlayClickSound()
    {
        if (!soundSource)
            return;

        soundSource.clip = menuClickSound;
        soundSource.Play();
    }

    public void PlayHoverSound()
    {
        if (!soundSource)
            return;

        if (!hoverSound)
            return;

        soundSource.clip = hoverSound;
        soundSource.Play();
    }

    public void PlayPointLoseSound()
    {
        if (!soundSource)
            return;
        soundSource.clip = losePointSound;
        soundSource.Play();
    }

    public void PlayPointWinSound()
    {
        if (!soundSource)
            return;
        soundSource.clip = winPointSound;
        soundSource.Play();
    }

    public void PlayWinSound()
    {
        if (!soundSource)
            return;
        StopGameMusic();
        soundSource.clip = winSound;
        soundSource.Play();
    }

    public void PlayCollisionSound()
    {
        if (!soundSource)
            return;

        soundSource.clip = collisionSound;
        soundSource.Play();
    }

    public void PlayCountSound()
    {
        if (!soundSource)
            return;

        soundSource.clip = countDownSound;
        soundSource.Play();
    }

    public void SetSoundFxVolume(float value)
    {
        float temp = Mathf.Clamp(value + soundSource.volume, 0, 1);
        soundSource.volume += value;
    }
    #endregion

    #region Music
    public void PlayCyberpunkMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = CyberpunkMap;
        musicSource.Play();
    }

    public void PlayForestMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = forestMap;
        musicSource.Play();
    }

    public void PlaySpaceMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = spaceMap;
        musicSource.Play();
    }

    public void PlayLoginMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = loginScreenMusic;
        musicSource.Play();
    }

    public void PlayLobbyMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = LobbyMusic;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (!musicSource)
            return;
        musicSource.clip = gameMusic;
        musicSource.Play();
    }

    public void StopGameMusic()
    {
        if (!musicSource)
            return;
        musicSource.Stop();
    }

    public void SetSoundMusicVolume(float value, bool setDirect = false)
    {
        if (!musicSource)
            return;

        if (!setDirect)
            musicSource.volume += value;
        else
            musicSource.volume = value;
    }
    #endregion

}
