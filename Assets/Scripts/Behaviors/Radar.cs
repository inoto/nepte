﻿using System;
using UnityEngine;

[Obsolete("Not used anymore",true)]
public class Radar : MonoBehaviour
{
    public bool showRadius = false;

    public float radius = 5;

	[Header("Components")]
	public Transform trans;
    public Owner owner;
    public Mover mover;
	public Weapon weapon;
    public CollisionCircle collision;

	void Awake ()
    {
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	    weapon = GetComponent<Weapon>();
    }

	public void ActivateCombat()
	{
		
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.yellow;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
            Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
}
