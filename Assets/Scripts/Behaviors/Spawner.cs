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
    Owner owner;
    IEnumerator spawnCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        //bas = GetComponent<Base>();
    }

	public void DelayedStart()
	{
        owner = GetComponent<Owner>();
		//spawnCoroutine = Spawn();
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
        yield return new WaitForSeconds(delay * Time.deltaTime*(delay*10));
        isActive = true;
        //Debug.Log("Spawn started");
        while (isActive)
        {
            GameObject obj = ObjectPool.Spawn(prefab, owner.playerController.trans, point, trans.rotation);
            Drone droneSpawned = obj.GetComponent<Drone>();
            droneSpawned.owner.playerNumber = owner.playerNumber;
            droneSpawned.owner.playerController = owner.playerController;
            //droneSpawned.playerRallyPoint = rallyPoint;
            droneSpawned.DelayedStart();
			//droneSpawned.ResetRallyPoint();

			float interval;
            if (intervalMin != intervalMax)
                interval = Random.Range(intervalMin, intervalMax);
            else
                interval = intervalMin;
            yield return new WaitForSeconds(interval * Time.deltaTime*(interval*10));
        }
        //Debug.Log("Spawn stopped");
    }
}
