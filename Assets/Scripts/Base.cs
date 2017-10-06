using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Base : MonoBehaviour, ITargetable
{
	public enum BaseType
	{
		Normal,
		Transit,
		Aura
	}
	public BaseType type;
	
	public static ConfigBase config;
	
    public bool useAsStartPosition = false;

    public bool isDead = false;
	public LineRenderer lineArrow;

	public GameObject propertyIcon;

	[Header("Cache")]
    public UISlider assignedHPbarSlider;
	
    public RallyPoint rallyPoint;
	public List<GameObject> attackers = new List<GameObject>();

    [Header("Modules")]
    public Health health = new Health(4000);
	public CollisionCircle collision;
	public CircleCollider2D collider;

    [Header("Components")]
    [SerializeField]
	public Transform trans;
	public Owner owner;
	public Spawner spawner;
//    public Body body;
	public Capture capture;
	MeshRenderer mesh;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject HPbarPrefab;
	
    [Header("Colors")]
    [SerializeField] Material materialNeutral;
	[SerializeField] Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        spawner = GetComponent<Spawner>();
        owner = GetComponent<Owner>();
//        body = GetComponent<Body>();
	    capture = GetComponent<Capture>();
	    collider = GetComponent<CircleCollider2D>();
    }

	public void Start()
	{
		trans.localScale = Vector3.one;
		
		collision = new CollisionCircle(trans, null, owner, null);
		CollisionManager.Instance.AddCollidable(collision);

		ConfigManager.Instance.OnConfigsLoaded += LoadFromConfig;
		
	}

	void LoadFromConfig()
	{
		config = ConfigManager.Instance.Base;
		if (config == null)
			return;
		if (health != null)
		{
			health.max = config.HealthMax;
			health.current = health.max;
		}
		if (collider != null)
		{
			collider.radius = config.ColliderRadius;
		}
		if (spawner != null)
		{
			spawner.intervalMax = config.ProduceUnitInterval;
			spawner.prefabName = config.SpawnUnitPrefabName;
			spawner.prefab = Resources.Load<GameObject>("Units/" + spawner.prefabName);
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
        //trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

	public void GlowAdd()
	{
		Material newMat = Resources.Load<Material>("BaseGlow");
		//		newMat.color = newColor;
		List<Material> listMat = new List<Material>(mesh.materials);
		listMat.Add(newMat);
		mesh.materials = listMat.ToArray();
		if (owner.playerNumber != -1)
			mesh.materials[1].SetColor("_TintColor", GameController.Instance.playerColors[owner.playerNumber]);
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
		lineArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[owner.playerNumber]);
		lineArrow.widthMultiplier = 3f;

		foreach (var b in GameController.Instance.bases)
			b.GlowAdd();
	}

	public void SetOwner(int _playerNumber, PlayerController _playerController)
	{
		owner.playerNumber = _playerNumber;
		owner.playerController = _playerController;

		if (capture != null)
			capture.Reset();

		// if player is new owner
		if (owner.playerController != null)
		{
			//owner.playerController.rallyPoint.DelayedStart();

			AddUIHPBar();

			spawner.StartSpawn(trans.position);
		}
		// else it's neutral
		else
		{
//			spawner.unitCount = 0;
			spawner.StopSpawn();
			spawner.StopAllCoroutines();
		}
		spawner.UpdateLabel();
		AssignMaterial();
	}

	public void PutDroneInside(Drone drone)
	{
		if (drone.owner.playerNumber == owner.playerNumber)
		{
			if (spawner.unitCount > 0)
				spawner.unitCount += 1;
		}
		else
		{
			if (spawner.unitCount > 0)
				spawner.unitCount -= 1;
			else
				SetOwner(drone.owner.playerNumber, drone.owner.playerController);
		}

		spawner.UpdateLabel();
		drone.Die();
	}

	void AddUIHPBar()
	{
		Transform HPBars = GameObject.Find("UIBars").transform;
		GameObject prefab = Resources.Load<GameObject>("UI/BaseHPBar");
	    
		Vector2 newPosition = trans.position;
		newPosition.y += mesh.bounds.extents.y-1;
		GameObject assignedHPBarObject = Instantiate(prefab, newPosition, trans.rotation, HPBars);
		assignedHPBarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		assignedHPbarSlider = assignedHPBarObject.GetComponent<UISlider>();
	}

	void AssignMaterial()
	{
        if (mesh != null && owner != null)
        {
            if (owner.playerNumber < 0)
                mesh.sharedMaterial = materialNeutral;
            else
                mesh.sharedMaterial = materials[owner.playerNumber];
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

		if (assignedHPbarSlider != null)
			Destroy(assignedHPbarSlider.gameObject);
		SetOwner(killerOwner.playerNumber, killerOwner.playerController);
		health.current = health.max;
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
	
	public void Damage(Weapon weapon)
	{
		health.current -= weapon.damage;
		if (assignedHPbarSlider != null)
			assignedHPbarSlider.Set((float)health.current / health.max);
		if (health.current <= 0)
		{
			Die(weapon.owner);
			weapon.EndCombat();
		}
	}
	
	public void Damage(int damage)
	{
		health.current -= damage;
		if (assignedHPbarSlider != null)
			assignedHPbarSlider.Set((float)health.current / health.max);
		if (health.current <= 0)
		{
			Die(new Owner());
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
