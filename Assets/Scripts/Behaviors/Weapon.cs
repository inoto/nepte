using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	private bool isStarted = false;
	public bool showRadius = false;
	public bool isEnabled = true;
	public bool isAttacking = false;
	
    public float radius = 3.5f;
	public float attackSpeed = 1;
	public int damage = 40;
	public int damageNoBonuses = 0;
	public ITargetable target;
	public bool hasTarget;

	public GameObject missilePrefab;
	public string missilePrefabName;
	
	[Header("Components")]
    public Transform trans;
    public Owner owner;
    public Mover mover;
    public CollisionCircle collision;

	// Use this for initialization
	void Awake()
	{
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
		
		missilePrefab = Resources.Load<GameObject>(missilePrefabName);
	}

	private void Start()
	{
		collision = new CollisionCircle(trans, mover, owner, this);
		CollisionManager.Instance.AddCollidable(collision);
		isAttacking = false;
		collision.isDead = false;
		collision.collidedBaseCircle = null;
		target = null;
		hasTarget = false;
		isStarted = true;
//		Debug.Log("start");
	}
	
	private void OnEnable()
	{
		if (isStarted)
		{
			CollisionManager.Instance.AddCollidable(collision);
			isAttacking = false;
			collision.isDead = false;
			collision.collidedBaseCircle = null;
			target = null;
			hasTarget = false;
		}
//		Debug.Log("on enable");
	}

	public void StopAttacking()
	{
		isAttacking = false;
	}

	public void AttackTarget()
	{
		if (target != null && !isAttacking)
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
		if (missilePrefab == null)
			return;
//		GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
		GameObject laserMissileObject = Instantiate(missilePrefab, trans.position, trans.rotation);
		laserMissileObject.transform.parent = GameController.Instance.transform;
		LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		//laserMissile.owner = owner;
		laserMissile.weapon = this;
		laserMissile.target = target;
	}

	public void AddDamage(int additionalDamage)
	{
		damage += additionalDamage;
	}
	
	public void RemoveDamage(int additionalDamage)
	{
		damage -= additionalDamage;
	}

	public void NewTarget(ITargetable newTarget)
	{
		target = newTarget;
		hasTarget = true;
	}

	public void EndCombat()
	{
		target = null;
		hasTarget = false;
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
