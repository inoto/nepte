using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    public bool canProduce;
    public bool isActive;
    public float unitCountF;
    public float unitCount;
    public int unitCountMax;
    private UILabel unitCountLabel;
    public bool isCapturing;
    public int maxCapturePoints;
    public int captureLead;
    public float delay;
    public float intervalMin;
    public float intervalMax;
    public Vector2 point;
    public GameObject prefab;
    public string prefabName;

    Transform trans;
    Owner owner;
    Base bas;
    Coroutine releaseCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        bas = GetComponent<Base>();
        prefab = Resources.Load<GameObject>("Units/" + prefabName);
//        Debug.Log("Units/" + prefabName);
        //bas = GetComponent<Base>();
    }

	public void Start()
	{
	    AddUIUnitCount();
//	    unitCount = 0;
	    if (canProduce && releaseCoroutine != null)
	        StopCoroutine(releaseCoroutine);
	}

    public void StartSpawn(Vector2 _point)
    {
        if (!canProduce)
            canProduce = true;
        point = _point;
        UpdateLabel();

//        var bas = GetComponent<Base>();
        AddBonusFromDrone((int)unitCount);
        
        StartCoroutine(Spawn());
    }

    public void StopSpawn()
    {
        canProduce = false;
        isActive = false;
    }

    public void ReleaseUnits(GameObject obj)
    {
        if (canProduce && releaseCoroutine != null)
            StopCoroutine(releaseCoroutine);
        if (unitCount > 0)
            releaseCoroutine = StartCoroutine(ReleaseAllUnits(obj));
    }
    
    IEnumerator ReleaseAllUnits(GameObject obj)
    {
        Base targetBas = obj.GetComponent<Base>();
        
        int count = Mathf.FloorToInt(unitCount);
        while (count > 0)
        //for (int i = 0; i < count; i++)
        {
            GameObject droneObject = ObjectPool.Spawn(prefab, owner.playerController.trans, point, trans.rotation);
            Drone droneSpawned = droneObject.GetComponent<Drone>();
            droneSpawned.owner.playerNumber = owner.playerNumber;
            droneSpawned.owner.playerController = owner.playerController;
            //droneSpawned.playerRallyPoint = rallyPoint;
            droneSpawned.DelayedStart();
            droneSpawned.mover.followBase.UpdateTarget(targetBas);

            count--;
            unitCount--;
            RemoveBonusFromDrone();
            
            UpdateLabel();

            if (canProduce && unitCount < unitCountMax && !isActive)
            {
                StartCoroutine("Spawn");
            }
            
            //Delay();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void EnableCapturing(int side)
    {
        isCapturing = true;
        var sprite = unitCountLabel.transform.parent.GetComponent<UISprite>();
        sprite.color = GameController.Instance.playerColors[side];
        captureLead = side;
    }
    
    public void DisableCapturing()
    {
        isCapturing = false;
        var sprite = unitCountLabel.transform.parent.GetComponent<UISprite>();
        sprite.color = Color.white;
        captureLead = -1;
    }

    public void UpdateLabel()
    {
        if (unitCountLabel != null)
        {
            if (!isCapturing)
                unitCountLabel.text = unitCount.ToString();
            else
                unitCountLabel.text = unitCount + "/" + maxCapturePoints;
        }
    }
    
    public void AddBonusFromDrone()
    {
        AddBonusFromDrone(1);
    }

    void AddBonusFromDrone(int multiplier)
    {
        bas.AddBonusHP(ConfigManager.Instance.Drone.HealthMax * multiplier);
        bas.weapon.AddDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
    }
    
    public void RemoveBonusFromDrone()
    {
        RemoveBonusFromDrone(1);
    }

    void RemoveBonusFromDrone(int multiplier)
    {
        bas.RemoveBonusHP(ConfigManager.Instance.Drone.HealthMax * multiplier);
        bas.weapon.RemoveDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
    }
    
    public void PutDroneInside(Drone drone)
    {
        if (drone.owner.playerNumber == owner.playerNumber)
        {
            unitCount += 1;
            AddBonusFromDrone();
        }
        // if owners are different
        else
        {
            if (!isCapturing)
            {
                if (unitCount > 0)
                {
                    unitCount -= 1;
                    RemoveBonusFromDrone();
                }
                else if (unitCount == 0)
                {
                    EnableCapturing(drone.owner.playerNumber);
                    unitCount += 1;
                }
            }
            else
            {
                if (captureLead == drone.owner.playerNumber)
                {
                    if (unitCount < maxCapturePoints)
                    {
                        unitCount += 1;
                    }
                    if (unitCount >= maxCapturePoints)
                    {
                        DisableCapturing();
                        bas.SetOwner(drone.owner.playerNumber, drone.owner.playerController);
                        unitCount = maxCapturePoints;
                        AddBonusFromDrone(maxCapturePoints);
                    }
                }
                else
                {
                    if (unitCount > 0)
                    {
                        unitCount -= 1;
                    }
                    else
                    {
                        EnableCapturing(drone.owner.playerNumber);
                        unitCount = 1;
                    }
                }
            }
        }
        UpdateLabel();
        drone.Die();
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
            AddBonusFromDrone();
            
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
