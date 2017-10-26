using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MixerContoller : MonoBehaviour
{
    public AudioMixer MasterMixer;

    public AudioSource AudioSource;

    public float VarVolume = 0.0f;

    public AudioClip MainThemeClip;
    public AudioClip[] AudioClips;
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
			if (VarVolume < 1.0f)
			{
				VarVolume += 0.3f * Time.deltaTime;
				AudioSource.volume = VarVolume;
			}
			else
			{
				clipAttached = false;
			}
		}
        else if (clipDetached)
        {
            if (VarVolume > 0.0f)

            {
                VarVolume -= 0.3f * Time.deltaTime;
                AudioSource.volume = VarVolume;
            }
            else
            {
                clipDetached = false;
                AudioSource.Stop();
                AudioSource.clip = null;
            }
        }
    }

	private IEnumerator CheckMusic()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);
            if (AudioSource.clip == null)
            {
                PlayNextClip();
            }
        }
    }

	private void PlayNextClip()
	{
        // to begin of clip array
        if (lastPlayedClip == AudioClips.Length - 1)
        {
            lastPlayedClip = 0;
        }
        else
        {
            lastPlayedClip += 1;
        }
        AudioSource.clip = AudioClips[lastPlayedClip];
		AudioSource.volume = 0.0f;
		clipAttached = true;
		AudioSource.Play();
	}

	private void PlayMainTheme()
	{
        AudioSource.volume = 0.0f;
        clipDetached = false;
        clipAttached = true;
        AudioSource.clip = MainThemeClip;
        AudioSource.Play();
	}

	public void StopMainTheme()
	{
        clipAttached = false;
        clipDetached = true;
        // start track same time
		AudioSource.clip = AudioClips[lastPlayedClip];
		AudioSource.volume = 0.0f;
		clipAttached = true;
		AudioSource.Play();

        StartCoroutine(CheckMusic());
	}

    public void SetMusicVolume(float newValue)
    {
        MasterMixer.SetFloat("musicVolume", LinearToDecibel(newValue));
    }

	public void SetSoundsVolume(float newValue)
	{
        MasterMixer.SetFloat("soundsVolume", LinearToDecibel(newValue));
	}

    private static float LinearToDecibel(float newValue)
    {
		float dB;
		if (newValue != 0.0f)
		{
			dB = 20.0f * Mathf.Log10(newValue);
		}
		else
		{
			dB = -80.0f;
		}
	    return dB;
    }


}
