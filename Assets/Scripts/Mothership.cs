using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Mothership : MonoBehaviour, ITargetable
{
	public static ConfigMothership config;
	
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
	    weapon = GetComponent<Weapon>();
	    assignedCircle = GameObject.Find("MothershipCircle");
	    config = ConfigManager.Instance.Mothership;
    }

	public void DelayedStart()
    {
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
		
		LoadFromConfig();
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
			float dist = desired.magnitude;
			desired.Normalize();

			if (dist < bas.collider.radius)
			{
				mode = Mode.MovingAround;
			}
			else
			{
				Vector2 force = desired - velocity;
				force = Mover.LimitVector(force, 2);
				transform.position += (Vector3)force * Time.deltaTime * speed;
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
		speed = config.SpeedMax;
		radius = config.MovingAroundRadius;
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
		QuadMesh qm = GetComponent<QuadMesh>();
		if (qm != null)
		{
			float size = qm.size * 1.5f;
			explosion.transform.localScale = new Vector3(size, size, 1);
		}
	}

	void AssignMaterial()
	{
		if (mesh != null && owner != null)
			mesh.sharedMaterial = materials[owner.playerNumber];
		else
			Debug.LogError("Cannot assign material");
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
