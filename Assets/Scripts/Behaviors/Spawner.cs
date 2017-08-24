using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    public bool isActive;
    public float delay;
    public float intervalMin;
    public float intervalMax;
    public Vector2 point;
    public GameObject prefab;

    Transform trans;
    Base bas;
    IEnumerator spawnCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        bas = GetComponent<Base>();
    }

	private void Start()
	{
		spawnCoroutine = Spawn();
	}

    public void StartSpawn(Vector2 _point)
    {
        point = _point;
        StartCoroutine("Spawn");
    }

    public void StopSpawn()
    {
        isActive = false;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(delay);
        isActive = true;
        //Debug.Log("Spawn started");
        while (isActive)
        {
			GameObject obj = ObjectPool.Spawn(prefab, trans.parent, point, trans.rotation);
			Drone droneSpawned = obj.GetComponent<Drone>();
			droneSpawned.owner = bas.owner;
			//droneSpawned.playerRallyPoint = rallyPoint;
			droneSpawned.ActivateWithOwner();
			//droneSpawned.ResetRallyPoint();

			float interval;
            if (intervalMin != intervalMax)
                interval = Random.Range(intervalMin, intervalMax);
            else
                interval = intervalMin;
            yield return new WaitForSeconds(interval);
        }
        //Debug.Log("Spawn stopped");
    }
}
