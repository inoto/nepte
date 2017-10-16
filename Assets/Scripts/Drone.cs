using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Drone : MonoBehaviour, ITargetable
{
	public static ConfigDrone config;
	
    public enum Mode
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }
	public Mode mode;

	private bool isStarted = false;
	public GameObject explosionPrefab;

    [Header("Modules")]
    public Health health;
	public CollisionCircle collision;

    [Header("Components")]
    public Transform trans;
    MeshRenderer mesh;
    public Owner owner;
    public Mover mover;
	public Weapon weapon;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	    weapon = GetComponent<Weapon>();
	    
	    config = ConfigManager.Instance.Drone;
    }

	public void DelayedStart()
    {
        AssignMaterial();
        mover.DelayedStart();
        owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	private void OnEnable()
	{
		mode = Drone.Mode.Idle;
		collision = new CollisionCircle(trans, mover, owner, null);
		CollisionManager.Instance.AddCollidable(collision);
		collision.isDead = false;
		collision.collidedBaseCircle = null;
		
		LoadFromConfig();
	}
	
	void LoadFromConfig()
	{
		if (config == null)
			return;
		if (health != null)
		{
			health.max = config.HealthMax;
			health.current = health.max;
		}
		if (mover != null)
		{
			mover.maxSpeed = config.SpeedMax;
			mover.maxForce = config.ForceMax;
			mover.turnSpeed = config.TurnSpeed;
			mover.followBase.attackRadius = ConfigManager.Instance.Base.ColliderRadius;
			mover.followBase.enterRadius = config.EnterBaseRadius;
			mover.separation.enabled = config.SeparationEnabled;
			mover.separation.desired = config.SeparationRadius;
			mover.cohesion.enabled = config.CohesionEnabled;
			mover.cohesion.desired = config.CohesionRadius;
		}
		if (weapon != null)
		{
			weapon.attackSpeed = config.AttackSpeed;
			weapon.damage = config.AttackDamage;
			weapon.damageNoBonuses = weapon.damage;
			weapon.radius = config.AttackRadius;
			weapon.missilePrefabName = config.AttackMissilePrefabName;
			weapon.missilePrefab = Resources.Load<GameObject>(weapon.missilePrefabName);
		}
	}

	public void PutIntoBase(Base _bas)
	{
		_bas.spawner.unitCount += 1;
		_bas.spawner.UpdateLabel();
		Die();
	}

	public void Die()
	{
		mode = Mode.Dead;
		collision.isDead = true;
		weapon.collision.isDead = true;

		if (health.current <= 0)
			MakeExplosion();
		owner.playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	void MakeExplosion()
	{
		GameObject explosion = Instantiate(explosionPrefab, trans.position, trans.rotation);
		explosion.transform.parent = GameController.Instance.transform;
		float size = GetComponent<QuadMesh>().size * 1.5f;
		explosion.transform.localScale = new Vector3(size, size, 1);
	}

	void AssignMaterial()
	{
		if (mesh != null && owner != null)
			mesh.sharedMaterial = materials[owner.playerNumber];
		else
			Debug.LogError("Cannot assign material.");
	}

	public void Damage(Weapon _weapon)
	{
		health.current -= _weapon.damage;
		if (health.current <= 0)
		{
			Die();
			_weapon.EndCombat();
		}
	}
	
	public void Damage(int damage)
	{
		health.current -= damage;
		if (health.current <= 0)
		{
			Die();
		}
	}

	public GameObject GameObj
	{
		get { return gameObject; }
	}

	public bool IsDied
	{
		get { return (mode == Mode.Dead); }
	}

	private void OnDrawGizmos()
	{
		if (collision != null)
			collision.DrawGizmos();
	}
}
