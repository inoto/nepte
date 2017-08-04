using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour, IOwnable
{
    public int owner;

    private float waitTime;

    [Header("Attack")]
    public int damage = 20;

    [Header("Cache")]
	private Drone droneParent;
	private CircleCollider2D weaponCollider;
	private IOwnable triggeredDrone;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject laserMissilePrefab;

    // TODO: move states and other big logic to drone

    // Use this for initialization
    void Start ()
    {
        droneParent = gameObject.transform.parent.gameObject.GetComponent<Drone>();
        weaponCollider = gameObject.GetComponent<CircleCollider2D>();
    }
	
	// Update is called once per frame
	//void Update ()
    //{
    //    if (mode != Mode.Attacking)
    //    {
    //        droneParent.canMove = true;
    //    }
    //    else
    //        droneParent.canMove = false;

    //    if (!enemy && mode != Mode.Idle)
    //    {
    //        //StopCoroutine("Attack");
    //        droneParent.EnterIdleMode();
    //    }
    //}

    public void ReleaseLaserMissile(Vector3 newDestinationVector)
    {
		GameObject laserMissileObject = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
        laserMissileObject.transform.SetParent(GameController.Instance.transform);
        laserMissileObject.transform.localScale = droneParent.transform.localScale;
        LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		laserMissile.owner = owner;
		laserMissile.damage = damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("unit"))
        {
            triggeredDrone = other.gameObject.GetComponent<IOwnable>();
            if (droneParent.enemy == triggeredDrone.GetGameObject())
            {
                droneParent.EnterAttackingMode();
            }
        }
    }

    void OnTriggetExit2D (Collider2D other)
    {
        triggeredDrone = null;
        if (other.gameObject == droneParent.enemy)
        {
            droneParent.EnterCombatMode(other.gameObject);
            //StopCoroutine("Attack");
        }
    }



    //IEnumerator Attack()
    //{
    //    while (enemy)
    //    {
    //        waitTime = Random.Range(0.5f, 1.5f);
    //        yield return new WaitForSeconds(waitTime);
    //        if (enemy)
    //        {
    //            ReleaseLaserMissile();
    //        }
    //        else
    //        {
    //            yield break;
    //        }
    //        // TODO: remove random to use slow ratation and attack after ration is completed
    //    }
    //}

	public int GetOwner()
	{
		return owner;
	}

	public void SetOwner(int newOwner)
	{
		owner = newOwner;
	}

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
