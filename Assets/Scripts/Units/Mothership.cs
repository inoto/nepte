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
	
	private GameObject explosionPrefab;

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
        Owner.PlayerController.PlayerUnitCount += 1;
        PlayerController.UnitCount += 1;
    }

	private void OnEnable()
	{
		//mode = Mode.Idle;
		Collision = new CollisionCircle(Trans, null, Owner, null);
		CollisionManager.Instance.AddCollidable(Collision);
		Collision.IsDead = false;
		Collision.CollidedBaseCircle = null;
		
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
			Health.Max = _config.HealthMax;
			Health.Current = Health.Max;
		}
		MoveSpeed = _config.SpeedMax;
		MoveRadius = _config.MovingAroundRadius;
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

	private void Die()
	{
		Mode = MothershipMode.Dead;
		Collision.IsDead = true;
		Weapon.Collision.IsDead = true;
		
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
		QuadMesh qm = GetComponent<QuadMesh>();
		if (qm != null)
		{
			float size = qm.Size * 1.5f;
			explosion.transform.localScale = new Vector3(size, size, 1);
		}
		explosion.transform.localScale = Trans.localScale * 3.5f;
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
