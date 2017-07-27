using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
	public int owner = 0;
	public int health = 100;
    public float spawnTime = 2f;
    public GameObject drone;

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
        Instantiate(drone, transform.position, transform.rotation);
    }
}
