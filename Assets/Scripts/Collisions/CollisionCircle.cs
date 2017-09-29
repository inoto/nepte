using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionCircle
{
//    public enum CollisionType
//    {
//        Body,
//        Radar,
//        Weapon
//    }
//    public CollisionType collisionType;
	public int instanceId;

//    public Body body;
//    public Radar radar;
//    public Weapon weapon;

    public Transform trans;
    public Mover mover;
    public Owner owner;
	public Weapon weapon;

	public bool isDead = false;
    public bool isCollidedWithBase = false;
	public bool isStatic;
	public CollisionCircle collidedBaseCircle = null;
	public int collidedCount = 0;

    public CollisionCircle(Transform _trans, Mover _mover, Owner _owner, Weapon _weapon)
    {
//        body = _body;
//        collisionType = CollisionType.Body;
        trans = _trans;
	    mover = _mover;
	    if (mover == null)
		    isStatic = true;
        owner = _owner;
	    weapon = _weapon;
	    isCollidedWithBase = false;
	    collidedBaseCircle = null;
	    collidedCount = 0;
	    instanceId = trans.gameObject.GetInstanceID();
    }
	public CollisionCircle(Radar _radar, Transform _trans, Mover _mover, Owner _owner)
	{
//		radar = _radar;
//        collisionType = CollisionType.Radar;
		trans = _trans;
		mover = _mover;
		owner = _owner;
		isCollidedWithBase = false;
		collidedBaseCircle = null;
		collidedCount = 0;
		instanceId = trans.gameObject.GetInstanceID();
	}
	public CollisionCircle(Weapon _weapon, Transform _trans, Mover _mover, Owner _owner)
	{
//		weapon = _weapon;
//        collisionType = CollisionType.Weapon;
		trans = _trans;
		mover = _mover;
		owner = _owner;
		isCollidedWithBase = false;
		collidedBaseCircle = null;
		collidedCount = 0;
		instanceId = trans.gameObject.GetInstanceID();
	}

	public float GetRadius()
	{
//		switch (collisionType)
//		{
//			case CollisionType.Body:
//				return body.radius;
//			case CollisionType.Radar:
//				return radar.radius;
//			case CollisionType.Weapon:
//				return weapon.radius;
//		}
		if (weapon != null)
			return weapon.radius;
		else
			return 0;
	}

	public void Collided(CollisionCircle other)
	{
		
	}
}
