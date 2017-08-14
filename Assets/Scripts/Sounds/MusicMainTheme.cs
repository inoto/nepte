using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMainTheme : MonoBehaviour
{
    public float fadeInTime = 0;
    public float fadeOutTime = 0;

    private AudioSource audioSource;

	// Use this for initialization
	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void FadeIn()
    {
        
    }
}
