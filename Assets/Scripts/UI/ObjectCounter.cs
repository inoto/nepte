using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCounter : MonoBehaviour
{
	public UILabel label;

	// Use this for initialization
	void Awake()
	{
		label = GetComponent<UILabel>();
	}

	// Update is called once per frame
	void Update()
	{
        label.text = PlayerController.unitCount.ToString();
	}
}
