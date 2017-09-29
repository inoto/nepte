﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public bool showRadius = false;

	public bool isAttacking = false;
	
    public float radius = 3.5f;
	public float attackSpeed = 1;
	public int damage = 40;
	public ITargetable target;
	public bool hasTarget;

	[SerializeField] private GameObject missilePrefab;
	
	[Header("Components")]
    public Transform trans;
    public Owner owner;
    public Mover mover;
//    public CollisionCircle collision;

	// Use this for initialization
	void Awake()
	{
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	}

//	private void Start()
//	{
//        collision = new CollisionCircle(this, trans, mover, owner);
//		CollisionManager.Instance.AddCollidable(collision);
//	}

	public void StopAttacking()
	{
		isAttacking = false;
	}

	public void AttackTarget()
	{
		if (target != null && !isAttacking)
			//if (mover.IsFacing(target.GameObj.transform.position, 120))
			StartCoroutine(ReleaseMissileToTarget());
		
	}

	IEnumerator ReleaseMissileToTarget()
	{
		isAttacking = true;
		if (target != null)
			ReleaseLaserMissile(target.GameObj.transform.position);
		yield return new WaitForSeconds(attackSpeed);
		isAttacking = false;
	}
	
	public void ReleaseLaserMissile(Vector3 newDestinationVector)
	{
//		GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
		GameObject laserMissileObject = Instantiate(missilePrefab, trans.position, trans.rotation);
		laserMissileObject.transform.parent = GameController.Instance.transform;
		LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		//laserMissile.owner = owner;
		laserMissile.weapon = this;
		laserMissile.target = target;
	}

	public void NewTarget(ITargetable newTarget)
	{
		target = newTarget;
		showRadius = true;
		mover.followRally.enabled = false;
		mover.followTarget.enabled = true;
	}

	public void EndCombat()
	{
		isAttacking = false;
		target = null;
		hasTarget = false;
		showRadius = false;
		mover.followRally.enabled = true;
		mover.followTarget.enabled = false;
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
}
