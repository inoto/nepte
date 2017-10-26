using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour, ITargetable
{
	public enum PlanetType
	{
		Normal,
		Main,
		Transit,
	}
	public PlanetType Type;
	
	private static ConfigBase _config;
	
    public bool UseAsStartPosition = false;

    public bool IsDead = false;
	public LineRenderer LineRendererArrow;
	public Dictionary<Planet,Vector2> DictDistances = new Dictionary<Planet, Vector2>();
	public GameObject PropertyIcon;

	[Header("Cache")]
    public UISlider AssignedHpBarSlider;

    [Header("Modules")]
    public Health Health = new Health(4000);
	public CollisionCircle Collision;
	public CircleCollider2D Collider;

    [Header("Components")]
	public Transform Trans;
	public Owner Owner;
	public Spawner Spawner;
	public Weapon Weapon;
	private MeshRenderer mesh;

    [Header("Prefabs")]
	private GameObject explosionPrefab;
	private GameObject hpBarPrefab;
	private GameObject mothershipOrbitPrefab;
	
	[Header("AIPlayer")]
	public int UnitCountNearBasSelf = 0;
	public int UnitCountNearBasEnemies = 0;

    private void Awake()
    {
		Trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        Spawner = GetComponent<Spawner>();
        Owner = GetComponent<Owner>();
	    Weapon = GetComponent<Weapon>();
	    Collider = GetComponent<CircleCollider2D>();
	    mothershipOrbitPrefab = Resources.Load<GameObject>("MothershipOrbit");
    }

	public void Start()
	{
		Trans.localScale = Vector3.one;
		
		Collision = new CollisionCircle(Trans, null, Owner, null);
		CollisionManager.Instance.AddCollidable(Collision);

		ConfigManager.Instance.OnConfigsLoaded += Reset;
		GameManager.Instance.OnGameStart += Reset;
		GameManager.Instance.OnGameStart += Initialize;
	}

	private void Reset()
	{
		
		_config = ConfigManager.Instance.GetBaseConfig(this);
		if (_config == null)
		{
			Debug.LogError(typeof(Planet) + " config was not found");
			return;
		}
		if (Health != null)
		{
			Health.Max = _config.HealthMax;
			Health.Reset();
		}
		if (Collider != null)
		{
			Collider.radius = _config.ColliderRadius;
		}
		if (Spawner != null)
		{
			Spawner.StopSpawn();
			
			Spawner.Interval = _config.ProduceUnitInterval;
			Spawner.PrefabName = _config.SpawnUnitPrefabName;
			Spawner.Prefab = Resources.Load<GameObject>("Units/" + Spawner.PrefabName);
			Spawner.MaxCaptureUnits = _config.CaptureUnitCount;
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
	}

	private void Update()
    {
        if (Health.Current <= 0)
		{
			Die(new Owner());
		}
	    if (LineRendererArrow != null)
	    {
		    Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    point.z = Trans.position.z;
		    LineRendererArrow.SetPosition(1, point);
	    }
    }

	private void Initialize()
	{
		if (Spawner != null)
		{
			Spawner.UnitCount = Spawner.UnitCountInitial;
			Spawner.UpdateLabel();
		}
		if (GameManager.Instance.PlayersWithUnassignedPlanets.Count > 0 && UseAsStartPosition)
		{
			int player = GameManager.Instance.PlayersWithUnassignedPlanets.Dequeue();
			PlayerController playerController = GameManager.Instance.PlayerController[player + 1];
			SetOwner(player, playerController);
			Type = PlanetType.Main;
			
			playerController.Trans.position = Trans.position;
			Vector3 pos = Trans.position;
			pos.z += 0.1f;
			MothershipOrbit newMothershipOrbit =
				Instantiate(mothershipOrbitPrefab, Trans.position, Trans.rotation, Trans).GetComponent<MothershipOrbit>();
			newMothershipOrbit.transform.position = pos;
			newMothershipOrbit.Owner.PlayerController = Owner.PlayerController;
			newMothershipOrbit.Owner.PlayerNumber = Owner.PlayerNumber;
			newMothershipOrbit.DelayedStart();
			newMothershipOrbit.Planet = this;
		}
		else
		{
			int player = -1;
			PlayerController playerController = GameManager.Instance.PlayerController[player + 1];
			SetOwner(player, playerController);
		}
		explosionPrefab = Resources.Load<GameObject>("Explosion");
	}

	public void GlowAdd()
	{
		Material newMat = Resources.Load<Material>("BaseGlow");
		//		newMat.color = newColor;
		List<Material> listMat = new List<Material>(mesh.materials);
		listMat.Add(newMat);
		mesh.materials = listMat.ToArray();
		if (Owner.PlayerNumber != -1)
		{
			mesh.materials[1].SetColor("_TintColor", GameManager.Instance.PlayerColors[Owner.PlayerNumber + 1]);
		}
	}

	public void GlowRemove()
	{
		List<Material> listMat = new List<Material>(mesh.materials);
		listMat.RemoveAt(1);
		mesh.materials = listMat.ToArray();
	}

	public void MakeArrow()
	{
		LineRendererArrow = gameObject.AddComponent<LineRenderer>();
		LineRendererArrow.SetPosition(0, Trans.position);
		LineRendererArrow.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
		LineRendererArrow.material = (Material)Resources.Load("Arrow");
		if (Owner.PlayerNumber < 0)
		{
			LineRendererArrow.material.SetColor("_TintColor", GameManager.Instance.PlayerColors[0]);
		}
		else
		{
			LineRendererArrow.material.SetColor("_TintColor", GameManager.Instance.PlayerColors[Owner.PlayerNumber+1]);
		}
		LineRendererArrow.widthMultiplier = 3f;
		LineRendererArrow.material.SetTextureOffset("_MainTex", new Vector2(0.1f, 0));

		foreach (var b in GameManager.Instance.Planets)
		{
			b.GlowAdd();
		}
	}

	public void SetOwner(int newPlayerNumber, PlayerController newPlayerController)
	{
		if (Owner.PlayerController != null)
		{
			if (Owner.PlayerController.Planets.Contains(this))
			{
				Owner.PlayerController.Planets.Remove(this);
			}
		}

		Owner.PlayerNumber = newPlayerNumber;
		Owner.PlayerController = newPlayerController;
		
		Owner.PlayerController.Planets.Add(this);
//		Debug.Log("bases count of player " + owner.playerNumber + " is " + owner.playerController.bases.Count);

		if (Health.Percent < 1)
		{
			Reset();
		}

		if (AssignedHpBarSlider != null)
		{
			Destroy(AssignedHpBarSlider.gameObject);
		}
		// if player is new owner
		if (Owner.PlayerNumber != -1)
		{
			//AddUIHPBar();
			Spawner.StartSpawn(Trans.position);
		}
		// else it's neutral
		else
		{
			Spawner.StopSpawn();
			Spawner.StopAllCoroutines();
		}
		Spawner.AddBonusInitial();
		Spawner.UpdateLabel();
		AssignMaterial();
	}

	private void AddUIHPBar()
	{
		Transform UIBars = GameObject.Find("UIBars").transform;
		GameObject prefab = Resources.Load<GameObject>("UI/BaseHPBar");
	    
		Vector2 newPosition = Trans.position;
		newPosition.y += mesh.bounds.extents.y-1;
		GameObject assignedHpBarObject = Instantiate(prefab, newPosition, Trans.rotation, UIBars);
		assignedHpBarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		AssignedHpBarSlider = assignedHpBarObject.GetComponent<UISlider>();
	}

	private void AssignMaterial()
	{
		if (mesh != null)
		{
			if (Owner.PlayerNumber < 0)
			{
				mesh.material.SetColor("_TintColor", GameManager.Instance.PlayerColors[0]);
			}
			else
			{
				mesh.material.SetColor("_TintColor", GameManager.Instance.PlayerColors[Owner.PlayerNumber + 1]);
			}
		}
		else
		{
			Debug.LogError("Cannot assign material.");
		}
	}

    private void Die(Owner killerOwner)
    {
        CancelInvoke();
	    Spawner.StopSpawn();
        //isDead = true;
        GameObject explosion = Instantiate(explosionPrefab, Trans.position, Trans.rotation, GameManager.Instance.transform);
	    explosion.transform.localScale = Trans.localScale * 6;
	    explosion.GetComponent<AudioSource>().enabled = true;
	    PlayerController oldPlayerController = Owner.PlayerController;
	    int oldPlayerNumber = Owner.PlayerNumber;
		SetOwner(-1, GameManager.Instance.PlayerController[0]);
	    Health.Max = Health.MaxNoBonuses;
		Health.Current = Health.MaxNoBonuses;
	    if (oldPlayerController.Planets.Count <= 0 && oldPlayerController.PlayerUnitCount <= 0)
	    {
		    if (oldPlayerNumber == 0)
		    {
			    GameManager.Instance.Lose();
		    }
		    else if (oldPlayerNumber > 0)
		    {
			    GameManager.Instance.Win();
		    }
	    }
    }
	
	public void AddBonusHp(int addition)
	{
		Health.Max += addition;
		Health.Current += addition;
		UpdateHpValue();
	}

	public void RemoveBonusHp(int addition)
	{
		Health.Max -= addition;
//		health.current -= addition;
		UpdateHpValue();
	}
	
	public void RemoveBonusHpCurrent(int addition)
	{
		Health.Current -= addition;
		UpdateHpValue();
	}

	public void UpdateHpValue()
	{
		if (AssignedHpBarSlider != null)
		{
			AssignedHpBarSlider.Set((float) Health.Current / Health.Max);
			Health.Percent = AssignedHpBarSlider.value;
		}
	}

	private void CalculateUnitCountFromDamage(int damage)
	{
		if (Spawner.UnitCount <= 0)
		{
			return;
		}
		float oneUnitHealth = Health.Current / Spawner.UnitCount;
		Spawner.UnitCountF -= (float) damage / oneUnitHealth;
		
		if (Spawner.UnitCountF <= 0 && Spawner.UnitCount > 0)
		{
			Spawner.UnitCount -= 1;
			Spawner.UnitCountF = 1;
			RemoveBonusHp(ConfigManager.Instance.Drone.HealthMax);
			if (this.Weapon != null)
			{
				this.Weapon.RemoveDamage(ConfigManager.Instance.Drone.AttackDamage);
			}
		}
		Spawner.UpdateLabel();
	}

	public void OnDrawGizmos()
	{
		//if (showRadius)
		//{
		//	Color newColorAgain = Color.green;
		//	newColorAgain.a = 0.5f;
		//	Gizmos.color = newColorAgain;
		//	Gizmos.DrawWireSphere(trans.position, radius);
		//}
	}
	
	public void Damage(Weapon fromWeapon)
	{
		Health.Current -= fromWeapon.Damage;
		UpdateHpValue();
		CalculateUnitCountFromDamage(fromWeapon.Damage);
		if (Health.Current <= 0)
		{
			Die(fromWeapon.Owner);
			fromWeapon.EndCombat();
			if (Health.Current < 0)
			{
				Health.Current = 0;
			}
		}
	}
	
	public void Damage(int damage)
	{
		Health.Current -= damage;
		UpdateHpValue();
		CalculateUnitCountFromDamage(damage);
		if (Health.Current <= 0)
		{
			Die(new Owner());
			if (Health.Current < 0)
			{
				Health.Current = 0;
			}
		}
	}

	public GameObject GameObj
	{
		get { return gameObject; }
	}

	public bool IsDied
	{
		get { return (IsDead); }
	}
}
