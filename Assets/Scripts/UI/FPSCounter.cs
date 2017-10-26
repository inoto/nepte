using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public UILabel Label;

	private float frameCount = 0;
	private float nextUpdate = 0.0f;
	private float fps = 0.0f;
	private const float updateRate = 4.0f; // 4 updates per sec.

	// Use this for initialization
	private void Awake()
	{
		Label = GetComponent<UILabel>();
	}

    private void Start()
    {
        nextUpdate = Time.time;
    }

    // Update is called once per frame
	private void Update()
	{
		frameCount++;
		if (Time.time > nextUpdate)
		{
			nextUpdate += 1.0f / updateRate;
			fps = frameCount * updateRate;
            Label.text = fps.ToString();
			frameCount = 0;
		}
	}
}
