using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int owner;

    private float waitTime;

    [Header("Attack")]
    public float attackRadius = 0.2f;
    public float detectRadius = 0.3f;
    public int damage = 20;

    [Header("Cache")]
	private Drone droneParent;
	private CircleCollider2D weaponCollider;
    private GameObject enemy;
	private Drone droneTriggered;
    private Base baseTriggered;
    private GameObject laserMissile;

    [Header("Prefabs")]
    public GameObject laserMissilePrefab;

    enum Mode
    {
        Idle,
        Combat,
        Attacking
    }
    Mode mode;

    // Use this for initialization
    void Start ()
    {
        droneParent = gameObject.transform.parent.gameObject.GetComponent<Drone>();
        weaponCollider = gameObject.GetComponent<CircleCollider2D>();
        mode = Mode.Idle;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (mode != Mode.Attacking)
        {
            transform.position = droneParent.transform.position;
            transform.rotation = droneParent.transform.rotation;
            droneParent.canMove = true;
        }
        else
            droneParent.canMove = false;

        if (!enemy && mode != Mode.Idle)
        {
            //StopCoroutine("Attack");
            EnterIdleMode();
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "drone")
        {
            droneTriggered = other.gameObject.GetComponent<Drone>();
            if (mode == Mode.Idle)
            {
                if (droneTriggered
				&& other.gameObject != droneParent.gameObject
				&& droneTriggered.owner != owner)
                {
					EnterCombatMode(other.gameObject);
				} 
            }
            else if (mode == Mode.Combat)
            {
                if (enemy == droneTriggered.gameObject)
                {
                    EnterAttackingMode();
                }
            }
        }
        else if (other.gameObject.tag == "base")
        {
			if (mode == Mode.Idle)
			{
				baseTriggered = other.gameObject.GetComponent<Base>();
				if (baseTriggered
					&& baseTriggered.owner != owner)
				{
					EnterCombatMode(other.gameObject);
				}
			}
			else if (mode == Mode.Combat)
			{
				droneTriggered = other.gameObject.GetComponent<Drone>();
                if (enemy == baseTriggered.gameObject)
				{
					EnterAttackingMode();
				}
			}
        }
    }

    void OnTriggetExit2D (Collider2D other)
    {
        droneTriggered = null;
        if (other.gameObject == enemy)
        {
            mode = Mode.Combat;
            StopCoroutine("Attack");
        }
    }

    void EnterCombatMode(GameObject newEnemy)
    {
        mode = Mode.Combat;
        enemy = newEnemy;
        droneParent.SetRallyPoint(newEnemy.gameObject);
        weaponCollider.radius = attackRadius;
    }

    void EnterIdleMode()
    {
        mode = Mode.Idle;
        if (droneParent.playerRallyPoint)
            droneParent.SetRallyPoint(droneParent.playerRallyPoint);
        else
            droneParent.SetRallyPoint(FindObjectOfType<Battleground>().gameObject);
        weaponCollider.radius = detectRadius;
    }

    void EnterAttackingMode()
    {
        mode = Mode.Attacking;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (enemy)
        {
            waitTime = Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(waitTime);
            if (enemy)
            {
                laserMissile = Instantiate(laserMissilePrefab, gameObject.transform.position, gameObject.transform.rotation);
                laserMissile.GetComponent<LaserMissile>().gotoPosition = enemy.transform.position;
                laserMissile.GetComponent<LaserMissile>().owner = owner;
                laserMissile.GetComponent<LaserMissile>().damage = damage;
            }
            else
            {
                yield break;
            }
        }
    }
}
