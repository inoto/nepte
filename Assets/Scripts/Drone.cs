using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public int owner = 0;
    public int health = 100;
    public Vector3 gotoPosition;
    public float speed = 0.2f;
	private float step;
    public GameObject enemy;
    private float attackRange = 2f;
    private int damage = 100;


	// Use this for initialization
	void Start ()
    {
        gotoPosition = GetRallyPoint();
	}

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        //if (gotoPosition == transform.position)
        //{
        //    Vector3 rally = GetRallyPoint();
        //}

        // move
		float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, gotoPosition, step);
	}

    Vector3 GetRallyPoint()
    {
        return GameObject.Find("Rpoint").transform.position;
    }

    void Attack()
    {
        if (gameObject.GetInstanceID() >= enemy.GetInstanceID())
        {
            enemy.GetComponent<Drone>().health -= damage;
        }
    }

    void FindEnemiesInRange(float radius)
    {
        
    }

	void OnTriggerEnter2D(Collider2D other)
	{
        enemy = other.gameObject;
        if (enemy != null)
        {
            Attack();
        }
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
