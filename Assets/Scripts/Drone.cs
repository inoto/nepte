using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject droneExplosion;

    public int owner = 0;

    public int health = 100;

    public GameObject rallyPoint;
    public Vector3 gotoPosition;

    public float speed = 0.2f;
    public float angle;
    public float rotationSpeed = 50f;
	private float step;

    public float bodyRadius;
    public float attackRadius;
    public float detectRadius;

    public Drone enemyDrone;
    private GameObject weaponChild;

    public GameObject weapon;

    public GameObject laserMissile;
    private float attackRange = 2f;
    private float attackSpeed = 2f;
    private int damage = 100;

    private bool isDead = false;
    private bool isCollideWithAlly = false;

    enum UnitState
    {
        Idle,
        CollideWithAlly,
        CollideWithEnemy,
        Moving,
        Dead
    }
    UnitState state;

	// Use this for initialization
	void Start ()
    {
        weapon = Instantiate(weapon, transform.position, transform.rotation);
        weapon.transform.SetParent(transform);
        weapon.GetComponent<Weapon>().owner = owner;
    }

    // Update is called once per frame
    void Update()
    {
        //if (health <= 0)
        //{
        //    enemyDrone.isDead = true;

        //    Destroy(gameObject);
        //}
        

        if ((!isCollideWithAlly) && (enemyDrone == null))
        {
            gotoPosition = rallyPoint.transform.position - transform.position;

            // rotation if needed
            angle = Mathf.Atan2(gotoPosition.y, gotoPosition.x) * Mathf.Rad2Deg;
            Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, qt, Time.deltaTime * rotationSpeed);

            // movement
            step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, rallyPoint.transform.position, step);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Attack(mousePosition);
        }

    }

    public GameObject GetRallyPoint()
    {
        return rallyPoint;
    }

    public void SetRallyPoint(GameObject rally)
    {
        rallyPoint = rally;
    }

    void Kill(GameObject obj)
    {
        //enemyDrone.health -= damage;
        obj.GetComponent<Drone>().isDead = true;
        Instantiate(droneExplosion, transform.position, transform.rotation);
        Destroy(obj);
        Destroy(obj);
    }

    void Attack(Vector3 pos)
    {
        GameObject instance = Instantiate(laserMissile, transform.position, transform.rotation);
        instance.GetComponent<LaserMissile>().gotoPosition = pos;
        instance.GetComponent<LaserMissile>().owner = owner;
    }

    //void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.tag == "dummy")
    //        return;

    //    if (owner == other.gameObject.GetComponent<Drone>().owner)
    //        state = UnitState.CollideWithAlly;
    //    else
    //    {
    //        state = UnitState.CollideWithEnemy;
    //        enemyDrone = other.gameObject.GetComponent<Drone>();
    //    }
            
    //}

	void OnTriggerEnter2D (Collider2D other)
	{
        if (other.gameObject.tag == "missile")
        {
            if (other.gameObject.GetComponent<LaserMissile>().owner != owner)
            {
                Destroy(other.gameObject);
                Kill(gameObject);
                
            }
                
        }

        //other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        //Vector3 vec = transform.Translate(other.Distance(other), transform.forward);

        //Vector2 pos = hit.point - transform.position;
        //pos = -pos.normalized;
        //transform.position = Vector2.MoveTowards(transform.position, pos, step * 4);
        

        /*if (owner == other.gameObject.GetComponent<Drone>().owner)
        {
            isCollideWithAlly = true;
            //transform.position = Vector2.MoveTowards(transform.position, rallyPoint.transform.position, step);
            Vector2 pos = other.contacts[0].point - (Vector2)transform.position;
            pos = -pos.normalized;
            transform.position = Vector2.MoveTowards(transform.position, pos, step*4);
        }
        else
        {
            
            isCollideWithAlly = false;
			if (!other.gameObject.GetComponent<Drone>().isDead)
			{
				enemyDrone = other.gameObject.GetComponent<Drone>();
                //Attack(other.transform.position);
                //if (enemyDrone != null)
				//{
                //    bool randBool = (Random.Range(0,100) >= 50);
                //    if (randBool)
                //        Kill(gameObject); // else it's a miss
				//}
            }
        }*/

    }

	/*void OnCollisionExit2D(Collision2D other)
	{
        //state = UnitState.Moving;
        isCollideWithAlly = false;
	}*/

}
