using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	const float minPathUpdateTime = .2f;
	const float pathUpdateMoveThreshold = .5f;

    Vector2 directionVector;

	public float speed = 20;
    public float speedPercent = 1;
	public float turnSpeed = 3;
	public float turnDst = 5;
	public float stoppingDst = 10;

	Path path;

    private Drone droneComponent;

	void Start() {
        droneComponent = GetComponent<Drone>();

		StartCoroutine (UpdatePath ());
	}

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(waypoints, transform.position, turnDst, stoppingDst);

			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator UpdatePath() {

		if (Time.timeSinceLevelLoad < .3f) {
			yield return new WaitForSeconds (.3f);
		}
        PathRequestManager.RequestPath (new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = droneComponent.destinationTransform.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			//print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
			if ((droneComponent.destinationTransform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest(transform.position, droneComponent.destinationTransform.position, OnPathFound));
				targetPosOld = droneComponent.destinationTransform.position;
			}
		}
	}

	IEnumerator FollowPath() {

		bool followingPath = true;
		int pathIndex = 0;
        Rotate();

		speedPercent = 1;

		while (followingPath) {
			Vector2 pos2D = new Vector2 (transform.position.x, transform.position.y);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {
                directionVector = droneComponent.destinationTransform.position - transform.position;
				if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
					//speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDst);
					if (speedPercent < 0.01f) {
						followingPath = false;
					}
				}

                Rotate();

                float step = Time.deltaTime * speed * speedPercent;
                transform.position = Vector2.MoveTowards(transform.position, droneComponent.destinationTransform.position, step);
			}

			yield return null;

		}
	}

    void Rotate()
    {
		float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
		Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.Lerp(transform.rotation, qt, Time.deltaTime * turnSpeed);
    }

	public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos ();
		}
	}
}
