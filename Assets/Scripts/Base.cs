using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, IOwnable
{
    public int owner;

    public int health = 1000;

    public bool isDead = false;

	public List<Node> node = new List<Node>();

    [Header("Cache")]
    public Transform trans;
	private MeshRenderer mesh;
	private MeshFilter mf;
    private GameObject playerControllerParent;
    public UISprite assignedHPbar;
    public GameObject rallyPointObject;
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
		playerControllerParent = transform.parent.gameObject;

		GameObject assignedHPbarObject = Instantiate(HPbarPrefab, trans.position, trans.rotation);
		assignedHPbarObject.transform.SetParent(GameObject.Find("HPBars").transform);

		assignedHPbar = assignedHPbarObject.GetComponent<UISprite>();
		assignedHPbar.SetAnchor(gameObject);
		assignedHPbar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		InvokeRepeating("SpawnDrone", spawnTime, spawnTime);

		TakeNodes();
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

    void Die()
    {
        isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, trans.position, trans.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
        tmpObject.transform.localScale = trans.localScale;
        Destroy(gameObject);
        Destroy(assignedHPbar.gameObject);
    }

    void SpawnDrone()
    {
		
        GameObject droneObject = ObjectPool.Spawn(dronePrefab, trans.parent, GameController.Instance.playerStartPosition[owner], trans.rotation);
		Drone droneSpawned = droneObject.GetComponent<Drone>();
        droneSpawned.owner = owner;
        droneObject.GetComponent<Unit>().ActivateWithOwner();
        //droneSpawned.ResetRallyPoint();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("missile"))
        {
            triggeredLaserMissile = other.gameObject.GetComponent<LaserMissile>();
            if (triggeredLaserMissile.owner != owner
                && !triggeredLaserMissile.wasExecuted)
            {
                triggeredLaserMissile.wasExecuted = true;
                if (assignedHPbar != null)
                {
                    UISlider HPslider = assignedHPbar.GetComponent<UISlider>();
                    HPslider.Set((float)health / 1000);
                }
                health -= triggeredLaserMissile.damage;
            }

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredLaserMissile = null;
    }

	public void AddAttacker(GameObject newObj)
	{
		attackers.Add(newObj);
	}

	public bool IsActive()
	{
        return gameObject.activeSelf;
	}

	public int GetOwner()
	{
		return owner;
	}

	public void SetOwner(int newOwner)
	{
		owner = newOwner;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
