using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	public int owner = 0;
	public int health = 100;
    public float spawnTime = 2f;
    public GameObject spawnUnit;

    //public GameObject rallyPoint;

	// Use this for initialization
	void Start ()
    {
		InvokeRepeating("SpawnDrone", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void SpawnDrone()
    {
        GameObject instance = Instantiate(spawnUnit, transform.position, transform.rotation);
		instance.GetComponent<Drone>().owner = owner;
		instance.transform.SetParent(transform);
        instance.GetComponent<Drone>().SetRallyPoint(GetComponentInParent<PlayerController>().pRallyPoint);
    }
}
