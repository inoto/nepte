using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{

    public UILabel label;

    public float timer;

	// Use this for initialization
	void Start ()
    {
        label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        label.text = formattedTime;
	}
}
