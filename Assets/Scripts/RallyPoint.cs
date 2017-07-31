using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public int owner = 0;

    private Vector3 mousePosition;
	
	// Update is called once per frame
	void Update ()
    {
		if (Input.GetMouseButtonDown(0))
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
