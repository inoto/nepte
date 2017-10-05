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

	public GameObject explosionPrefab;

    [Header("Modules")]
    public Health health = new Health(100);
	public CollisionCircle collision;

    [Header("Components")]
    public Transform trans;
    MeshRenderer mesh;
    public Owner owner;
//    public Body body;
    public Mover mover;
//    public Radar radar;
	public Weapon weapon;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        owner = GetComponent<Owner>();
//        body = GetComponent<Body>();
        mover = GetComponent<Mover>();
//        radar = GetComponent<Radar>();
	    weapon = GetComponent<Weapon>();
	    
	    config = ConfigManager.Instance.Drone;
    }

	private void Start()
	{
		collision = new CollisionCircle(trans, mover, owner, null);
		CollisionManager.Instance.AddCollidable(collision);
		
		LoadFromConfig();
	}

	public void DelayedStart()
    {
        //SetOwnerAsInParent();
        AssignMaterial();
        mover.DelayedStart();
        owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	private void OnEnable()
	{
		mode = Drone.Mode.Idle;
		collision.isDead = false;
		collision.collidedBaseCircle = null;
		weapon.target = null;
//		Debug.Log("on enable");
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
			mover.followRally.stopRadius = config.FollowStopRadius;
			mover.separation.enabled = config.SeparationEnabled;
			mover.separation.desired = config.SeparationRadius;
			mover.cohesion.enabled = config.CohesionEnabled;
			mover.cohesion.desired = config.CohesionRadius;
		}
		if (weapon != null)
		{
			weapon.attackSpeed = config.AttackSpeed;
			weapon.damage = config.AttackDamage;
			weapon.radius = config.AttackRadius;
			weapon.missilePrefabName = config.AttackMissilePrefabName;
			weapon.missilePrefab = Resources.Load<GameObject>(weapon.missilePrefabName);
		}
	}

	public void PutIntoBase(Base _bas)
	{
		_bas.spawner.unitCount += 1;
		_bas.spawner.UpdateLabel();
		PutIntoBase();
	}

	public void PutIntoBase()
	{
//		Debug.Log("put into base");
		mode = Mode.Dead;
		collision.isDead = true;
		weapon.collision.isDead = true;
		if (collision.collidedBaseCircle != null)
		{
//			Debug.Log("unit put into base");
			Capture capture = collision.collidedBaseCircle.trans.GetComponent<Capture>();
			if (capture != null)
			{
				//capture.RemoveCapturerByPlayer(owner.playerNumber);
				capture.bas.collision.collidedCount--;
				collision.collidedBaseCircle = null;
			}
			
		}
		owner.playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	void Die()
	{
		mode = Mode.Dead;
		collision.isDead = true;
		weapon.collision.isDead = true;
		if (collision.collidedBaseCircle != null)
		{
			Capture capture = collision.collidedBaseCircle.trans.GetComponent<Capture>();
			capture.RemoveCapturerByPlayer(owner.playerNumber);
			capture.bas.collision.collidedCount--;
			collision.collidedBaseCircle = null;
		}
		
		MakeExplosion();
		owner.playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	void MakeExplosion()
	{
		GameObject explosion = Instantiate(explosionPrefab, trans.position, trans.rotation);
		explosion.transform.parent = GameController.Instance.transform;
	}

	void SetOwnerAsInParent()
	{
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
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
