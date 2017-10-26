using UnityEngine;

public class GameTimer : MonoBehaviour
{

    public UILabel Label;

	// Use this for initialization
	private void Awake ()
    {
        Label = GetComponent<UILabel>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        int minutes = Mathf.FloorToInt(GameManager.Instance.GameTimer / 60F);
        int seconds = Mathf.FloorToInt(GameManager.Instance.GameTimer - minutes * 60);
        string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        Label.text = formattedTime;
	}
}
