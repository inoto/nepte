using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public int owner = 0;

    private Vector3 mousePosition;

    public Vector3 prevPosition;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (owner == 0 && Input.GetMouseButtonDown(0))
		{
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
            if (transform.position != mousePosition)
            {
                transform.position = mousePosition;

            }
		}
	}
}
