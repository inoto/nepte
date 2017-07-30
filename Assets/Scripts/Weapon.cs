using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int owner;

    public float attackRadius = 2;
    public float detectRadius = 3;

    public GameObject laserMissilePrefab;

    private GameObject gameObjectParent;
    private CircleCollider2D weaponCollider;

    private Drone enemyDrone;
    private Drone targetDrone;

    private float waitTime;

    enum Mode
    {
        Idle,
        Attack
    }
    Mode mode;

    // Use this for initialization
    void Start ()
    {
        gameObjectParent = gameObject.transform.parent.gameObject;
        weaponCollider = gameObject.GetComponent<CircleCollider2D>();
        mode = Mode.Idle;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = gameObjectParent.transform.position;
        transform.rotation = gameObjectParent.transform.rotation;
        if (!enemyDrone && mode == Mode.Attack)
        {
            Debug.Log("enter idle mode");
            EnterIdleMode();
            GameObject rallyPoint = gameObjectParent.GetComponent<Drone>().rallyPoint;
            gameObjectParent.GetComponent<Drone>().SetRallyPoint(enemyDrone.gameObject);
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (mode == Mode.Idle)
        {
            targetDrone = other.gameObject.GetComponent<Drone>();
            if (targetDrone && other.gameObject != gameObjectParent && targetDrone.owner != owner)
            {
                enemyDrone = targetDrone;
                EnterAttackMode();
            }
        }
        else if (mode == Mode.Attack)
        {
            targetDrone = other.gameObject.GetComponent<Drone>();
            if (enemyDrone == targetDrone)
            {
                
                StartCoroutine(Attack());
            }
            gameObjectParent.GetComponent<Drone>().SetRallyPoint(enemyDrone.gameObject);
        }
    }

    void OnTriggetExit2D (Collider2D other)
    {
        targetDrone = null;
    }

    void EnterAttackMode()
    {
        mode = Mode.Attack;
        weaponCollider.radius = attackRadius;
    }

    void EnterIdleMode()
    {
        mode = Mode.Idle;
        weaponCollider.radius = detectRadius;
    }

    IEnumerator Attack()
    {
        waitTime = Random.Range(0.3f, 0.9f);
        yield return new WaitForSeconds(waitTime);
        GameObject instance = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
        instance.GetComponent<LaserMissile>().gotoPosition = enemyDrone.gameObject.transform.position;
        instance.GetComponent<LaserMissile>().owner = owner;

    }
}
