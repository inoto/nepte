using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	private bool isStarted = false;
	public bool ShowRadius = false;
	public bool IsEnabled = true;
	public bool IsAttacking = false;
	
    public float Radius = 3.5f;
	public float AttackSpeed = 1;
	public int Damage = 40;
	public int DamageNoBonuses = 0;
	public ITargetable Target;
	public bool HasTarget;

	public GameObject MissilePrefab;
	public string MissilePrefabName;
	
	[Header("Components")]
    public Transform Trans;
    public Owner Owner;
    public Mover Mover;
    public CollisionCircle Collision;

	private void Awake()
	{
		Trans = GetComponent<Transform>();
        Owner = GetComponent<Owner>();
        Mover = GetComponent<Mover>();
		
		MissilePrefab = Resources.Load<GameObject>(MissilePrefabName);
	}

	private void Start()
	{
		Collision = new CollisionCircle(Trans, Mover, Owner, this);
		CollisionManager.Instance.AddCollidable(Collision);
		IsAttacking = false;
		Collision.IsDead = false;
		Collision.CollidedBaseCircle = null;
		Target = null;
		HasTarget = false;
		isStarted = true;
	}
	
	private void OnEnable()
	{
		if (isStarted)
		{
			CollisionManager.Instance.AddCollidable(Collision);
			IsAttacking = false;
			Collision.IsDead = false;
			Collision.CollidedBaseCircle = null;
			Target = null;
			HasTarget = false;
		}
	}

	public void StopAttacking()
	{
		IsAttacking = false;
	}

	public void AttackTarget()
	{
		if (Target != null && !IsAttacking)
		{
			StartCoroutine(ReleaseMissileToTarget());
		}
	}

	private IEnumerator ReleaseMissileToTarget()
	{
		IsAttacking = true;
		if (Target != null)
		{
			ReleaseLaserMissile(Target.GameObj.transform.position);
		}
		yield return new WaitForSeconds(AttackSpeed);
		IsAttacking = false;
	}

	private void ReleaseLaserMissile(Vector3 newDestinationVector)
	{
		if (MissilePrefab == null)
		{
			return;
		}
//		GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
		GameObject laserMissileObject = Instantiate(MissilePrefab, Trans.position, Trans.rotation);
		laserMissileObject.transform.parent = GameManager.Instance.transform;
		LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.DestinationVector = newDestinationVector;
		//laserMissile.owner = owner;
		laserMissile.Weapon = this;
		laserMissile.Target = Target;
	}

	public void AddDamage(int additionalDamage)
	{
		Damage += additionalDamage;
	}
	
	public void RemoveDamage(int additionalDamage)
	{
		Damage -= additionalDamage;
	}

	public void NewTarget(ITargetable newTarget)
	{
		Target = newTarget;
		HasTarget = true;
	}

	public void EndCombat()
	{
		Target = null;
		HasTarget = false;
	}

	public void OnDrawGizmos()
	{
		if (ShowRadius)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(Trans.position, Radius);
		}
	}
}
