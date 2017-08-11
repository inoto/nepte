using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{

    const float minPathUpdateTime = .5f;
    const float pathUpdateMoveThreshold = .5f;

    Vector2 directionVector;

    public float speed = 0.2f;
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
    public Node newNode;
    public Node destinationNode;

    private Drone droneComponent;

    void Start()
    {
        droneComponent = GetComponent<Drone>();

        StartCoroutine(UpdateNextNode());
    }

    public void NextNodeFound(Vector2[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new List<Vector2>(waypoints);
            pathIndex = 0;
            pathCurrent = path[pathIndex];
            hasCollided = false;
            droneComponent.ResetRallyPoint();
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        //else
        //{
        //    droneComponent.mode = Drone.Mode.Attacking;
        //}
    }

    /*public void Collide(Node nodeCollide)
    {
		Node newNode = Grid.Instance.ClosestToDestinationWalkableNode(nodeCollide, droneComponent.destinationTransform.position);
		Vector2[] waypoint = new Vector2[] { newNode.worldPosition };
		OnPathFound(waypoint, true);
    }*/

    IEnumerator UpdateNextNode()
    {
        //PathRequestManager.RequestPath(new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));

        Vector2 targetPosOld = droneComponent.destinationTransform.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            Vector2 destination = new Vector2(droneComponent.destinationTransform.position.x, droneComponent.destinationTransform.position.y);
            if (destination != targetPosOld || hasCollided)
            {
                //PathRequestManager.RequestPath(new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));
                targetPosOld = droneComponent.destinationTransform.position;
            }

        }
    }

    IEnumerator FollowPath()
    {
        droneComponent.mode = Drone.Mode.Moving;
        speedPercent = 1;

        while (true)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
            newNode = Grid.Instance.NodeFromWorldPoint(transform.position);
            if (newNode != null)
            {
                if (newNode != node)
                {
                    //              if (!newNode.walkable)
                    //              {
                    //                  if (newNode.prisoner.GetComponent<Drone>().mode != Drone.Mode.Moving)
                    //{
                    //	hasCollided = true;
                    //}
                    //if (droneComponent.mode == Drone.Mode.Idle)
                    //{
                    //	hasCollided = true;
                    //}
                    //}
                    if (newNode.walkable)
                    {
                        if (hasNode)
                        {
                            node.ReleaseObject();
                        }
                        newNode.ImprisonObject(gameObject);
                    }
                    //else
                    //{
                    //    bool result = GetWalkableInNeigbours();
                    //}
                }
                newNode = null;
            }

            if ((pathIndex == (path.Count - 1)) && stoppingDst > 0)
            {
                speedPercent = .75f;
            }

            if (pathCurrent == pos2D)
            {
                if (pathIndex <= path.Count)
                {
                    if (pathIndex < path.Count)
                    {
                        // find next node from path
                        Node nextNode = Grid.Instance.NodeFromWorldPoint(path[pathIndex + 1]);
                        // if not walkable
                        if (!nextNode.walkable)
                        {
                            // if all neigbours not walkable
                            if (!GetWalkableInNeigbours())
                            {
                                droneComponent.EnterIdleMode();
                                yield break;
                            }
                        }
                    }
                    pathIndex++;
                }
                else
                {
                    droneComponent.EnterIdleMode();
                    yield break;
                }

                //if (pathIndex >= path.Length)
                //{

                //}
                pathCurrent = path[pathIndex];
            }

            Rotate();

            Move();

            yield return null;
        }
    }

    bool GetWalkableInNeigbours()
    {
        List<Node> neig = Grid.Instance.GetNeighbours(node);
		foreach (Node n in neig)
		{
			// check each for walkable node
			if (n.walkable)
			{
				// insert it into path
                path[pathIndex] = node.worldPosition;
                return true;
			}
		}
        return false;
    }

    void Rotate()
    {
        directionVector = pathCurrent - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * turnSpeed);
    }

    void Move()
    {
        speed = 0.2f;
		float step = Time.deltaTime * speed * speedPercent;
		transform.position = Vector2.MoveTowards(transform.position, pathCurrent, step);
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
