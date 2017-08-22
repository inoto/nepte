using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, ITargetable, ICollidable
{
    public bool showRadius;

    public int owner;

    public int health = 4000;
    public float radius;
    public CollisionType cType = CollisionType.Base;
    public TargetType tType = TargetType.Base;

    public bool isDead = false;

	public List<Node> node = new List<Node>();

    [Header("Cache")]
    public Transform trans;
	private MeshRenderer mesh;
	private MeshFilter mf;
    private GameObject playerControllerParent;
    public UISlider assignedHPbarSlider;
    public RallyPoint rallyPoint;
    public LaserMissile triggeredLaserMissile;
	public List<GameObject> attackers = new List<GameObject>();

    [Header("Spawn")]
    public float spawnTime = 3f;
    public GameObject dronePrefab;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject HPbarPrefab;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        radius = GetComponent<Quad>().size;
		playerControllerParent = transform.parent.gameObject;

		GameObject assignedHPbarObject = Instantiate(HPbarPrefab, trans.position, trans.rotation);
		assignedHPbarObject.transform.SetParent(GameObject.Find("HPBars").transform);
        assignedHPbarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        UISprite assignedHPbarSprite = assignedHPbarObject.GetComponent<UISprite>();
		assignedHPbarSprite.SetAnchor(gameObject);
		
        assignedHPbarSlider = assignedHPbarObject.GetComponent<UISlider>();

		InvokeRepeating("SpawnDrone", spawnTime, spawnTime);

		TakeNodes();
        CollisionManager.Instance.AddCollidable(this);
    }

    public void StartWithOwner()
    {
        AssignMaterial();
    }

    // Update is called once per frame
    void Update()
    {
		if (health <= 0)
		{
			Die();
		}

        //trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

    public void Damage(int damage)
    {
		health -= damage;
        assignedHPbarSlider.Set((float)health / 4000);
    }

	void TakeNodes()
	{
		Vector2 tmp = new Vector2(trans.position.x, trans.position.y);
		List<Node> list = Grid.Instance.FindNodesInRadius(tmp, GetComponent<Quad>().size);
		foreach (Node n in list)
		{
			n.ImprisonObject(gameObject);
		}
	}

	void AssignMaterial()
	{
		mesh.material = materials[owner];
	}

    private void OnDestroy()
    {
        CollisionManager.Instance.RemoveCollidable(this);
    }

    void Die()
    {
        CancelInvoke();
        isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, trans.position, trans.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
		//tmpObject.transform.localScale = trans.localScale;
		CollisionManager.Instance.RemoveCollidable(this);
        Destroy(assignedHPbarSlider.gameObject);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }

    void SpawnDrone()
    {
		
        GameObject droneObject = ObjectPool.Spawn(dronePrefab, trans.parent, GameController.Instance.playerStartPosition[owner], trans.rotation);
		Drone droneSpawned = droneObject.GetComponent<Drone>();
        droneSpawned.owner = owner;
        droneSpawned.playerRallyPoint = rallyPoint;
		droneSpawned.ActivateWithOwner();
        //droneSpawned.ResetRallyPoint();
    }

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.green;
			newColorAgain.a = 0.5f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}

	public int InstanceId
	{
		get { return gameObject.GetInstanceID(); }
	}
	public Vector2 Point
	{
		get { return trans.position; }
		set { trans.position = value; }
	}
	public float Radius
	{
        get { return radius; }
	}
	public float RadiusHard
	{
		get { return radius; }
	}
	public CollisionType collisionType
	{
		get { return cType; }
	}
	public bool Active
	{
		get { return gameObject.activeSelf; }
	}
	public GameObject GameObject
	{
		get { return gameObject; }
	}
	public Drone drone
	{
		get { return null; }
	}
	public Base bas
	{
		get { return this; }
	}

	public Drone DroneObj
	{
		get { return null; }
	}
	public Base BaseObj
	{
		get { return this; }
	}
	public GameObject GameObj
	{
		get { return gameObject; }
	}
	public TargetType targetableType
	{
		get { return tType; }
	}
    public bool IsDied
    {
        get { return isDead; }
    }
}
