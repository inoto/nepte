using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{

    public UILabel label;

	// Use this for initialization
	void Awake ()
    {
        label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        int minutes = Mathf.FloorToInt(GameController.Instance.gameTimer / 60F);
        int seconds = Mathf.FloorToInt(GameController.Instance.gameTimer - minutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        label.text = formattedTime;
	}
}
