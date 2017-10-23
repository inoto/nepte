using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Drone : MonoBehaviour, ITargetable
{
	private static ConfigDrone _config;
	
    public enum DroneMode
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }
	public DroneMode Mode;

	private bool isStarted = false;
	[SerializeField] private GameObject explosionPrefab;

    [Header("Modules")]
    public Health Health;
	public CollisionCircle Collision;

    [Header("Components")]
    public Transform Trans;
    private MeshRenderer mesh;
    public Owner Owner;
    public Mover Mover;
	public Weapon Weapon;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		Trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        Owner = GetComponent<Owner>();
        Mover = GetComponent<Mover>();
	    Weapon = GetComponent<Weapon>();
	    
	    _config = ConfigManager.Instance.Drone;
    }

	public void DelayedStart()
    {
        AssignMaterial();
        Mover.DelayedStart();
        Owner.playerController.playerUnitCount += 1;
        PlayerController.unitCount += 1;
    }

	private void OnEnable()
	{
		Mode = Drone.DroneMode.Idle;
		Collision = new CollisionCircle(Trans, Mover, Owner, null);
		CollisionManager.Instance.AddCollidable(Collision);
		Collision.isDead = false;
		Collision.collidedBaseCircle = null;
		
		LoadFromConfig();
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
		if (Mover != null)
		{
			Mover.maxSpeed = _config.SpeedMax;
			Mover.maxForce = _config.ForceMax;
			Mover.turnSpeed = _config.TurnSpeed;
			Mover.followBase.attackRadius = ConfigManager.Instance.Base.ColliderRadius;
			Mover.followBase.enterRadius = _config.EnterBaseRadius;
			Mover.separation.enabled = _config.SeparationEnabled;
			Mover.separation.desired = _config.SeparationRadius;
			Mover.cohesion.enabled = _config.CohesionEnabled;
			Mover.cohesion.desired = _config.CohesionRadius;
		}
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

	public void PutIntoBase(Planet newPlanet)
	{
		newPlanet.Spawner.unitCount += 1;
		newPlanet.Spawner.UpdateLabel();
		Die();
	}

	public void Die()
	{
		Mode = DroneMode.Dead;
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
		float size = GetComponent<QuadMesh>().size * 1.5f;
		explosion.transform.localScale = new Vector3(size, size, 1);
	}

	private void AssignMaterial()
	{
		if (mesh != null)
		{
			if (Owner.playerNumber < 0)
				mesh.material.SetColor("_Color", GameController.Instance.playerColors[0]);
			else
				mesh.material.SetColor("_Color", GameController.Instance.playerColors[Owner.playerNumber + 1]);
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
		get { return (Mode == DroneMode.Dead); }
	}

	private void OnDrawGizmos()
	{
		if (Collision != null)
		{
			Collision.DrawGizmos();
		}
	}
}
