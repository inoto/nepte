using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
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
    [SerializeField]
	public Transform Trans;
	public Owner Owner;
	public Spawner Spawner;
	public Weapon Weapon;
	private MeshRenderer mesh;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
	private GameObject hpBarPrefab;
	private GameObject mothershipOrbitPrefab;
	
    [Header("Colors")]
    [SerializeField] private Material materialNeutral;
	[SerializeField] private Material[] materials;

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
		GameController.Instance.OnGameStart += Reset;
		GameController.Instance.OnGameStart += Initialize;
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
			Health.max = _config.HealthMax;
			Health.Reset();
		}
		if (Collider != null)
		{
			Collider.radius = _config.ColliderRadius;
		}
		if (Spawner != null)
		{
			Spawner.StopSpawn();
			
			Spawner.intervalMax = _config.ProduceUnitInterval;
			Spawner.prefabName = _config.SpawnUnitPrefabName;
			Spawner.prefab = Resources.Load<GameObject>("Units/" + Spawner.prefabName);
			Spawner.maxCapturePoints = _config.CaptureUnitCount;
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

	private void Update()
    {
        if (Health.current <= 0)
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
			Spawner.unitCount = Spawner.unitCountInitial;
			Spawner.UpdateLabel();
		}
		if (GameController.Instance.playersWithUnassignedBases.Count > 0 && UseAsStartPosition)
		{
			int player = GameController.Instance.playersWithUnassignedBases.Dequeue();
			PlayerController playerController = GameController.Instance.playerController[player + 1];
			SetOwner(player, playerController);
			Type = PlanetType.Main;
			
			playerController.trans.position = Trans.position;
			Vector3 pos = Trans.position;
			pos.z += 0.1f;
			MothershipOrbit newMothershipOrbit =
				Instantiate(mothershipOrbitPrefab, Trans.position, Trans.rotation, Trans).GetComponent<MothershipOrbit>();
			newMothershipOrbit.transform.position = pos;
			newMothershipOrbit.Owner.playerController = Owner.playerController;
			newMothershipOrbit.Owner.playerNumber = Owner.playerNumber;
			newMothershipOrbit.DelayedStart();
			newMothershipOrbit.Planet = this;
		}
		else
		{
			int player = -1;
			PlayerController playerController = GameController.Instance.playerController[player + 1];
			SetOwner(player, playerController);
		}
		
	}

	public void GlowAdd()
	{
		Material newMat = Resources.Load<Material>("BaseGlow");
		//		newMat.color = newColor;
		List<Material> listMat = new List<Material>(mesh.materials);
		listMat.Add(newMat);
		mesh.materials = listMat.ToArray();
		if (Owner.playerNumber != -1)
		{
			mesh.materials[1].SetColor("_TintColor", GameController.Instance.playerColors[Owner.playerNumber + 1]);
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
		if (Owner.playerNumber < 0)
		{
			LineRendererArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[0]);
		}
		else
		{
			LineRendererArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[Owner.playerNumber+1]);
		}
		LineRendererArrow.widthMultiplier = 3f;
		LineRendererArrow.material.SetTextureOffset("_MainTex", new Vector2(0.1f, 0));

		foreach (var b in GameController.Instance.bases)
		{
			b.GlowAdd();
		}
	}

	public void SetOwner(int newPlayerNumber, PlayerController newPlayerController)
	{
		if (Owner.playerController != null)
		{
			if (Owner.playerController.bases.Contains(this))
			{
				Owner.playerController.bases.Remove(this);
			}
		}

		Owner.playerNumber = newPlayerNumber;
		Owner.playerController = newPlayerController;
		
		Owner.playerController.bases.Add(this);
//		Debug.Log("bases count of player " + owner.playerNumber + " is " + owner.playerController.bases.Count);

		if (Health.percent < 1)
		{
			Reset();
		}

		if (AssignedHpBarSlider != null)
		{
			Destroy(AssignedHpBarSlider.gameObject);
		}
		// if player is new owner
		if (Owner.playerNumber != -1)
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

	void AddUIHPBar()
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
			if (Owner.playerNumber < 0)
			{
				mesh.material.SetColor("_TintColor", GameController.Instance.playerColors[0]);
			}
			else
			{
				mesh.material.SetColor("_TintColor", GameController.Instance.playerColors[Owner.playerNumber + 1]);
			}
//            if (owner.playerNumber < 0)
//                mesh.sharedMaterial = materialNeutral;
//            else
//                mesh.sharedMaterial = materials[owner.playerNumber];
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
        GameObject tmpObject = Instantiate(explosionPrefab, Trans.position, Trans.rotation, GameController.Instance.transform);
		//tmpObject.transform.localScale = trans.localScale;
	    PlayerController oldPlayerController = Owner.playerController;
	    int oldPlayerNumber = Owner.playerNumber;
		SetOwner(-1, GameController.Instance.playerController[0]);
	    Health.max = Health.maxNoBonuses;
		Health.current = Health.maxNoBonuses;
	    if (oldPlayerController.bases.Count <= 0 && oldPlayerController.playerUnitCount <= 0)
	    {
		    if (oldPlayerNumber == 0)
		    {
			    GameController.Instance.Lose();
		    }
		    else if (oldPlayerNumber > 0)
		    {
			    GameController.Instance.Win();
		    }
	    }
    }
	
	public void AddBonusHp(int addition)
	{
		Health.max += addition;
		Health.current += addition;
		UpdateHpValue();
	}

	public void RemoveBonusHp(int addition)
	{
		Health.max -= addition;
//		health.current -= addition;
		UpdateHpValue();
	}
	
	public void RemoveBonusHpCurrent(int addition)
	{
		Health.current -= addition;
		UpdateHpValue();
	}

	public void UpdateHpValue()
	{
		if (AssignedHpBarSlider != null)
		{
			AssignedHpBarSlider.Set((float) Health.current / Health.max);
			Health.percent = AssignedHpBarSlider.value;
		}
	}

	private void CalculateUnitCountFromDamage(int damage)
	{
		if (Spawner.unitCount <= 0)
		{
			return;
		}
		float oneUnitHealth = Health.current / Spawner.unitCount;
		Spawner.unitCountF -= (float) damage / oneUnitHealth;
		
		if (Spawner.unitCountF <= 0 && Spawner.unitCount > 0)
		{
			Spawner.unitCount -= 1;
			Spawner.unitCountF = 1;
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
		Health.current -= fromWeapon.damage;
		UpdateHpValue();
		CalculateUnitCountFromDamage(fromWeapon.damage);
		if (Health.current <= 0)
		{
			Die(fromWeapon.owner);
			fromWeapon.EndCombat();
			if (Health.current < 0)
			{
				Health.current = 0;
			}
		}
	}
	
	public void Damage(int damage)
	{
		Health.current -= damage;
		UpdateHpValue();
		CalculateUnitCountFromDamage(damage);
		if (Health.current <= 0)
		{
			Die(new Owner());
			if (Health.current < 0)
			{
				Health.current = 0;
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
