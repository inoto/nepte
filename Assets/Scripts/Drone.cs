using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public int owner;

    public int health = 100;

	public bool isDead = false;
	public bool canMove = true;

    [Header("Destination")]
    public GameObject currentRallyPoint;
    public GameObject playerRallyPoint;
    public Vector3 gotoPosition;
    public Vector3 pos;

    [Header("Move")]
    public float speed = 0.2f;
    public float angle;
    public float rotationSpeed = 50f;
	private float step;

    [Header("Cache")]
    public LaserMissile laserMissileTriggered;
    public Drone droneTriggered;
    private GameObject weaponChild;

    [Header("Prefabs")]
    public GameObject droneExplosionPrefab;
    public GameObject weaponPrefab;
    public GameObject markerPrefab;
	// Use this for initialization
	void Start ()
    {
        transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        weaponChild = Instantiate(weaponPrefab, transform.position, transform.rotation);
        weaponChild.transform.SetParent(transform);
        weaponChild.GetComponent<Weapon>().owner = owner;
        if (playerRallyPoint)
            currentRallyPoint = playerRallyPoint;
        else
            currentRallyPoint = FindObjectOfType<Battleground>().gameObject;
        //markerPrefab = Instantiate(markerPrefab, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Kill();
        }

        if (currentRallyPoint)
        {
            if (currentRallyPoint.transform.position == transform.position)
                canMove = false;

            if (canMove)
            {
                gotoPosition = currentRallyPoint.transform.position - transform.position;

                Rotate();

                Move();
            }
        }
        if (droneTriggered)
        {
            pos = droneTriggered.transform.position - transform.position;
            pos = -pos.normalized;
            transform.position = Vector2.MoveTowards(transform.position, pos, step);
        }

    }

    void Rotate()
    {
		angle = Mathf.Atan2(gotoPosition.y, gotoPosition.x) * Mathf.Rad2Deg;
		Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, qt, Time.deltaTime * rotationSpeed);
    }

    void Move()
    {
		step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, currentRallyPoint.transform.position, step);
    }

    public GameObject GetRallyPoint()
    {
        return currentRallyPoint;
    }

    public void SetRallyPoint(GameObject newRallyPoint)
    {
        currentRallyPoint = newRallyPoint;
        if (!canMove)
            canMove = true;
    }

    void Kill()
    {
        isDead = true;
        Instantiate(droneExplosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

	void OnTriggerEnter2D (Collider2D other)
	{
        if (other.gameObject.tag == "missile")
        {
            laserMissileTriggered = other.gameObject.GetComponent<LaserMissile>();
            if (laserMissileTriggered.owner != owner
                && !laserMissileTriggered.wasExecuted
                && !isDead)
            {
                laserMissileTriggered.wasExecuted = true;
                health -= laserMissileTriggered.damage;
            }
                
        }
        else if (other.gameObject.tag == "drone")
        {
            //pos = other.transform.position - transform.position;
			//pos = -pos.normalized;
			//transform.position = Vector2.MoveTowards(transform.position, pos, step);
            droneTriggered = other.gameObject.GetComponent<Drone>();
		}

        /*if (owner == other.gameObject.GetComponent<Drone>().owner)
        {
            isCollideWithAlly = true;
            //transform.position = Vector2.MoveTowards(transform.position, rallyPoint.transform.position, step);
            Vector2 pos = other.contacts[0].point - (Vector2)transform.position;
            pos = -pos.normalized;
            transform.position = Vector2.MoveTowards(transform.position, pos, step);
        }*/

    }

    void OnTriggerExit2D(Collider2D other)
	{
        droneTriggered = null;
	}

}
