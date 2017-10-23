using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionCircle
{
	public bool showGizmos;

	public int instanceId;

    public Transform trans;
    public Mover mover;
    public Owner owner;
	public Weapon weapon;
	public Base bas;

	
	public bool isInQT = false;
	public bool isDead = false;
    public bool isCollidedWithBase = false;
	public bool isStatic;
	public bool isWeapon;
	public CollisionCircle collidedBaseCircle = null;
	public int collidedCount = 0;
	

    public CollisionCircle(Transform _trans, Mover _mover, Owner _owner, Weapon _weapon)
    {
	    showGizmos = true;
        trans = _trans;
	    mover = _mover;
	    if (mover == null)
	    {
		    isStatic = true;
		    bas = trans.GetComponent<Base>();
	    }
	    owner = _owner;
	    weapon = _weapon;
	    if (weapon != null)
	    {
		    isWeapon = true;
	    }
	    isCollidedWithBase = false;
	    collidedBaseCircle = null;
	    collidedCount = 0;
	    instanceId = trans.gameObject.GetInstanceID();
    }
	public CollisionCircle(Radar _radar, Transform _trans, Mover _mover, Owner _owner)
	{
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
		if (isWeapon)
			return weapon.radius;
		else if (isStatic)
			return bas != null ? bas.collider.radius : 0;
		else
			return 0;
	}

	public void Collided(CollisionCircle other)
	{
		if (instanceId == other.instanceId)
			return;
		if (owner.playerNumber == other.owner.playerNumber)
			return;
		if (isDead)
			return;
		if (!isWeapon)
			return;
		if (isWeapon && !mover.weapon.hasTarget)
		{
			mover.weapon.target = other.trans.GetComponent<ITargetable>();
			mover.weapon.hasTarget = true;
		}
		else
		{
			mover.weapon.AttackTarget();
		}
	}

	public void CollidedEnded(CollisionCircle other)
	{
		if (instanceId == other.instanceId)
			return;
		if (owner.playerNumber == other.owner.playerNumber)
			return;
		if (isDead)
			return;
		if (!isWeapon)
			return;
		if (mover.weapon.target != null)
		{
			if (mover.weapon.target.GameObj == other.trans.gameObject)
				mover.weapon.target = null;
		}
	}

	public void DrawGizmos()
	{
		if (showGizmos)
		{
			if (isInQT)
				Gizmos.color = Color.green;
			else
				Gizmos.color = Color.red;
			Gizmos.DrawSphere(trans.position, 0.1f);
		}
	}
}
