using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour, ITargetable, ICollidable
{
	[Header("Gizmos")]
	public bool showPath = false;
	public bool showRadius = false;

	[Header("Head")]
	public int owner;
    public int health = 100;
	public bool isDead;
	private PlayerController playerController;
	//public CollisionCircle collisionCircle;

	public enum Mode
    {
        Idle,
        MovingRally,
        Combat,
        Attacking
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

		//collisionCircle = new CollisionCircle(trans.position, gameObject.GetComponent<Quad>().size, this);
	}

    void Start ()
    {
		Activate();
    }

    private void OnEnable()
    {
		Activate();
    }

    void Activate()
    {
        health = 100;
        target = null;
        AssignMaterial();
		//CollisionManager.Instance.AddUnit(collisionCircle);
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
		playerRallyPoint = GameController.Instance.playerControllerObject[owner].GetComponent<PlayerController>().rallyPoint;
		EnterIdleMode();
		ResetRallyPoint();
		playerRallyPoint.OnRallyPointChanged += ResetRallyPoint;
		
	}

	private void OnDestroy()
    {
        Deactivate();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    void Deactivate()
    {
		playerRallyPoint.OnRallyPointChanged -= ResetRallyPoint;
		//CollisionManager.Instance.RemoveUnit(collisionCircle);
		//CollisionManager.Instance.RemoveCollidable(this);
		radar.gameObject.SetActive(false);
		weapon.gameObject.SetActive(false);
		playerController.playerUnitCount -= 1;
		PlayerController.unitCount -= 1;
	}

	void AssignMaterial()
	{
		mesh.material = materials[owner];
	}

    public void EnterCombatMode(ITargetable _target)
    {
        mode = Mode.Combat;
        if (target == _target)
            return;
		StopCoroutine("FollowPath");
		target = _target;
		radar.gameObject.SetActive(false);
		weapon.gameObject.SetActive(true);
	}

    public void EnterIdleMode()
    {
        mode = Mode.Idle;
		weapon.gameObject.SetActive(false);
		radar.gameObject.SetActive(true);
	}

    public void EnterAttackingMode()
    {
        mode = Mode.Attacking;
        StartCoroutine(Attack());
    }

	void Die()
	{
		Deactivate();
		ObjectPool.Recycle(gameObject);
		GameObject explosion = Instantiate(droneExplosionPrefab, trans.position, trans.rotation);
		explosion.transform.SetParent(GameController.Instance.transform);
	}

	void Rotate()
	{
		directionVector = (nextNode.worldPosition - (Vector2)trans.position).normalized;
		float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
		Quaternion qt = Quaternion.AngleAxis(angle - 90, Vector3.forward);
		trans.rotation = Quaternion.Slerp(trans.rotation, qt, Time.deltaTime * turnSpeed);
	}

	void Move()
	{
		float step = Time.deltaTime * speed * speedPercent;
		trans.position = Vector2.MoveTowards(trans.position, nextNode.worldPosition, step);
	}

	IEnumerator FollowPath()
	{
		mode = Drone.Mode.MovingRally;
		speedPercent = 1;

		while (true)
		{
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


			yield return null;
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

	public void ResetRallyPoint()
	{
		if (mode != Mode.Idle)
			return;
		if (mode != Mode.MovingRally)
			return;
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
		StopCoroutine("FollowPath");
		StartCoroutine("FollowPath");
	}

	IEnumerator Attack()
    {
        while (target != null)
        {
            float waitTime = Random.Range(0.5f, 1.5f);
            //directionVector = destinationTransform.position - trans.position;
            //Rotate();
            // TODO: remove random to use slow ratation and attack after ration is completed
            yield return new WaitForSeconds(waitTime);
            if (target != null)
            {
                //Rotate();
                ReleaseLaserMissile(target.GameObj.transform.position);
            }
            else
            {
				ResetRallyPoint();
				EnterIdleMode();
                yield break;
            }
        }
    }

	public void ReleaseLaserMissile(Vector3 newDestinationVector)
	{
		GameObject laserMissileObject = ObjectPool.Spawn(laserMissilePrefab, GameController.Instance.transform, trans.position, trans.rotation);
		//laserMissileObject.transform.position = transform.position;
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
	public CollisionType Type
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
}
