using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	const float minPathUpdateTime = .5f;
	const float pathUpdateMoveThreshold = .5f;

    Vector2 directionVector;

    public float speed = 0.2f;
    public float speedPercent = 1;
	public float turnSpeed = 3;
	public float turnDst = 5;
	public float stoppingDst = 10;

	Vector2[] path;
    Vector2 pathCurrent;
    int pathIndex;
    public bool hasCollided = false;
    public bool hasNode = false;

    public Node node;

    private Drone droneComponent;

	void Start()
    {
        droneComponent = GetComponent<Drone>();

        StartCoroutine(UpdatePath());
	}

    public void OnPathFound(Vector2[] waypoints, bool pathSuccessful)
    {
		if (pathSuccessful) {
            path = waypoints;
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

    public void Collide(Node nodeCollide)
    {
		Node newNode = Grid.Instance.ClosestToDestinationWalkableNode(nodeCollide, droneComponent.destinationTransform.position);
		Vector2[] waypoint = new Vector2[] { newNode.worldPosition };
		OnPathFound(waypoint, true);
    }

    IEnumerator UpdatePath()
    {
        PathRequestManager.RequestPath(new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));

        Vector2 targetPosOld = droneComponent.destinationTransform.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            Vector2 destination = new Vector2(droneComponent.destinationTransform.position.x, droneComponent.destinationTransform.position.y);
            if (destination != targetPosOld || hasCollided)
            {
				PathRequestManager.RequestPath (new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));
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

            if ((pathIndex == (path.Length - 1)) && stoppingDst > 0)
            {
                speedPercent = .75f;
            }

			if (pathCurrent == pos2D)
			{
				pathIndex++;
				if (pathIndex >= path.Length)
				{
					droneComponent.EnterIdleMode();
					yield break;
				}
				pathCurrent = path[pathIndex];
			}

            Rotate();

            Move();

            yield return null;
        }
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

	//public void OnDrawGizmos() {
 //       if (path != null)
 //       {
 //           Color newColor = Color.green;
 //           newColor.a = 0.5f;
 //           Gizmos.color = newColor;
 //           for (int i = pathIndex; i < path.Length; i++)
 //           {
 //               if (i == pathIndex)
 //               {
 //                   Gizmos.DrawLine(transform.position, path[i]);
 //               }
 //               else
 //               {
 //                   Gizmos.DrawLine(path[i - 1], path[i]);

 //               }
 //           }
 //           Gizmos.color = newColor;
 //           Gizmos.DrawCube(path[path.Length-1], Vector3.one * (Grid.nodeDiameter - 0.01f));
	//	}
	//}
}
