using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float waitTime;

    [Header("Attack")]
    public int damage = 20;
    public float radiusAttack = 3;

    [Header("Cache")]
	private Unit unitComponent;
	private IOwnable triggeredDrone;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject laserMissilePrefab;

    public CollisionCircle collisionCircle;

    // TODO: move states and other big logic to drone

    // Use this for initialization
    void Awake ()
    {
        unitComponent = GetComponent<Unit>();
    }

	private void Start()
	{
        collisionCircle = new CollisionCircle(transform.position, radiusAttack, this);
		//CollisionManager.Instance.AddWeapon(collisionCircle);
	}

    public void ReleaseLaserMissile(Vector3 newDestinationVector)
    {
		//GameObject laserMissileObject = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.SetParent(GameController.Instance.transform);
        //laserMissileObject.transform.localScale = droneParent.transform.localScale;

        GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.position = transform.position;
        LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
        laserMissile.owner = unitComponent.droneComponent.owner;
		laserMissile.damage = damage;
    }
}
