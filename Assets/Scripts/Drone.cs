using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public GameObject droneExplosion;

    public int owner = 0;

    public int health = 100;

    private GameObject rallyPoint;
    public Vector3 gotoPosition;

    public float speed = 0.2f;
    public float angle;
    public float rotationSpeed = 50f;
	private float step;

    public Drone enemyDrone;

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
        
	}

    // Update is called once per frame
    void Update()
    {
        //if (health <= 0)
        //{
        //    enemyDrone.isDead = true;

        //    Destroy(gameObject);
        //}



        //if ((state != UnitState.CollideWithAlly) && (state != UnitState.CollideWithEnemy))
        //{
        //state = UnitState.Moving;

        if (!isCollideWithAlly)
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
        //else if (state == UnitState.CollideWithEnemy)
        //{
            
        //}
	}

    public GameObject GetRallyPoint()
    {
        return rallyPoint;
    }

    public void SetRallyPoint(GameObject rally)
    {
        rallyPoint = rally;
    }

    void KillEnemy()
    {
        enemyDrone.health -= damage;
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

	void OnCollisionEnter2D (Collision2D other)
	{
		//if (other.gameObject.tag == "dummy")
			//return;
        
        if (owner == other.gameObject.GetComponent<Drone>().owner)
        {
            isCollideWithAlly = true;
            //transform.position = Vector2.MoveTowards(transform.position, rallyPoint.transform.position, step);
            Vector2 pos = other.contacts[0].point - (Vector2)transform.position;
            pos = -pos.normalized;
            //GetComponent<Rigidbody>().AddForce(pos * 3);
            transform.position = Vector2.MoveTowards(transform.position, pos, step*3);
        }
        else
        {
            isCollideWithAlly = false;
			if (!isDead)
			{
				enemyDrone = other.gameObject.GetComponent<Drone>();
				if (enemyDrone != null)
				{
					enemyDrone.isDead = true;
                    Instantiate(droneExplosion, transform.transform.position, transform.rotation); 
					Destroy(gameObject);
				}
			}
        }

	}

	void OnCollisionExit2D(Collision2D other)
	{
        //state = UnitState.Moving;
        isCollideWithAlly = false;
	}

    IEnumerator WaitRandomForAttack()
    {
        float waitTime = Random.Range(0.3f, 0.9f);
        yield return new WaitForSeconds(waitTime);
        KillEnemy();
    }

}
