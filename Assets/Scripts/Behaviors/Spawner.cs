using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public bool CanProduce;
    public bool IsActive;
    [Header("Unit count")]
    public float UnitCountInitial;
    public float UnitCountF;
    public float UnitCount;
    public int UnitCountMax;
    private UILabel unitCountLabel;
    [Header("Capture")]
    public bool IsCapturing;
    public int MaxCaptureUnits;
    public int CaptureLeadPlayer;
    [Header("Spawn")]
    public float Delay;
    public float Interval;
    public Vector2 Point;
    public GameObject Prefab;
    public string PrefabName;

    [Header("Cache")]
    public Transform Trans;
    public Owner Owner;
    public Planet Planet;
    private Coroutine releaseCoroutine;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        Trans = GetComponent<Transform>();
        Owner = GetComponent<Owner>();
        Planet = GetComponent<Planet>();
        Prefab = Resources.Load<GameObject>("Units/" + PrefabName);
    }

	public void Start()
	{
	    AddUiUnitCount();
	    if (CanProduce && releaseCoroutine != null)
	    {
	        StopCoroutine(releaseCoroutine);
	    }
	}

    public void StartSpawn(Vector2 newPoint)
    {
        if (Planet.Type == Planet.PlanetType.Transit)
        {
            return;
        }
        if (!CanProduce)
        {
            CanProduce = true;
        }
        Point = newPoint;
        UpdateLabel();

        spawnCoroutine = StartCoroutine(Spawn());
    }

    public void StopSpawn()
    {
        CanProduce = false;
        IsActive = false;
    }

    public void ReleaseUnits(GameObject obj)
    {
        if (obj.GetInstanceID() == gameObject.GetInstanceID())
        {
            return;
        }
        if (releaseCoroutine != null)
        {
            StopCoroutine(releaseCoroutine);
        }
        if (UnitCount > 0)
        {
            releaseCoroutine = StartCoroutine(ReleaseAllUnits(obj));
        }
    }
    
    private IEnumerator ReleaseAllUnits(GameObject obj)
    {
        Planet targetPlanet = obj.GetComponent<Planet>();
        
        int count = Mathf.FloorToInt(UnitCount);
        while (count > 0)
        {
            GameObject droneObject = ObjectPool.Spawn(Prefab, Owner.PlayerController.Trans, Point, Trans.rotation);
            Drone droneSpawned = droneObject.GetComponent<Drone>();
            droneSpawned.Owner.PlayerNumber = Owner.PlayerNumber;
            droneSpawned.Owner.PlayerController = Owner.PlayerController;
            droneSpawned.DelayedStart();
            droneSpawned.Mover.FollowBase.UpdateTarget(targetPlanet);

            count--;
            UnitCount--;
            RemoveBonusFromDrone();
            if (Planet.Health.Current > Planet.Health.Max)
            {
                Planet.RemoveBonusHpCurrent(ConfigManager.Instance.Drone.HealthMax);
            }

            UpdateLabel();

            if (CanProduce && UnitCount < UnitCountMax && !IsActive)
            {
                StartCoroutine(Spawn());
            }
            
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void EnableCapturing(int side)
    {
        IsCapturing = true;
        var sprite = unitCountLabel.transform.parent.GetComponent<UISprite>();
        if (side >= 0)
        {
            sprite.color = GameManager.Instance.PlayerColors[side+1];
        }
        CaptureLeadPlayer = side;
    }

    private void DisableCapturing()
    {
        IsCapturing = false;
        var sprite = unitCountLabel.transform.parent.GetComponent<UISprite>();
        sprite.color = Color.white;
        CaptureLeadPlayer = -1;
    }

    public void UpdateLabel()
    {
        if (unitCountLabel != null)
        {
            if (!IsCapturing)
            {
                unitCountLabel.text = UnitCount.ToString();
            }
            else
            {
                unitCountLabel.text = UnitCount + "/" + MaxCaptureUnits;
            }
        }
    }

    public void AddBonusInitial()
    {
        AddBonusFromDrone((int)UnitCount);
    }

    private void AddBonusFromDrone()
    {
        AddBonusFromDrone(1);
    }

    private void AddBonusFromDrone(int multiplier)
    {
        Planet.AddBonusHp(ConfigManager.Instance.Drone.HealthMax * multiplier);
        if (Planet.Weapon != null)
        {
            Planet.Weapon.AddDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
        }
    }

    private void RemoveBonusFromDrone()
    {
        RemoveBonusFromDrone(1);
    }

    private void RemoveBonusFromDrone(int multiplier)
    {
        Planet.RemoveBonusHp(ConfigManager.Instance.Drone.HealthMax * multiplier);
        if (Planet.Weapon != null)
        {
            Planet.Weapon.RemoveDamage(ConfigManager.Instance.Drone.AttackDamage * multiplier);
        }
    }
    
    public void PutDroneInside(Drone drone)
    {
        if (drone.Owner.PlayerNumber == Owner.PlayerNumber)
        {
            UnitCount += 1;
            AddBonusFromDrone();
        }
        // if owners are different
        else
        {
            if (!IsCapturing)
            {
                if (UnitCount > 0)
                {
                    UnitCount -= 1;
                    RemoveBonusFromDrone();
                    Planet.RemoveBonusHpCurrent(ConfigManager.Instance.Drone.HealthMax);
                }
                else if (UnitCount == 0)
                {
                    EnableCapturing(drone.Owner.PlayerNumber);
                    UnitCount += 1;
                }
                else
                {
                    EnableCapturing(drone.Owner.PlayerNumber);
                    UnitCount = 1;
                }
            }
            else
            {
                if (CaptureLeadPlayer == drone.Owner.PlayerNumber)
                {
                    if (UnitCount < MaxCaptureUnits)
                    {
                        UnitCount += 1;
                    }
                    if (UnitCount >= MaxCaptureUnits)
                    {
                        DisableCapturing();
                        Planet.SetOwner(drone.Owner.PlayerNumber, drone.Owner.PlayerController);
                        UnitCount = MaxCaptureUnits;
//                        AddBonusFromDrone((int)unitCount);
                    }
                }
                else
                {
                    if (UnitCount > 0)
                    {
                        UnitCount -= 1;
                    }
                    else
                    {
                        EnableCapturing(drone.Owner.PlayerNumber);
                        UnitCount = 1;
                    }
                }
            }
        }
        UpdateLabel();
        drone.Die();
    }
    
    private void AddUiUnitCount()
    {
        Transform hpBars = GameObject.Find("UIBars").transform;
        GameObject labelPrefab = Resources.Load<GameObject>("UI/BaseUnitCount");
	    
        Vector2 newPosition = Trans.position;
        newPosition.y += GetComponent<MeshRenderer>().bounds.extents.y;
        GameObject assignedUnitCountObject = Instantiate(labelPrefab, newPosition, Trans.rotation, hpBars);
        assignedUnitCountObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        unitCountLabel = assignedUnitCountObject.transform.GetChild(0).GetComponent<UILabel>();
//		UISprite assignedHPbarSprite = assignedUIBarObject.GetComponent<UISprite>();
//		assignedHPbarSprite.SetAnchor(gameObject);
    }

    private IEnumerator Spawn()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        IsActive = true;
        yield return new WaitForSeconds(Delay);
        
        while (IsActive)
        {
            if (UnitCount >= UnitCountMax)
            {
                IsActive = false;
                yield break;
            }
            UnitCount += 1;
            UpdateLabel();
            AddBonusFromDrone();
            
            yield return new WaitForSeconds(Interval);
        }
    }
}
