using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public UILabel label;

    float frameCount = 0;
    float nextUpdate = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

	// Use this for initialization
	void Awake()
	{
		label = GetComponent<UILabel>();
	}

    private void Start()
    {
        nextUpdate = Time.time;
    }

    // Update is called once per frame
    void Update()
	{
		frameCount++;
		if (Time.time > nextUpdate)
		{
			nextUpdate += 1.0f / updateRate;
			fps = frameCount * updateRate;
            label.text = fps.ToString();
			frameCount = 0;
		}
	}
}
