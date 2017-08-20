using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public bool showBodyRadius = false;
	public bool showRadarRadius = false;
	public bool showWeaponRadius = false;

    private PlayerController playerController;
    public Transform trans;

    [Header("Destination")]
	public RallyPoint playerRallyPoint;
	public Vector2 directionVector;
	public Vector2 destinationPoint;

    [Header("Move")]
    public float speed = 2;
    public float speedPercent = 1;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;
    public float radius = 0.5f;
    public float radiusHard = 0.5f;

	[Header("Attack")]

	[Header("Nodes")]
    public Node node;
    public Node[] nextNodes = new Node[8];
	public Node nextNode;
	public Node tmpNode;
    public Node destinationNode;

    public Drone droneComponent;
    //public Radar radarComponent;
	//public Weapon weaponComponent;


    public CollisionCircle collisionCircle;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        droneComponent = GetComponent<Drone>();
        //radarComponent = GetComponent<Radar>();
		//weaponComponent = GetComponent<Weapon>();

        radius = gameObject.GetComponent<Quad>().size;
        radiusHard = radius/2+radius/5;

    }

    void OnEnable()
	{
        Activate();
		//CollisionManager.Instance.AddUnit(this);
	}

    void Activate()
    {
        node = Grid.Instance.NodeFromWorldPoint(trans.position);
	
        PlayerController.unitCount += 1;
    }

    public void ActivateWithOwner()
    {
        playerController = transform.parent.gameObject.GetComponent<PlayerController>();
        playerController.playerUnitCount += 1;
        playerRallyPoint = GameController.Instance.playerControllerObject[droneComponent.owner].GetComponent<PlayerController>().rallyPoint;
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
        //CollisionManager.Instance.RemoveUnit(collisionCircle);
	}

    private void Update()
    {
    }

    void Deactivate()
    {
        playerRallyPoint.OnRallyPointChanged -= ResetRallyPoint;

        //QuadTree.Instance.qtree.Remove(gameObject);
        playerController.playerUnitCount -= 1;
        PlayerController.unitCount -= 1;
    }

	IEnumerator FollowPath()
    {
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
				droneComponent.EnterIdleMode();
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
				droneComponent.EnterIdleMode();
				yield break;
            }


            yield return null;
        }
    }

	void DefineNextNode(Node newNextNode)
	{
        //List<Node> closestToDestination = new List<Node>();
        //nextNodes = destNode.neigbours; //.Instance.GetNeighbours(destNode);

        int closestNodeIndex = 0;
        for (int i = 0; i < newNextNode.neigbours.Length; i++)
        //foreach (Node n in nextNodes)
        {
            if (newNextNode.neigbours[i] != null)
            {
                if (newNextNode.neigbours[i].distance[droneComponent.owner] < newNextNode.neigbours[closestNodeIndex].distance[droneComponent.owner])
                {
                    //Debug.Log("node " + nextNodes[i].gridX + "," + nextNodes[i].gridY + " < node " + nextNodes[closestNodeIndex].gridX + "," + nextNodes[closestNodeIndex].gridY);
                    closestNodeIndex = i;//nextNodes.IndexOf(n);//Array.IndexOf(nextNodes, n); 
                }
            }
            //if (n.distance[droneComponent.owner] >= node.distance[droneComponent.owner])
            //continue;
            //if (!n.suitable[droneComponent.owner])
            //continue;
            //nextNode = n;
            //closestToDestination.Add(n);
        }
        nextNode = newNextNode.neigbours[closestNodeIndex];
	}

	public void SubscribeResetRallyPoint()
	{
		playerRallyPoint.OnRallyPointChanged += ResetRallyPoint;
	}

	public void UnsubscribeResetRallyPoint()
	{
		playerRallyPoint.OnRallyPointChanged -= ResetRallyPoint;
	}

	public void ResetRallyPoint()
	{
        destinationPoint = (Vector2)playerRallyPoint.gameObject.transform.position;
        destinationNode = Grid.Instance.NodeFromWorldPoint(destinationPoint);
		if (node == destinationNode)
		{
			droneComponent.EnterIdleMode();
			return;
		}
		DefineNextNode(node);
        if (node == nextNode)
        {
            droneComponent.EnterIdleMode();
            return;
        }
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
	}

	public void StopMoving()
	{
		StopCoroutine("FollowPath");
	}

    void Rotate()
    {
        directionVector = (nextNode.worldPosition - (Vector2)trans.position).normalized;
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle-90, Vector3.forward);
        trans.rotation = Quaternion.Slerp(trans.rotation, qt, Time.deltaTime * turnSpeed);
    }

    void Move()
    {
		float step = Time.deltaTime * speed * speedPercent;
		trans.position = Vector2.MoveTowards(trans.position, nextNode.worldPosition, step);
    }

	public void OnDrawGizmos() {

	}

	public Unit GetUnit()
	{
		return this;
	}

	public Base GetBase()
	{
		return null;
	}

	public GameObject GetGameobject()
	{
		return gameObject;
	}
}
