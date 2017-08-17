using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Unit : MonoBehaviour, ICollidableUnit
{
    public bool drawGizmos = false;

    IEnumerator coroutineFollowPath;

    const float minPathUpdateTime = .5f;
    const float pathUpdateMoveThreshold = .5f;

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
    public float radiusSoft = 0.5f;
    public float radiusHard = 0.5f;

    [Header("Nodes")]
    public Node node;
    public Node[] nextNodes = new Node[8];
	public Node nextNode;
	public Node tmpNode;
    public Node destinationNode;

    public Drone droneComponent;
    public Radar radarComponent;
	public Weapon weaponComponent;


    public CollisionCircle collisionCircle;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        droneComponent = GetComponent<Drone>();
        radarComponent = GetComponent<Radar>();
		weaponComponent = GetComponent<Weapon>();

        radiusSoft = gameObject.GetComponent<Quad>().size;
        radiusHard = radiusSoft/2+radiusSoft/5;

        collisionCircle = new CollisionCircle(trans.position, gameObject.GetComponent<Quad>().size, this);
    }

    void OnEnable()
	{
        Activate();
        CollisionManager.Instance.AddUnit(collisionCircle);
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
        if (droneComponent.owner == 0)
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
        collisionCircle.point = trans.position;
    }

    void Deactivate()
    {
        if (droneComponent.owner == 0)
            playerRallyPoint.OnRallyPointChanged -= ResetRallyPoint;

        //QuadTree.Instance.qtree.Remove(gameObject);
        playerController.playerUnitCount -= 1;
        PlayerController.unitCount -= 1;
    }

	IEnumerator FollowPath()
    {
        droneComponent.mode = Drone.Mode.Moving;
        speedPercent = 1;

        while (true)
        {
            tmpNode = Grid.Instance.NodeFromWorldPoint(trans.position);
			if (tmpNode != node)
			{
				node = tmpNode;
				DefineNextNode(node);
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
                droneComponent.EnterIdleMode();
                yield break;
            }


            yield return null;
        }
    }

	void DefineNextNode(Node destNode)
	{
        //List<Node> closestToDestination = new List<Node>();
        //nextNodes = destNode.neigbours; //.Instance.GetNeighbours(destNode);

        bool closestFound = false;
        int closestNodeIndex = 0;
        for (int i = 0; i < destNode.neigbours.Length - 1; i++)
        //foreach (Node n in nextNodes)
        {
            if (destNode.neigbours[i] != null)
            {
                if (destNode.neigbours[i].distance[droneComponent.owner] < destNode.neigbours[closestNodeIndex].distance[droneComponent.owner])
                {
                    //Debug.Log("node " + nextNodes[i].gridX + "," + nextNodes[i].gridY + " < node " + nextNodes[closestNodeIndex].gridX + "," + nextNodes[closestNodeIndex].gridY);
                    closestNodeIndex = i;//nextNodes.IndexOf(n);//Array.IndexOf(nextNodes, n); 
                    closestFound = true;
                }
            }
            //if (n.distance[droneComponent.owner] >= node.distance[droneComponent.owner])
            //continue;
            //if (!n.suitable[droneComponent.owner])
            //continue;
            //nextNode = n;
            //closestToDestination.Add(n);
        }
        if (closestFound)
            nextNode = destNode.neigbours[closestNodeIndex];
	}

	void ResetRallyPoint()
	{
		destinationPoint = playerRallyPoint.gameObject.transform.position;
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

    void Rotate()
    {
        directionVector = nextNode.worldPosition - (Vector2)trans.position;
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        trans.rotation = Quaternion.Slerp(trans.rotation, qt, Time.deltaTime * turnSpeed);
    }

    void Move()
    {
		float step = Time.deltaTime * speed * speedPercent;
		trans.position = Vector2.MoveTowards(trans.position, nextNode.worldPosition, step);
    }

	public void OnDrawGizmos() {
        if (drawGizmos)
        {
            if (nextNode != null)
            {
                Color newColor = Color.green;
                newColor.a = 0.3f;
                Gizmos.color = newColor;
                Gizmos.DrawLine(trans.position, nextNode.worldPosition);

                Gizmos.color = newColor;
                Gizmos.DrawCube(nextNode.worldPosition, nextNode.rect.size * (nextNode.rect.size.x - 0.05f));
            }
        }
	}

    public Drone GetDrone()
    {
        return droneComponent;
    }

    public Drone.Mode GetMode()
    {
        return droneComponent.mode;
    }

	public Vector2 GetPoint()
    {
        return trans.position;
    }
	public float GetRadiusSoft()
    {
        return radiusSoft;
    }
	public float GetRadiusHard()
	{
		return radiusHard;
	}
	public GameObject GetGameobject()
    {
        return gameObject;
    }
}
