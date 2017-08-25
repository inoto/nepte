using UnityEngine;
using System.Collections;

public class Cohesion : MonoBehaviour
{
	public Drone drone;

	private void Awake()
	{
		drone = GetComponent<Drone>();
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
