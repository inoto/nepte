using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, IOwnable
{
    public int owner;

    public int health = 1000;

    public bool isDead = false;

    [Header("Cache")]
    private Transform trans;
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

	[Header("Sprite sets")]
    public Sprite[] spriteSetDrones;

    //public GameObject rallyPoint;

    // Use this for initialization
    void Start()
    {
        playerControllerParent = transform.parent.gameObject;

        trans = transform;

        GameObject assignedHPbarObject = Instantiate(HPbarPrefab, transform.position, transform.rotation);
        assignedHPbarObject.transform.SetParent(GameObject.Find("HPBars").transform);

        assignedHPbar = assignedHPbarObject.GetComponent<UISprite>();
        assignedHPbar.SetAnchor(gameObject);
		assignedHPbar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        CollisionManager.Instance.AddBaseTransform(transform);

        InvokeRepeating("SpawnDrone", spawnTime, spawnTime);
    }

    // Update is called once per frame
    void Update()
    {
		if (health <= 0)
		{
			Die();
		}

        trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

    void Die()
    {
        isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, transform.position, transform.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
        tmpObject.transform.localScale = trans.localScale;
        CollisionManager.Instance.RemoveBaseTransform(transform);
        Destroy(gameObject);
        Destroy(assignedHPbar.gameObject);
    }

    void SpawnDrone()
    {
        GameObject droneObject = ObjectPool.Spawn(dronePrefab, transform.parent, GameController.Instance.playerStartPosition[owner], transform.rotation);

		Drone droneSpawned = droneObject.GetComponent<Drone>();
        droneSpawned.owner = owner;
        //droneSpawned.ResetRallyPoint();

		droneSpawned.gameObject.GetComponent<SpriteRenderer>().sprite = spriteSetDrones[owner];
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
