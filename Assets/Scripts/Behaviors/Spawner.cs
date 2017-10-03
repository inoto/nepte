using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    public bool isActive;
    public int unitCount;
    public UILabel unitCountLabel;
    public float delay;
    public float intervalMin;
    public float intervalMax;
    private float interval;
    public Vector2 point;
    public GameObject prefab;
    private float timeSinceLastSpawn;
    

    Transform trans;
    Owner owner;
    Coroutine spawnCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        //bas = GetComponent<Base>();
        
    }

	public void DelayedStart()
	{
        owner = GetComponent<Owner>();
	    unitCount = 0;
	    if (spawnCoroutine != null)
	        StopCoroutine(spawnCoroutine);
	}

    public void StartSpawn(Vector2 _point)
    {
        point = _point;
        unitCountLabel = GetComponent<Base>().assignedUnitCountLabel;
        StartCoroutine("Spawn");
    }

    public void StopSpawn()
    {
        isActive = false;
    }

    public void ReleaseUnits(GameObject obj)
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        if (unitCount > 0)
            spawnCoroutine = StartCoroutine(ReleaseAllUnits(obj));
    }
    
    IEnumerator ReleaseAllUnits(GameObject obj)
    {
        int count = unitCount;
        while (count > 0)
        //for (int i = 0; i < count; i++)
        {
            GameObject droneObject = ObjectPool.Spawn(prefab, owner.playerController.trans, point, trans.rotation);
            Drone droneSpawned = droneObject.GetComponent<Drone>();
            droneSpawned.owner.playerNumber = owner.playerNumber;
            droneSpawned.owner.playerController = owner.playerController;
            //droneSpawned.playerRallyPoint = rallyPoint;
            droneSpawned.DelayedStart();
            droneSpawned.mover.followRally.UpdateRallyPoint(obj);
            //droneSpawned.mover.followRally.rally = obj;
            //droneSpawned.ResetRallyPoint();
            count--;
            unitCount--;
            UpdateLabel();
            //Delay();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void UpdateLabel()
    {
        if (unitCountLabel != null)
            unitCountLabel.text = unitCount.ToString();
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(delay);
        isActive = true;
        //Debug.Log("Spawn started");
        while (isActive)
        {
            unitCount += 1;
            UpdateLabel();
            
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
