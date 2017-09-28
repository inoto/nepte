﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionCircle
{
    public enum CollisionType
    {
        Body,
        Radar,
        Weapon
    }
    public CollisionType collisionType;
	public int instanceId;

    public Body body;
    public Radar radar;
    public Weapon weapon;

    public Transform trans;
    public Mover mover;
    public Owner owner;

	public bool isDead = false;
    public bool isCollided = false;
	public CollisionCircle collidedBaseCircle = null;
	public int collidedCount = 0;

    public CollisionCircle(Body _body, Transform _trans, Mover _mover, Owner _owner)
    {
        body = _body;
        collisionType = CollisionType.Body;
        trans = _trans;
        mover = _mover;
        owner = _owner;
        isCollided = false;
	    collidedBaseCircle = null;
	    collidedCount = 0;
	    instanceId = trans.gameObject.GetInstanceID();
    }
	public CollisionCircle(Radar _radar, Transform _trans, Mover _mover, Owner _owner)
	{
		radar = _radar;
        collisionType = CollisionType.Radar;
		trans = _trans;
		mover = _mover;
		owner = _owner;
		isCollided = false;
		collidedBaseCircle = null;
		collidedCount = 0;
		instanceId = trans.gameObject.GetInstanceID();
	}
	public CollisionCircle(Weapon _weapon, Transform _trans, Mover _mover, Owner _owner)
	{
		weapon = _weapon;
        collisionType = CollisionType.Weapon;
		trans = _trans;
		mover = _mover;
		owner = _owner;
		isCollided = false;
		collidedBaseCircle = null;
		collidedCount = 0;
		instanceId = trans.gameObject.GetInstanceID();
	}

	public float GetRadius()
	{
		switch (collisionType)
		{
			case CollisionType.Body:
				return body.radius;
			case CollisionType.Radar:
				return radar.radius;
			case CollisionType.Weapon:
				return weapon.radius;
		}
		return 0;
	}

	public void Collided(CollisionCircle other)
	{
		
	}
}
