using UnityEngine;

public class MusicMainTheme : MonoBehaviour
{
    public float FadeInTime = 0;
    public float FadeOutTime = 0;

    private AudioSource audioSource;

	private void Start ()
    {
        audioSource = GetComponent<AudioSource>();
	}
}
