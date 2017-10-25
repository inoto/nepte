using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    public bool canProduce;
    public bool isActive;
    [Header("Unit count")]
    public float unitCountInitial;
    public float unitCountF;
    public float unitCount;
    public int unitCountMax;
    private UILabel unitCountLabel;
    [Header("Capture")]
    public bool isCapturing;
    public int maxCapturePoints;
    public int captureLead;
    [Header("Spawn")]
    public float delay;
    public float intervalMin;
    public float intervalMax;
    public Vector2 point;
    public GameObject prefab;
    public string prefabName;

    Transform trans;
    Owner owner;
    Planet bas;
    Coroutine releaseCoroutine;
    Coroutine spawnCoroutine;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        bas = GetComponent<Planet>();
        prefab = Resources.Load<GameObject>("Units/" + prefabName);
    }

	public void Start()
	{
	    AddUIUnitCount();
	    if (canProduce && releaseCoroutine != null)
	        StopCoroutine(releaseCoroutine);
	}

    public void StartSpawn(Vector2 _point)
    {
        if (bas.Type == Planet.PlanetType.Transit)
            return;
        if (!canProduce)
            canProduce = true;
        point = _point;
        UpdateLabel();

        spawnCoroutine = StartCoroutine(Spawn());
    }

    public void StopSpawn()
    {
        canProduce = false;
        isActive = false;
    }

    public void ReleaseUnits(GameObject obj)
    {
        if (obj.GetInstanceID() == gameObject.GetInstanceID())
            return;
        if (releaseCoroutine != null)
            StopCoroutine(releaseCoroutine);
        if (unitCount > 0)
            releaseCoroutine = StartCoroutine(ReleaseAllUnits(obj));
    }
    
    IEnumerator ReleaseAllUnits(GameObject obj)
    {
        Planet targetBas = obj.GetComponent<Planet>();
        
        int count = Mathf.FloorToInt(unitCount);
        while (count > 0)
        {
            GameObject droneObject = ObjectPool.Spawn(prefab, owner.playerController.Trans, point, trans.rotation);
            Drone droneSpawned = droneObject.GetComponent<Drone>();
            droneSpawned.Owner.playerNumber = owner.playerNumber;
            droneSpawned.Owner.playerController = owner.playerController;
            droneSpawned.DelayedStart();
            droneSpawned.Mover.followBase.UpdateTarget(targetBas);

            count--;
            unitCount--;
            RemoveBonusFromDrone();
            if (bas.Health.current > bas.Health.max)
                bas.RemoveBonusHpCurrent(ConfigManager.Instance.Drone.HealthMax);
            
            UpdateLabel();

            if (canProduce && unitCount < unitCountMax && !isActive)
            {
                StartCoroutine(Spawn());
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void EnableCapturing(int side)
    {
        isCapturing = true;
        var sprite = unitCountLabel.transform.parent.GetComponent<UISprite>();
        if (side >= 0)
            sprite.color = GameManager.Instance.PlayerColors[side+1];
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

    public void AddBonusInitial()
    {
        AddBonusFromDrone((int)unitCount);
    }
    
    public void AddBonusFromDrone()
    {
        AddBonusFromDrone(1);
    }

    void AddBonusFromDrone(int multiplier)
    {
        bas.AddBonusHp(ConfigManager.Instance.Drone.HealthMax * multiplier);
        if (bas.Weapon != null)
            bas.Weapon.AddDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
    }
    
    public void RemoveBonusFromDrone()
    {
        RemoveBonusFromDrone(1);
    }

    void RemoveBonusFromDrone(int multiplier)
    {
        bas.RemoveBonusHp(ConfigManager.Instance.Drone.HealthMax * multiplier);
        if (bas.Weapon != null)
            bas.Weapon.RemoveDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
    }
    
    public void PutDroneInside(Drone drone)
    {
        if (drone.Owner.playerNumber == owner.playerNumber)
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
                    bas.RemoveBonusHpCurrent(ConfigManager.Instance.Drone.HealthMax);
                }
                else if (unitCount == 0)
                {
                    EnableCapturing(drone.Owner.playerNumber);
                    unitCount += 1;
                }
                else
                {
                    EnableCapturing(drone.Owner.playerNumber);
                    unitCount = 1;
                }
            }
            else
            {
                if (captureLead == drone.Owner.playerNumber)
                {
                    if (unitCount < maxCapturePoints)
                    {
                        unitCount += 1;
                    }
                    if (unitCount >= maxCapturePoints)
                    {
                        DisableCapturing();
                        bas.SetOwner(drone.Owner.playerNumber, drone.Owner.playerController);
                        unitCount = maxCapturePoints;
//                        AddBonusFromDrone((int)unitCount);
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
                        EnableCapturing(drone.Owner.playerNumber);
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
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        isActive = true;
        yield return new WaitForSeconds(delay);
        
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
    }
}
