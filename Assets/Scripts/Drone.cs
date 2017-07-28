using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    private GameObject _instance;

    public int owner = 0;
    public int health = 100;
    private GameObject rallyPoint;
    //public Vector3 newPosition;
    public float speed = 0.2f;
	private float step;
    public Drone enemyDrone;
    private float attackRange = 2f;
    private int damage = 100;
    private bool isDead = false;


	// Use this for initialization
	void Start ()
    {
        
	}

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            enemyDrone.isDead = true;

            Destroy(gameObject);
        }
        //if (gotoPosition == transform.position)
        //{
        //    Vector3 rally = GetRallyPoint();
        //}

        // move
		float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, rallyPoint.transform.position, step);
	}

    public GameObject GetRallyPoint()
    {
        return rallyPoint;
    }

    public void SetRallyPoint(GameObject rally)
    {
        rallyPoint = rally;
    }

    void Attack()
    {
        enemyDrone.health -= damage;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isDead && owner != other.GetComponent<Drone>().owner)
        {
            enemyDrone = other.gameObject.GetComponent<Drone>();
            if (enemyDrone != null)
            {
                enemyDrone.isDead = true;
                Destroy(gameObject);
            }
        }
 
        //if (!other.GetComponent<Drone>().isDead)
        //    enemyDrone = other.gameObject;
        
        //if (enemyDrone != null)
        //{
        //    //StartCoroutine(WaitRandomForAttack());
        //    Attack();
        //}
	}

    IEnumerator WaitRandomForAttack()
    {
        float waitTime = Random.Range(0.3f, 0.9f);
        yield return new WaitForSeconds(waitTime);
        Attack();
    }

    void Spawn()
    {
        //_instance = Instantiate(gameObject, transform.position, transform.rotation);
    }

	//void OnCollisionEnter(Collision coll)
	//{
 //       Debug.Log("collision!");
	//}

    //void MoveOnCollision(Vector2 direction)
    //{
    //    //transform.position = Random
    //}


}
