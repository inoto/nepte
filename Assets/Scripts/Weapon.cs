using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool drawGizmos = false;

    private float waitTime;

    [Header("Attack")]
    public int damage = 20;
    public float radiusAttack = 3;

    [Header("Cache")]
    public Transform trans;
	private Unit unitComponent;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject laserMissilePrefab;

    public CollisionCircle collisionCircle;

    // TODO: move states and other big logic to drone

    // Use this for initialization
    void Awake ()
    {
        unitComponent = GetComponent<Unit>();
        trans = GetComponent<Transform>();
    }

	private void Start()
	{
        collisionCircle = new CollisionCircle(transform.position, radiusAttack, this);
		//CollisionManager.Instance.AddWeapon(collisionCircle);
	}

	private void Update()
	{
		collisionCircle.point = trans.position;
	}

    public void ReleaseLaserMissile(Vector3 newDestinationVector)
    {
		//GameObject laserMissileObject = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.SetParent(GameController.Instance.transform);
        //laserMissileObject.transform.localScale = droneParent.transform.localScale;

        GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, gameObject.transform.position, gameObject.transform.rotation);
        //laserMissileObject.transform.Rotate(new Vector3(0, 0, -90));
        //laserMissileObject.transform.position = transform.position;
        LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
        laserMissile.owner = unitComponent.droneComponent.owner;
		laserMissile.damage = damage;
    }

	public void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.3f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(collisionCircle.point, collisionCircle.radius);
		}
	}
}
