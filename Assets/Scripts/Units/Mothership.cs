using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Mothership : MonoBehaviour, ITargetable
{
	private static ConfigMothership _config;
	
    public enum MothershipMode
    {
        Idle,
        MovingAround,
	    MovingNewBase,
        Attacking,
        Dead
    }
	public MothershipMode Mode;

	private bool isStarted = false;
	[SerializeField] private GameObject explosionPrefab;

    [Header("Modules")]
    public Health Health;
	public CollisionCircle Collision;

    [Header("Components")]
    public Transform Trans;
    private MeshRenderer mesh;
    public Owner Owner;
	public Weapon Weapon;

	[Header("Cache")]
	public Planet Planet;
	public Planet TargetPlanet;
	public GameObject AssignedCircle;
	public float MoveSpeed = 3f;
	public float MoveRadius = 2.5f;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		Trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        Owner = GetComponent<Owner>();
	    Weapon = GetComponent<Weapon>();
	    AssignedCircle = GameObject.Find("MothershipCircle");
	    _config = ConfigManager.Instance.Mothership;
    }

	public void DelayedStart()
    {
        AssignMaterial();
        Owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	private void OnEnable()
	{
		//mode = Mode.Idle;
		Collision = new CollisionCircle(Trans, null, Owner, null);
		CollisionManager.Instance.AddCollidable(Collision);
		Collision.isDead = false;
		Collision.collidedBaseCircle = null;
		
		LoadFromConfig();
	}

	private void Start()
	{
		Trans.position += Vector3.right * MoveRadius;
	}

	private void Update()
	{
		if (Mode == MothershipMode.MovingAround)
		{
			var q = Trans.rotation;
			transform.RotateAround(Planet.transform.position, Vector3.forward, 20 * MoveSpeed * Time.deltaTime);
			transform.rotation = q;
		}
		else if (Mode == MothershipMode.MovingNewBase)
		{
			Vector2 desired = Planet.transform.position - Trans.position;
			float dist = desired.magnitude;
			desired.Normalize();

			if (dist < MoveRadius)
			{
				Mode = MothershipMode.MovingAround;
				var q = Trans.rotation;
				transform.RotateAround(Planet.transform.position, Vector3.forward, 20 *MoveSpeed * Time.deltaTime);
				transform.rotation = q;
			}
			else
			{
				transform.position += (Vector3)desired * Time.deltaTime * MoveSpeed;
			}
		}
	}

	private void LoadFromConfig()
	{
		if (_config == null)
		{
			return;
		}
		if (Health != null)
		{
			Health.max = _config.HealthMax;
			Health.current = Health.max;
		}
		MoveSpeed = _config.SpeedMax;
		MoveRadius = _config.MovingAroundRadius;
		if (Weapon != null)
		{
			Weapon.attackSpeed = _config.AttackSpeed;
			Weapon.damage = _config.AttackDamage;
			Weapon.damageNoBonuses = Weapon.damage;
			Weapon.radius = _config.AttackRadius;
			Weapon.missilePrefabName = _config.AttackMissilePrefabName;
			Weapon.missilePrefab = Resources.Load<GameObject>(Weapon.missilePrefabName);
		}
	}

	private void Die()
	{
		Mode = MothershipMode.Dead;
		Collision.isDead = true;
		Weapon.collision.isDead = true;

		if (Health.current <= 0)
		{
			MakeExplosion();
		}
		Owner.playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	private void MakeExplosion()
	{
		GameObject explosion = Instantiate(explosionPrefab, Trans.position, Trans.rotation);
		explosion.transform.parent = GameController.Instance.transform;
		QuadMesh qm = GetComponent<QuadMesh>();
		if (qm != null)
		{
			float size = qm.size * 1.5f;
			explosion.transform.localScale = new Vector3(size, size, 1);
		}
	}

	private void AssignMaterial()
	{
		if (mesh != null && Owner != null)
		{
			mesh.sharedMaterial = materials[Owner.playerNumber];
		}
		else
		{
			Debug.LogError("Cannot assign material");
		}
	}

	public void Damage(Weapon fromWeapon)
	{
		Health.current -= fromWeapon.damage;
		if (Health.current <= 0)
		{
			Die();
			fromWeapon.EndCombat();
		}
	}
	
	public void Damage(int damage)
	{
		Health.current -= damage;
		if (Health.current <= 0)
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
		get { return (Mode == MothershipMode.Dead); }
	}

	private void OnDrawGizmos()
	{
		if (Collision != null)
		{
			Collision.DrawGizmos();
		}
	}
}
