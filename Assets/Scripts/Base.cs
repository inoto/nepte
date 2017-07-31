using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	public int owner = 0;

	public int health = 1000;

    [Header("Cache")]
    public GameObject playerRallyPoint;
    public GameObject droneSpawned;
    public LaserMissile laserMissileTriggered;

    [Header("Spawn")]
    public float spawnTime = 3f;
    public GameObject dronePrefab;

    [Header("Prefabs")]
    public GameObject explosionPrefab;

    //public GameObject rallyPoint;

	// Use this for initialization
	void Start ()
    {
		InvokeRepeating("SpawnDrone", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Update ()
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
        droneSpawned = Instantiate(dronePrefab, transform.position, transform.rotation);
		droneSpawned.GetComponent<Drone>().owner = owner;
		droneSpawned.transform.SetParent(transform);
        droneSpawned.GetComponent<Drone>().playerRallyPoint = playerRallyPoint;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "missile")
        {
            laserMissileTriggered = other.gameObject.GetComponent<LaserMissile>();
            if (laserMissileTriggered.owner != owner
                && !laserMissileTriggered.wasExecuted)
            {
                laserMissileTriggered.wasExecuted = true;
                health -= laserMissileTriggered.damage;
            }

        }
    }
}
