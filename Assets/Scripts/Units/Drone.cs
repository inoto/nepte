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

    [Header("Modules")]
    public Health Health;
	public CollisionCircle Collision;

    [Header("Components")]
    public Transform Trans;
    private MeshRenderer mesh;
    public Owner Owner;
    public Mover Mover;
	public Weapon Weapon;

	private GameObject explosionPrefab;

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
        Owner.PlayerController.PlayerUnitCount += 1;
        PlayerController.UnitCount += 1;
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
			Health.Max = _config.HealthMax;
			Health.Current = Health.Max;
		}
		if (Mover != null)
		{
			Mover.MaxSpeed = _config.SpeedMax;
			Mover.MaxForce = _config.ForceMax;
			Mover.TurnSpeed = _config.TurnSpeed;
			Mover.FollowBase.AttackRadius = ConfigManager.Instance.Base.ColliderRadius;
			Mover.FollowBase.EnterRadius = _config.EnterBaseRadius;
			Mover.Separation.Enabled = _config.SeparationEnabled;
			Mover.Separation.Desired = _config.SeparationRadius;
			Mover.Cohesion.Enabled = _config.CohesionEnabled;
			Mover.Cohesion.Desired = _config.CohesionRadius;
		}
		if (Weapon != null)
		{
			Weapon.AttackSpeed = _config.AttackSpeed;
			Weapon.Damage = _config.AttackDamage;
			Weapon.DamageNoBonuses = Weapon.Damage;
			Weapon.Radius = _config.AttackRadius;
			Weapon.MissilePrefabName = _config.AttackMissilePrefabName;
			Weapon.MissilePrefab = Resources.Load<GameObject>(Weapon.MissilePrefabName);
		}
		explosionPrefab = Resources.Load<GameObject>("Explosion");
	}

	public void PutIntoBase(Planet newPlanet)
	{
		newPlanet.Spawner.UnitCount += 1;
		newPlanet.Spawner.UpdateLabel();
		Die();
	}

	public void Die()
	{
		Mode = DroneMode.Dead;
		Collision.isDead = true;
		Weapon.Collision.isDead = true;

		if (Health.Current <= 0)
		{
			MakeExplosion();
		}
		Owner.PlayerController.PlayerUnitCount -= 1;
		PlayerController.UnitCount -= 1;
		ObjectPool.Recycle(gameObject);
	}

	private void MakeExplosion()
	{
		GameObject explosion = Instantiate(explosionPrefab, Trans.position, Trans.rotation, GameManager.Instance.transform);
		float size = GetComponent<QuadMesh>().size * 1.5f;
		explosion.transform.localScale = new Vector3(size, size, 1);
		explosion.transform.localScale = Trans.localScale * 2;
	}

	private void AssignMaterial()
	{
		if (mesh != null)
		{
			if (Owner.PlayerNumber < 0)
			{
				mesh.material.SetColor("_Color", GameManager.Instance.PlayerColors[0]);
			}
			else
			{
				mesh.material.SetColor("_Color", GameManager.Instance.PlayerColors[Owner.PlayerNumber + 1]);
			}
		}
		else
		{
			Debug.LogError("Cannot assign material.");
		}
	}

	public void Damage(Weapon fromWeapon)
	{
		Health.Current -= fromWeapon.Damage;
		if (Health.Current <= 0)
		{
			Die();
			fromWeapon.EndCombat();
		}
	}
	
	public void Damage(int damage)
	{
		Health.Current -= damage;
		if (Health.Current <= 0)
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
