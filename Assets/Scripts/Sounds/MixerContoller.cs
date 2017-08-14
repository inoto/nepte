using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MixerContoller : MonoBehaviour
{


    public AudioMixer masterMixer;

    public AudioSource audioSource;

    public float varVolume = 0.0f;

    public AudioClip mainThemeClip;
    public AudioClip[] audioClips;
    private int lastPlayedClip = 0;

    private bool clipAttached;
    private bool clipDetached;

    private void Start()
    {
        PlayMainTheme();

    }

    private void Update()
    {
		if (clipAttached)
		{
			if (varVolume < 1.0f)
			{
				varVolume += 0.3f * Time.deltaTime;
				audioSource.volume = varVolume;
			}
			else
				clipAttached = false;
		}
        else if (clipDetached)
        {
            if (varVolume > 0.0f)

            {
                varVolume -= 0.3f * Time.deltaTime;
                audioSource.volume = varVolume;
            }
            else
            {
                clipDetached = false;
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
    }

    IEnumerator CheckMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);
            if (audioSource.clip == null)
            {
                PlayNextClip();
            }
                
        }
    }

    public void PlayNextClip()
	{
        // to begin of clip array
        if (lastPlayedClip == audioClips.Length - 1)
        {
            lastPlayedClip = 0;
        }
        else
        {
            lastPlayedClip += 1;
        }
        audioSource.clip = audioClips[lastPlayedClip];
		audioSource.volume = 0.0f;
		clipAttached = true;
		audioSource.Play();
	}

	public void PlayMainTheme()
	{
        audioSource.volume = 0.0f;
        clipDetached = false;
        clipAttached = true;
        audioSource.clip = mainThemeClip;
        audioSource.Play();
	}

	public void StopMainTheme()
	{
        clipAttached = false;
        clipDetached = true;
        // start track same time
		audioSource.clip = audioClips[lastPlayedClip];
		audioSource.volume = 0.0f;
		clipAttached = true;
		audioSource.Play();

        StartCoroutine(CheckMusic());
	}

    public void SetMusicVolume(float newValue)
    {
        masterMixer.SetFloat("musicVolume", LinearToDecibel(newValue));
    }

	public void SetSoundsVolume(float newValue)
	{
        masterMixer.SetFloat("soundsVolume", LinearToDecibel(newValue));
	}

    private float LinearToDecibel(float newValue)
    {
		float dB;
		if (newValue != 0.0f)
			dB = 20.0f * Mathf.Log10(newValue);
		else
			dB = -80.0f;
        return dB;
    }


}
