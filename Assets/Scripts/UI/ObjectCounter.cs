using UnityEngine;

public class ObjectCounter : MonoBehaviour
{
	public UILabel Label;

	// Use this for initialization
	private void Awake()
	{
		Label = GetComponent<UILabel>();
	}

	// Update is called once per frame
	private void Update()
	{
        Label.text = PlayerController.UnitCount.ToString();
	}
}
