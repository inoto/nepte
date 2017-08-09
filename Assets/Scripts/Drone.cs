﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, IOwnable
{
    public int owner;

    public int health = 100;

    public bool isDead = false;
    public GameObject cell;

    public Rect bounds;

    public enum Mode
    {
        Idle,
        Moving,
        Combat,
        Attacking
    }
    public Mode mode;

    [Header("Destination")]
    // TODO: change GameObject to Vector3 for rallypoints
    public Transform destinationTransform;
    public GameObject playerRallyPoint;
    public Vector3 directionVector;

    [Header("Components")]
    // TODO: use cached Transform
    //private Transform trans;
    private Unit unitComponent;
    private Weapon weaponComponent;
    private Radar radarComponent;

    private Collider2D droneCollider;
    public LaserMissile triggeredLaserMissile;
    public IOwnable triggeredDrone;
    public GameObject enemy;
    public List<GameObject> attackers = new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject droneExplosionPrefab;

    public static Sprite[] spriteSet;

    // Use this for initialization
    void Start ()
    {
        weaponComponent = GetComponent<Weapon>();
        radarComponent = GetComponent<Radar>();
        unitComponent = GetComponent<Unit>();

        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        playerRallyPoint = null;
    }

    void Initialize()
    {
        playerRallyPoint = GameController.Instance.playerControllerObject[owner].GetComponent<PlayerController>().rallyPoint.gameObject;
        isDead = false;
        health = 100;
        enemy = null;
        triggeredLaserMissile = null;
        triggeredDrone = null;
        destinationTransform = null;
        EnterIdleMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (destinationTransform != null && mode != Mode.Attacking)
        {
            //directionVector = currentRallyPoint.transform.position - transform.position;

            //Rotate();

            //Move();
        }

        if (enemy == null && mode != Mode.Idle)
        {
                EnterIdleMode();
        }

        //if ((Object)triggeredDrone != null)
        //{
        //    directionVector = triggeredDrone.GetGameObject().transform.position - this.transform.position;
        //    directionVector = -directionVector;
        //    transform.position = Vector2.MoveTowards(transform.position, directionVector, step);
        //}
        // TODO: move change of transform.position to the end after calculations
    }

    //void Rotate()
    //{
    //    angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
    //    Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, qt, Time.deltaTime * rotationSpeed);
    //}

    //void Move()
    //{
    //    step = speed * Time.deltaTime;
    //    transform.position = Vector2.MoveTowards(transform.position, destinationTransform.position, step);
    //}

    public void ResetRallyPoint()
    {
        if (playerRallyPoint)
            destinationTransform = playerRallyPoint.transform;
        else
            destinationTransform = FindObjectOfType<Battleground>().transform;
    }

    void Die()
    {
        isDead = true;
        ObjectPool.Recycle(gameObject);
        GameObject tmpObject = Instantiate(droneExplosionPrefab, transform.position, transform.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
        UnbindAttackers();
    }

    void UnbindAttackers()
    {
        foreach (GameObject attacker in attackers)
        {
            attacker.GetComponent<Drone>().enemy = null;
        }
        attackers.Clear();
    }

    public bool IsIdle
    {
        get
        {
            if (mode == Mode.Idle)
                return true;
            else
                return false;
        }
    }

    public bool HasNoEnemy
    {
        get
        {
            if (enemy == null)
                return true;
            else
                return false;
        }
    }

    public void EnterCombatMode(GameObject newEnemy)
    {
        mode = Mode.Combat;
        if (newEnemy == enemy)
            return;
        enemy = newEnemy;
        destinationTransform = enemy.transform;
        //radarChild.gameObject.SetActive(false);
        //weaponChild.gameObject.SetActive(true);
    }

    public void EnterIdleMode()
    {
        mode = Mode.Idle;
        ResetRallyPoint();
        //radarChild.gameObject.SetActive(true);
        //weaponChild.gameObject.SetActive(false);
    }

    public void EnterAttackingMode()
    {
        mode = Mode.Attacking;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (enemy != null)
        {
            float waitTime = Random.Range(0.5f, 1.5f);
            directionVector = destinationTransform.position - transform.position;
            //Rotate();
            // TODO: remove random to use slow ratation and attack after ration is completed
            yield return new WaitForSeconds(waitTime);
            if (enemy != null)
            {
                //Rotate();
                weaponComponent.ReleaseLaserMissile(enemy.transform.position);
            }
            else
            {
                yield break;
            }
        }
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("missile"))
        {
            triggeredLaserMissile = other.gameObject.GetComponent<LaserMissile>();
            if (triggeredLaserMissile.owner != owner
                && !triggeredLaserMissile.wasExecuted
                && !isDead)
            {
                triggeredLaserMissile.wasExecuted = true;
                health -= triggeredLaserMissile.damage;
                if (health <= 0)
                {
                    Die();
                }
            }

        }
        else if (other.gameObject.CompareTag("drone") || other.gameObject.CompareTag("base"))
        {

            triggeredDrone = other.gameObject.GetComponent<IOwnable>();
            //Debug.Log(gameObject + " triggered with " + triggeredDrone.GetGameObject());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredDrone = null;

    }

    public void AddAttacker(GameObject newObj)
    {
        attackers.Add(newObj);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public int GetOwner()
    {
        return owner;
    }

    public void SetOwner(int newOwner)
    {
        owner = newOwner;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
