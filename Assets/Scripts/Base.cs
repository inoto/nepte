using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, IOwnable
{
    public int owner = 0;

    public int health = 1000;

    [Header("Cache")]
    private GameObject playerControllerParent;
    public GameObject rallyPointObject;
    public LaserMissile triggeredLaserMissile;

    [Header("Spawn")]
    public float spawnTime = 3f;
    public GameObject dronePrefab;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;

    //public GameObject rallyPoint;

    // Use this for initialization
    void Start()
    {
        playerControllerParent = transform.parent.gameObject;

        InvokeRepeating("SpawnDrone", spawnTime, spawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Kill();
        }
    }

    void Kill()
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void SpawnDrone()
    {
        GameObject droneObject = Instantiate(dronePrefab, transform.position, transform.rotation);
        Drone droneSpawned = droneObject.GetComponent<Drone>();
        droneSpawned.owner = owner;
        //PlayerController playerController = transform.parent.GetComponent<PlayerController>();
        droneSpawned.transform.SetParent(transform.parent);
        droneSpawned.playerRallyPoint = rallyPointObject;

        GameController gameController = playerControllerParent.transform.parent.gameObject.GetComponent<GameController>();
        droneSpawned.gameObject.GetComponent<SpriteRenderer>().sprite = gameController.spriteSetDrones[owner];
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
                health -= triggeredLaserMissile.damage;
            }

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredLaserMissile = null;
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
