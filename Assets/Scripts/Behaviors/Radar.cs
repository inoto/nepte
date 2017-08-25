﻿using UnityEngine;

public class Radar : MonoBehaviour
{
    public bool showRadius = false;

    public float radius = 5;

	public Drone drone;

	// Use this for initialization
	void Awake ()
    {
		drone = GetComponent<Drone>();
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.yellow;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
            Gizmos.DrawWireSphere(drone.trans.position, radius);
		}
	}
}
