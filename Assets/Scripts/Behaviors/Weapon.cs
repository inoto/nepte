using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public bool showRadius = false;

    public float radius = 3.5f;
	public float attackSpeed = 1;
	public int damage = 40;
	public ITargetable target;

	[SerializeField] private GameObject missilePrefab;
	
	[Header("Components")]
    public Transform trans;
    public Owner owner;
    public Mover mover;
    public CollisionCircle collision;

	// Use this for initialization
	void Awake()
	{
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	}

	private void Start()
	{
        collision = new CollisionCircle(this, trans, mover, owner);
		CollisionManager.Instance.AddCollidable(collision);
	}
	
	public void ReleaseLaserMissile(Vector3 newDestinationVector)
	{
//		GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
		GameObject laserMissileObject = Instantiate(missilePrefab, trans.position, trans.rotation);
		laserMissileObject.transform.parent = GameController.Instance.transform;
		LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		//laserMissile.owner = owner;
		laserMissile.damage = damage;
		laserMissile.target = target;
	}

	public void Activate(ITargetable newTarget)
	{
		target = newTarget;
		showRadius = true;
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
}
