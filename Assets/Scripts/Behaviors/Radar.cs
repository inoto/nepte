﻿﻿using UnityEngine;
using System.Collections.Generic;

public class Radar : MonoBehaviour
{
    public bool showRadius = false;

    public float radius = 5;

	[Header("Components")]
	public Transform trans;
    public Owner owner;
    public Mover mover;
    public CollisionCircle collision;

	// Use this for initialization
	void Awake ()
    {
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	}

	private void Start()
	{
        collision = new CollisionCircle(this, trans, mover, owner);
		CollisionManager.Instance.AddCollidable(collision);
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
