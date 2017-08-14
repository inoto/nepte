using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    IEnumerator coroutineFollowPath;

    const float minPathUpdateTime = .5f;
    const float pathUpdateMoveThreshold = .5f;

    private PlayerController playerController;

    [Header("Destination")]
	public RallyPoint playerRallyPoint;
	public Vector2 directionVector;
	public Vector3 destinationPoint;

    [Header("Move")]
    public float speed = 2;
    public float speedPercent = 1;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    [Header("Nodes")]
    public Node node;
    public List<Node> nextNodes;
	public Node nextNode;
	public Node tmpNode;
    public Node destinationNode;

    private Drone droneComponent;

    void Start()
    {
        Activate();
    }

	void OnEnable()
	{
        Activate();
	}

    void Activate()
    {
        coroutineFollowPath = FollowPath();

        // player specific assigns
        playerController = transform.parent.gameObject.GetComponent<PlayerController>();
		droneComponent = GetComponent<Drone>();
		playerRallyPoint = GameController.Instance.playerControllerObject[droneComponent.owner].GetComponent<PlayerController>().rallyPoint;
		playerRallyPoint.OnRallyPointChanged += ResetRallyPoint;

        node = Grid.Instance.NodeFromWorldPoint(droneComponent.trans.position);
		ResetRallyPoint();

        QuadTree.Instance.qtree.Insert(gameObject);
        playerController.unitCount += 1;
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

        QuadTree.Instance.qtree.Remove(gameObject);
        playerController.unitCount -= 1;
    }

	IEnumerator FollowPath()
    {
        droneComponent.mode = Drone.Mode.Moving;
        speedPercent = 1;

        while (true)
        {
			tmpNode = Grid.Instance.NodeFromWorldPoint(droneComponent.trans.position);
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

		nextNodes = Grid.Instance.GetNeighbours(destNode);
		if (nextNodes.Count > 0)
		{
            bool closestFound = false;
            int closestNodeIndex = 0;
			foreach (Node n in nextNodes)
			{
                if (n.distance[droneComponent.owner] < nextNodes[closestNodeIndex].distance[droneComponent.owner])
                {
                    closestNodeIndex = nextNodes.IndexOf(n);
                    closestFound = true;
                }
				//if (n.distance[droneComponent.owner] >= node.distance[droneComponent.owner])
					//continue;
				//if (!n.suitable[droneComponent.owner])
					//continue;
				//nextNode = n;
                //closestToDestination.Add(n);
			}
            if (closestFound)
                nextNode = nextNodes[closestNodeIndex];
		}
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
		DefineNextNode(destinationNode);
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
        directionVector = nextNode.worldPosition - transform.position;
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, qt, Time.deltaTime * turnSpeed);
    }

    void Move()
    {
		float step = Time.deltaTime * speed * speedPercent;
		transform.position = Vector2.MoveTowards(transform.position, nextNode.worldPosition, step);
    }

	public void OnDrawGizmos() {
        if (nextNode != null)
        {
            Color newColor = Color.green;
            newColor.a = 0.3f;
			Gizmos.color = newColor;
			Gizmos.DrawLine(transform.position, nextNode.worldPosition);

			Gizmos.color = newColor;
            Gizmos.DrawCube(nextNode.worldPosition, nextNode.rect.size * (nextNode.rect.size.x - 0.05f));
		}
	}
}
