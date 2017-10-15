using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Mothership : MonoBehaviour, ITargetable
{
	public static ConfigDrone config;
	
    public enum Mode
    {
        Idle,
        MovingAround,
	    MovingNewBase,
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
	public Weapon weapon;

	[Header("Cache")]
	public Base bas;
	public Base basTarget;
	public GameObject assignedCircle;
	public float angle = 0;
	public float speed = 0.3f;
	public float radius = 2.5f;
	public Vector2 velocity = Vector2.zero;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        owner = GetComponent<Owner>();
//        body = GetComponent<Body>();
//        radar = GetComponent<Radar>();
	    weapon = GetComponent<Weapon>();
	    assignedCircle = GameObject.Find("MothershipCircle");
	    //config = ConfigManager.Instance.Drone;
    }

//	private void Start()
//	{
//		Debug.Log("start");
//	}

	public void DelayedStart()
    {
        //SetOwnerAsInParent();
        AssignMaterial();
        owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	private void OnEnable()
	{
		//mode = Mode.Idle;
		collision = new CollisionCircle(trans, null, owner, null);
		CollisionManager.Instance.AddCollidable(collision);
		collision.isDead = false;
		collision.collidedBaseCircle = null;
		
//		LoadFromConfig();
//		Debug.Log("on enable");
	}

	private void Update()
	{
		if (mode == Mode.MovingAround)
		{
			angle += Time.deltaTime;
			var x = Mathf.Cos(angle * speed) * radius;
			var y = Mathf.Sin(angle * speed) * radius;
			transform.position = new Vector3(x, y, 0) + bas.transform.position;
		}
		else if (mode == Mode.MovingNewBase)
		{
			Vector2 desired = bas.transform.position - trans.position;

			/* Get the distance to the target */
			float dist = desired.magnitude;

			desired.Normalize();

			/* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
			//float targetSpeed;
			//float currentSpeed = 0;
			if (dist < bas.collider.radius)
			{
				mode = Mode.MovingAround;
			}
			else
			{
				Vector2 force = desired - velocity;
				force = Mover.LimitVector(force, 2);
				transform.position += (Vector3)force * Time.deltaTime;
			}
		}
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

	public void Die()
	{
		mode = Mode.Dead;
		collision.isDead = true;
		weapon.collision.isDead = true;
//		if (collision.collidedBaseCircle != null)
//		{
//			Capture capture = collision.collidedBaseCircle.trans.GetComponent<Capture>();
//			capture.RemoveCapturerByPlayer(owner.playerNumber);
//			capture.bas.collision.collidedCount--;
//			collision.collidedBaseCircle = null;
//		}
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
		if (GetComponent<QuadMesh>() != null)
		{
			float size = GetComponent<QuadMesh>().size * 1.5f;
			explosion.transform.localScale = new Vector3(size, size, 1);
		}
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
