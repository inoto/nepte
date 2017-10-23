using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using UnityEngine;

public class Base : MonoBehaviour, ITargetable
{
	public enum BaseType
	{
		Normal,
		Main,
		Transit,
	}
	public BaseType type;
	
	public static ConfigBase config;
	
    public bool useAsStartPosition = false;

    public bool isDead = false;
	public LineRenderer lineArrow;
	public Dictionary<Base,Vector2> dictDistances = new Dictionary<Base, Vector2>();
	public Dictionary<Base,int> dictOwners = new Dictionary<Base, int>();

	public GameObject propertyIcon;

	[Header("Cache")]
    public UISlider assignedHPbarSlider;
	
    public RallyPoint rallyPoint;
	public List<GameObject> attackers = new List<GameObject>();
	private float arrowOffset = 0;

    [Header("Modules")]
    public Health health = new Health(4000);
	public CollisionCircle collision;
	public CircleCollider2D collider;

    [Header("Components")]
    [SerializeField]
	public Transform trans;
	public Owner owner;
	public Spawner spawner;
	public Weapon weapon;
	MeshRenderer mesh;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject HPbarPrefab;
	public GameObject mothershipOrbitPrefab;
	
    [Header("Colors")]
    [SerializeField] Material materialNeutral;
	[SerializeField] Material[] materials;

