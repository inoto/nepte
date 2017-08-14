using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{

    const float minPathUpdateTime = .5f;
    const float pathUpdateMoveThreshold = .5f;

	public Transform destinationTransform;
	public RallyPoint playerRallyPoint;
	public Vector2 directionVector;
	public Vector3 destinationPoint;

    public float speed = 2f;
    public float speedPercent = 1;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    List<Vector2> path;

    Vector2 pathCurrent;
    int pathIndex;
    public bool hasCollided = false;
    public bool hasNode = false;

    public Node node;
    public List<Node> nextNodes;
	public Node nextNode;
	public Node tmpNode;
	public Node destinationNode;

    private Drone droneComponent;

    void Start()
    {
		
  //      droneComponent = GetComponent<Drone>();

		//ResetRallyPoint();

		//node = Grid.Instance.NodeFromWorldPoint(transform.position);
		//StartCoroutine(FollowPath());
    }

	void OnEnable()
	{
		droneComponent = GetComponent<Drone>();
		playerRallyPoint = GameController.Instance.playerControllerObject[droneComponent.owner].GetComponent<PlayerController>().rallyPoint;
		playerRallyPoint.OnRallyPointChanged += ResetRallyPoint;

		node = Grid.Instance.NodeFromWorldPoint(transform.position);
		ResetRallyPoint();

		StartCoroutine(FollowPath());
	}

	private void OnDisable()
	{
		playerRallyPoint.OnRallyPointChanged -= ResetRallyPoint;
	}

	IEnumerator FollowPath()
    {
        droneComponent.mode = Drone.Mode.Moving;
        speedPercent = 1;

        while (true)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
			tmpNode = Grid.Instance.NodeFromWorldPoint(transform.position);
			//if (nextNode == node)
			//	continue;
			//else
			//	node = nextNode;
			if (tmpNode != node)
			{
				node = tmpNode;
				DefineNextNode(node);
			}
            //    else
            //    {
            //        droneComponent.EnterIdleMode();
            //        yield break;
            //    }

            Rotate();

            Move();

            yield return null;
        }
    }

	void DefineNextNode(Node destinationNode)
	{
		nextNodes = Grid.Instance.GetNeighbours(destinationNode);
		if (nextNodes.Count > 0)
		{
			foreach (Node n in nextNodes)
			{
				if (n.distance[droneComponent.owner] >= node.distance[droneComponent.owner])
					continue;
				if (!n.suitable[droneComponent.owner])
					continue;
				nextNode = n;
			}
		}
	}

	void FindSuitableNodeAround()
	{

	}

	void ResetRallyPoint()
	{
		destinationPoint = playerRallyPoint.gameObject.transform.position;
		if (droneComponent.owner == 0)
			Debug.Log("rally point changed");
		DefineNextNode(Grid.Instance.NodeFromWorldPoint(destinationPoint));
	}

    void Rotate()
    {
        directionVector = nextNode.worldPosition - transform.position;
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * turnSpeed);
    }

    void Move()
    {
		float step = Time.deltaTime * speed * speedPercent;
		transform.position = Vector2.MoveTowards(transform.position, nextNode.worldPosition, step);
    }

	public void OnDrawGizmos() {
        if (path != null)
        {
            Color newColor = Color.green;
            newColor.a = 0.2f;
            Gizmos.color = newColor;
            for (int i = pathIndex; i < path.Count; i++)
            {
                if (i == pathIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);

                }
            }
            Gizmos.color = newColor;
            Gizmos.DrawCube(path[path.Count -1], Vector3.one * (Grid.Instance.nodeDiameter - 0.01f));
		}
	}
}
