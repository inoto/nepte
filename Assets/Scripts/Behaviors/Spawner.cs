using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    public bool canProduce;
    public bool isActive;
    public int unitCount;
    public int unitCountMax;
    private UILabel unitCountLabel;
    public float delay;
    public float intervalMin;
    public float intervalMax;
    public Vector2 point;
    public GameObject prefab;
    public string prefabName;

    Transform trans;
    Owner owner;
    Coroutine spawnCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        prefab = Resources.Load<GameObject>("Units/" + prefabName);
//        Debug.Log("Units/" + prefabName);
        //bas = GetComponent<Base>();
        
    }

	public void Start()
	{
	    AddUIUnitCount();
//	    unitCount = 0;
	    if (canProduce && spawnCoroutine != null)
	        StopCoroutine(spawnCoroutine);
	}

    public void StartSpawn(Vector2 _point)
    {
        if (!canProduce)
            canProduce = true;
        point = _point;
        UpdateLabel();
        StartCoroutine("Spawn");
    }

    public void StopSpawn()
    {
        canProduce = false;
        isActive = false;
    }

    public void ReleaseUnits(GameObject obj)
    {
        if (canProduce && spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        if (unitCount > 0)
            spawnCoroutine = StartCoroutine(ReleaseAllUnits(obj));
    }
    
    IEnumerator ReleaseAllUnits(GameObject obj)
    {
        Base bas = obj.GetComponent<Base>();
        
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
            droneSpawned.mover.followBase.UpdateTarget(bas);
            //droneSpawned.mover.followRally.rally = obj;
            //droneSpawned.ResetRallyPoint();
            count--;
            unitCount--;
            UpdateLabel();

            if (canProduce && unitCount < unitCountMax && !isActive)
            {
                StartCoroutine("Spawn");
            }
            
            //Delay();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void UpdateLabel()
    {
        if (unitCountLabel != null)
            unitCountLabel.text = unitCount.ToString();
    }
    
    void AddUIUnitCount()
    {
        Transform HPBars = GameObject.Find("UIBars").transform;
        GameObject prefab = Resources.Load<GameObject>("UI/BaseUnitCount");
	    
        Vector2 newPosition = trans.position;
        newPosition.y += GetComponent<MeshRenderer>().bounds.extents.y;
        GameObject assignedUnitCountObject = Instantiate(prefab, newPosition, trans.rotation, HPBars);
        assignedUnitCountObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        unitCountLabel = assignedUnitCountObject.transform.GetChild(0).GetComponent<UILabel>();
//		UISprite assignedHPbarSprite = assignedUIBarObject.GetComponent<UISprite>();
//		assignedHPbarSprite.SetAnchor(gameObject);
    }

    IEnumerator Spawn()
    {
        isActive = true;
        yield return new WaitForSeconds(delay);
        
//        Debug.Log("Spawn started");
        while (isActive)
        {
            if (unitCount >= unitCountMax)
            {
                isActive = false;
                yield break;
            }
            unitCount += 1;
            UpdateLabel();
            
			float interval;
            if (intervalMin != intervalMax)
                interval = Random.Range(intervalMin, intervalMax);
            else
                interval = intervalMax;
            yield return new WaitForSeconds(interval);
        }
        //Debug.Log("Spawn stopped");
    }
}
