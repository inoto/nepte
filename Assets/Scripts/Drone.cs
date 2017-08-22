using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, ITargetable, ICollidable
{
    public bool followRally = false;
    public bool followTarget = false;

	[Header("Gizmos")]
	public bool showPath = false;
	public bool showRadius = false;

	[Header("Head")]
	public int owner;
    public int health = 100;
	private PlayerController playerController;
	//public CollisionCircle collisionCircle;

	public enum Mode
    {
        Idle,
        FollowRally,
        Combat,
        FollowTarget,
        Attacking,
        Died
    }
    public Mode mode;

	[Header("Cache")]
	public Transform trans;
	public MeshRenderer mesh;
	public Radar radar;
	public Weapon weapon;

	[Header("Collision")]
	public float radius = 1f;
	public float radiusHard = 0.5f;
	public CollisionType cType = CollisionType.Drone;
    public TargetType tType = TargetType.Drone;

	[Header("Move")]
	public RallyPoint playerRallyPoint;
	public Vector2 directionVector;
	public Vector2 destinationPoint;
	public float speed = 2;
	public float speedPercent = 1;
	public float turnSpeed = 3;
	public Node node;
	public Node[] nextNodes = new Node[8];
	public Node nextNode;
	public Node tmpNode;
	public Node destinationNode;

	[Header("Attack")]
	public int damage = 50;
	public ITargetable target;
	[SerializeField]
	private GameObject laserMissilePrefab;

    [Header("Explosion")]
    [SerializeField]
    private GameObject droneExplosionPrefab;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
		radar = trans.GetChild(0).gameObject.GetComponent<Radar>();
		weapon = trans.GetChild(1).gameObject.GetComponent<Weapon>();

		radius = gameObject.GetComponent<Quad>().size;
		radiusHard = radius / 2 + radius / 5;
	}

    void Update ()
    {
        if (health <= 0)
        {
			GameObject explosion = Instantiate(droneExplosionPrefab, trans.position, trans.rotation);
			explosion.transform.SetParent(GameController.Instance.transform);
            Die();
        }
    }

    private void OnEnable()
    {
		Activate();
    }

    void Activate()
    {
        health = 100;
        target = null;
		CollisionManager.Instance.AddCollidable(this);
		radar.gameObject.SetActive(true);
		weapon.gameObject.SetActive(false);
		PlayerController.unitCount += 1;
		node = Grid.Instance.NodeFromWorldPoint(trans.position);
	}

	public void ActivateWithOwner()
	{
		playerController = trans.parent.gameObject.GetComponent<PlayerController>();
		playerController.playerUnitCount += 1;
		EnterIdleMode();
		NewDestination();
		AssignMaterial();
	}

    private void OnDestroy()
    {
        Die();
    }

	void AssignMaterial()
	{
        mesh.sharedMaterial = materials[owner];
	}

    public void EnterCombatMode(ITargetable _target)
    {
        StopCoroutine("FollowRally");
        mode = Mode.Combat;
        if (target == _target)
            return;
		target = _target;
		//radar.gameObject.SetActive(false);
		weapon.gameObject.SetActive(true);
        playerRallyPoint.OnRallyPointChanged -= NewDestination;
        NewTarget();
	}

    public void EnterIdleMode()
    {
        mode = Mode.Idle;
		weapon.gameObject.SetActive(false);
		radar.gameObject.SetActive(true);
        playerRallyPoint.OnRallyPointChanged += NewDestination;
	}

    public void EnterAttackingMode()
    {
        mode = Mode.Attacking;
        StopCoroutine("FollowTarget");
        //weapon.gameObject.SetActive(false);
        StartCoroutine(Attack());
    }

	void Die()
	{
		StopCoroutine("FollowRally");
		StopCoroutine("FollowTarget");
        mode = Mode.Died;
        playerRallyPoint.OnRallyPointChanged -= NewDestination;
        CollisionManager.Instance.RemoveCollidable(this);
        target = null;
		radar.gameObject.SetActive(false);
		weapon.gameObject.SetActive(false);
		playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
        ObjectPool.Recycle(gameObject);
	}

	void Rotate()
	{
		directionVector = (nextNode.worldPosition - (Vector2)trans.position).normalized;
		float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
		Quaternion qt = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		trans.rotation = Quaternion.Slerp(trans.rotation, qt, Time.deltaTime * turnSpeed);
	}

	void RotateToTarget()
	{
        directionVector = (target.GameObj.transform.position - trans.position).normalized;
		float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
		Quaternion qt = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		trans.rotation = Quaternion.Lerp(trans.rotation, qt, Time.deltaTime * turnSpeed);
	}

	void Move()
	{
		float step = Time.deltaTime * speed * speedPercent;
		trans.position = Vector2.MoveTowards(trans.position, nextNode.worldPosition, step);
	}

    public void NewTarget()
    {
        destinationPoint = target.GameObj.transform.position;
		StartCoroutine("FollowTarget");
    }

	public void NewDestination()
	{
		destinationPoint = playerRallyPoint.gameObject.transform.position;
		destinationNode = Grid.Instance.NodeFromWorldPoint(destinationPoint);
		if (node == destinationNode)
		{
			EnterIdleMode();
			return;
		}
		DefineNextNode(node);
		if (node == nextNode)
		{
			EnterIdleMode();
			return;
		}
		StopCoroutine("FollowRally");
        if (mode != Mode.Died)
		    StartCoroutine("FollowRally");
	}

	IEnumerator FollowRally()
	{
        mode = Mode.FollowRally;
		speedPercent = 1;

		while (true)
		{
            if (mode != Mode.FollowRally || mode == Mode.Died)
				yield break;
			tmpNode = Grid.Instance.NodeFromWorldPoint(trans.position);
			if (tmpNode != node)
			{
				node = tmpNode;
			}
			if (node.worldPosition == destinationPoint)
			{
				EnterIdleMode();
				yield break;
			}
			if (nextNode != node && nextNode != null)
			{
				Rotate();
				Move();
			}
			else
			{
				Rotate();
				Move();
				DefineNextNode(nextNode);
			}
			if (node == destinationNode)
			{
				EnterIdleMode();
				yield break;
			}
            yield return new WaitForSeconds(0.01f);
		}
	}

    IEnumerator FollowTarget()
    {
        mode = Mode.FollowTarget;
        speedPercent = 0.75f;
        while (true)
        {
            if (mode != Mode.FollowTarget || mode == Mode.Died)
				yield break;
			tmpNode = Grid.Instance.NodeFromWorldPoint(trans.position);
            if (tmpNode != node)
            {
                node = tmpNode;
            }
            nextNode = Grid.Instance.NodeFromWorldPoint(target.GameObj.transform.position);

            if (target.targetableType == TargetType.Drone)
            {
                if (target.DroneObj.mode != Mode.Died && mode != Mode.Died)
                {
                    RotateToTarget();
                    Move();
                }
                else
                {
                    target = null;
                    NewDestination();
                    EnterIdleMode();
                    yield break;
                }
            }
            else if (target.targetableType == TargetType.Base)
			{
                if (!target.BaseObj.isDead && mode != Mode.Died)
				{
					RotateToTarget();
					Move();
				}
				else
				{
					target = null;
					NewDestination();
					EnterIdleMode();
					yield break;
				}
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

	void DefineNextNode(Node newNextNode)
	{
		int closestNodeIndex = 0;
		for (int i = 0; i < newNextNode.neigbours.Length; i++)
		{
			if (newNextNode.neigbours[i] != null)
			{
				if (newNextNode.neigbours[i].distance[owner] < newNextNode.neigbours[closestNodeIndex].distance[owner])
				{
					closestNodeIndex = i;//nextNodes.IndexOf(n);//Array.IndexOf(nextNodes, n);
				}
			}
		}
		nextNode = newNextNode.neigbours[closestNodeIndex];
	}

	IEnumerator Attack()
    {
        while (true)
        {
            if (mode != Mode.Attacking || mode == Mode.Died)
				yield break;
            float waitTime = Random.Range(0.5f, 1.5f);
            // TODO: remove random to use slow rotation and attack after rotation is completed
            if (target.targetableType == TargetType.Drone)
            {
                if (target.DroneObj.mode != Mode.Died && mode != Mode.Died)
                {
                    RotateToTarget();
                    ReleaseLaserMissile(target.DroneObj.trans.position);
                }
                else
                {
                    target = null;
					EnterIdleMode();
					NewDestination();
                    yield break;
                }
            }
            else if (target.targetableType == TargetType.Base)
			{
                if (!target.BaseObj.isDead && mode != Mode.Died)
				{
                    RotateToTarget();
					ReleaseLaserMissile(target.BaseObj.trans.position);
				}
				else
				{
					target = null;
					EnterIdleMode();
                    NewDestination();
					yield break;
				}
                
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

	public void ReleaseLaserMissile(Vector3 newDestinationVector)
	{
        //GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
        GameObject laserMissileObject = Instantiate(laserMissilePrefab, trans.position, trans.rotation);
        laserMissileObject.transform.parent = GameController.Instance.transform;
		LaserMissile laserMissile = laserMissileObject.GetComponent<LaserMissile>();
		laserMissile.destinationVector = newDestinationVector;
		laserMissile.owner = owner;
		laserMissile.damage = damage;
		laserMissile.target = target;
	}

	public Drone DroneObj
	{
		get { return this; }
	}
	public Base BaseObj
    {
        get { return null; }
    }
	public GameObject GameObj
	{
		get { return gameObject; }
	}
    public TargetType targetableType
    {
        get { return tType; }
    }

    public bool IsDied
    {
        get
        {
            if (mode == Mode.Died)
                return true;
            else
                return false;
        }                
    }

	public void OnDrawGizmos()
	{
		if (showPath)
		{
			if (nextNode != null)
			{
				Color newColor = Color.gray;
				newColor.a = 0.3f;
				Gizmos.color = newColor;
				Gizmos.DrawLine(trans.position, nextNode.worldPosition);

				Gizmos.color = newColor;
				Gizmos.DrawCube(nextNode.worldPosition, nextNode.rect.size * (nextNode.rect.size.x - 0.05f));
			}
		}
		if (showRadius)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}

	public int InstanceId
	{
		get { return gameObject.GetInstanceID(); }
	}
	public Vector2 Point
	{
		get { return trans.position; }
		set { trans.position = value; }
	}
	public float Radius
	{
		get { return radius; }
	}
	public float RadiusHard
	{
		get { return radiusHard; }
	}
	public CollisionType collisionType
	{
		get { return cType; }
	}
	public bool Active
	{
		get { return gameObject.activeSelf; }
	}
	public GameObject GameObject
	{
		get { return gameObject; }
	}
	public Drone drone
	{
		get { return this; }
	}
    public Base bas
    {
        get { return null; }
    }
}