	[Header("AIPlayer")]
	public int unitCountNearBasSelf = 0;
	public int unitCountNearBasEnemies = 0;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        spawner = GetComponent<Spawner>();
        owner = GetComponent<Owner>();
	    weapon = GetComponent<Weapon>();
	    collider = GetComponent<CircleCollider2D>();
	    mothershipOrbitPrefab = Resources.Load<GameObject>("MothershipOrbit");
    }

	public void Start()
	{
		trans.localScale = Vector3.one;
		
		collision = new CollisionCircle(trans, null, owner, null);
		CollisionManager.Instance.AddCollidable(collision);

		ConfigManager.Instance.OnConfigsLoaded += Reset;
		GameController.Instance.OnGameStart += Reset;
		GameController.Instance.OnGameStart += Initialize;
	}

	void Reset()
	{
		
		config = ConfigManager.Instance.GetBaseConfig(this);
		if (config == null)
		{
			Debug.LogError("Base config was not found");
			return;
		}
		if (health != null)
		{
			health.max = config.HealthMax;
			health.Reset();
		}
		if (collider != null)
		{
			collider.radius = config.ColliderRadius;
		}
		if (spawner != null)
		{
			spawner.StopSpawn();
			
			spawner.intervalMax = config.ProduceUnitInterval;
			spawner.prefabName = config.SpawnUnitPrefabName;
			spawner.prefab = Resources.Load<GameObject>("Units/" + spawner.prefabName);
			spawner.maxCapturePoints = config.CaptureUnitCount;
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

	void Update()
    {
        if (health.current <= 0)
		{
			Die(new Owner());
		}
	    if (lineArrow != null)
	    {
		    Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    point.z = trans.position.z;
		    lineArrow.SetPosition(1, point);
	    }
    }

	void Initialize()
	{
		if (spawner != null)
		{
			spawner.unitCount = spawner.unitCountInitial;
			spawner.UpdateLabel();
		}
		if (GameController.Instance.playersWithUnassignedBases.Count > 0 && useAsStartPosition)
		{
			int player = GameController.Instance.playersWithUnassignedBases.Dequeue();
			PlayerController playerController = GameController.Instance.playerController[player + 1];
			SetOwner(player, playerController);
			type = BaseType.Main;
			
			playerController.trans.position = trans.position;
			Vector3 pos = trans.position;
			pos.z += 0.1f;
			MothershipOrbit newMothershipOrbit =
				Instantiate(mothershipOrbitPrefab, trans.position, trans.rotation, trans).GetComponent<MothershipOrbit>();
			newMothershipOrbit.transform.position = pos;
			newMothershipOrbit.owner.playerController = owner.playerController;
			newMothershipOrbit.owner.playerNumber = owner.playerNumber;
			newMothershipOrbit.DelayedStart();
			newMothershipOrbit.bas = this;
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
		if (owner.playerNumber != -1)
			mesh.materials[1].SetColor("_TintColor", GameController.Instance.playerColors[owner.playerNumber+1]);
	}

	public void GlowRemove()
	{
		
		List<Material> listMat = new List<Material>(mesh.materials);
		listMat.RemoveAt(1);
		mesh.materials = listMat.ToArray();
	}

	public void MakeArrow()
	{
		
		lineArrow = gameObject.AddComponent<LineRenderer>();
		lineArrow.SetPosition(0, trans.position);
		lineArrow.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
		lineArrow.material = (Material)Resources.Load("Arrow");
		if (owner.playerNumber < 0)
			lineArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[0]);
		else
			lineArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[owner.playerNumber+1]);
		lineArrow.widthMultiplier = 3f;
		lineArrow.material.SetTextureOffset("_MainTex", new Vector2(0.1f, 0));

		foreach (var b in GameController.Instance.bases)
			b.GlowAdd();
	}

	public void SetOwner(int _playerNumber, PlayerController _playerController)
	{
		if (owner.playerController != null)
		{
			if (owner.playerController.bases.Contains(this))
				owner.playerController.bases.Remove(this);
		}
		
		owner.playerNumber = _playerNumber;
		owner.playerController = _playerController;
		
		owner.playerController.bases.Add(this);
//		Debug.Log("bases count of player " + owner.playerNumber + " is " + owner.playerController.bases.Count);

		if (health.percent < 1)
			Reset();

		if (assignedHPbarSlider != null)
			Destroy(assignedHPbarSlider.gameObject);
		// if player is new owner
		if (owner.playerNumber != -1)
		{
			//AddUIHPBar();
			spawner.StartSpawn(trans.position);
		}
		// else it's neutral
		else
		{
			spawner.StopSpawn();
			spawner.StopAllCoroutines();
		}
		spawner.AddBonusInitial();
		spawner.UpdateLabel();
		AssignMaterial();
	}

	void AddUIHPBar()
	{
		Transform UIBars = GameObject.Find("UIBars").transform;
		GameObject prefab = Resources.Load<GameObject>("UI/BaseHPBar");
	    
		Vector2 newPosition = trans.position;
		newPosition.y += mesh.bounds.extents.y-1;
		GameObject assignedHPBarObject = Instantiate(prefab, newPosition, trans.rotation, UIBars);
		assignedHPBarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		assignedHPbarSlider = assignedHPBarObject.GetComponent<UISlider>();
	}

	void AssignMaterial()
	{
		
        if (mesh != null)
        {
	        if (owner.playerNumber < 0)
				mesh.material.SetColor("_TintColor", GameController.Instance.playerColors[0]);
	        else
		        mesh.material.SetColor("_TintColor", GameController.Instance.playerColors[owner.playerNumber+1]);
//            if (owner.playerNumber < 0)
//                mesh.sharedMaterial = materialNeutral;
//            else
//                mesh.sharedMaterial = materials[owner.playerNumber];
        }
		else
			Debug.LogError("Cannot assign material.");
		
	}

    void Die(Owner killerOwner)
    {
        CancelInvoke();
	    spawner.StopSpawn();
        //isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, trans.position, trans.rotation, GameController.Instance.transform);
		//tmpObject.transform.localScale = trans.localScale;
	    PlayerController oldPlayerController = owner.playerController;
	    int oldPlayerNumber = owner.playerNumber;
		SetOwner(-1, GameController.Instance.playerController[0]);
	    health.max = health.maxNoBonuses;
		health.current = health.maxNoBonuses;
	    if (oldPlayerController.bases.Count <= 0 && oldPlayerController.playerUnitCount <= 0)
	    {
		    if (oldPlayerNumber == 0)
			    GameController.Instance.Lose();
		    else if (oldPlayerNumber > 0)
			    GameController.Instance.Win();
	    }
    }
	
	public void AddBonusHP(int addition)
	{
		health.max += addition;
		health.current += addition;
		UpdateHPValue();
	}

	public void RemoveBonusHP(int addition)
	{
		health.max -= addition;
//		health.current -= addition;
		UpdateHPValue();
	}
	
	public void RemoveBonusHPCurrent(int addition)
	{
		health.current -= addition;
		UpdateHPValue();
	}

	public void UpdateHPValue()
	{
		if (assignedHPbarSlider != null)
		{
			assignedHPbarSlider.Set((float) health.current / health.max);
			health.percent = assignedHPbarSlider.value;
		}
	}

	void CalculateUnitCountFromDamage(int damage)
	{
		if (spawner.unitCount <= 0)
			return;
		float oneUnitHealth = health.current / spawner.unitCount;
		spawner.unitCountF -= (float) damage / oneUnitHealth;
		
		if (spawner.unitCountF <= 0 && spawner.unitCount > 0)
		{
			spawner.unitCount -= 1;
			spawner.unitCountF = 1;
			RemoveBonusHP(ConfigManager.Instance.Drone.HealthMax);
			if (this.weapon != null)
				this.weapon.RemoveDamage(ConfigManager.Instance.Drone.AttackDamage);
		}
		spawner.UpdateLabel();
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
		health.current -= fromWeapon.damage;
		UpdateHPValue();
		CalculateUnitCountFromDamage(fromWeapon.damage);
		if (health.current <= 0)
		{
			Die(fromWeapon.owner);
			fromWeapon.EndCombat();
			if (health.current < 0)
				health.current = 0;
		}
	}
	
	public void Damage(int damage)
	{
		health.current -= damage;
		UpdateHPValue();
		CalculateUnitCountFromDamage(damage);
		if (health.current <= 0)
		{
			Die(new Owner());
			if (health.current < 0)
				health.current = 0;
		}
	}

	public GameObject GameObj
	{
		get { return gameObject; }
	}

	public bool IsDied
	{
		get { return (isDead); }
	}
}
