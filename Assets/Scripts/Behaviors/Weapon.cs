﻿using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public bool showRadius = false;

    public float radius = 3.5f;

	public Drone drone;

	// Use this for initialization
	void Awake()
	{
		drone = GetComponent<Drone>();
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(drone.trans.position, radius);
		}
	}
}
