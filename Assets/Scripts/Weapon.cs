using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
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

    public void ReleaseLaserMissile(Vector3 newDestinationVector)
    {
		//GameObject laserMissileObject = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.SetParent(GameController.Instance.transform);
        //laserMissileObject.transform.localScale = droneParent.transform.localScale;

        GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, droneParent.transform, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.position = transform.position;
        LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		laserMissile.owner = droneParent.owner;
		laserMissile.damage = damage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("drone") || other.gameObject.CompareTag("base"))
        {
            triggeredDrone = other.gameObject.GetComponent<IOwnable>();
            if (droneParent.enemy == triggeredDrone.GetGameObject()
                && triggeredDrone.IsActive())
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
        }
    }
}
