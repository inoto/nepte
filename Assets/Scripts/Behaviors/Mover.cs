using UnityEngine;
using System.Collections;

[System.Serializable]
public class Mover : MonoBehaviour
{
    public enum DestinationType
    {
        None,
        Rally,
        Drone,
        Base
    }

    [Header("Gizmos")]
    public bool showCurrentNode;
    public bool showPath;

    [Header("Main")]
    public Vector2 velocity;
    public Vector2 acceleration;

    public DestinationType destinationType;
    public bool isActive = false;
    public Vector2 destinationPoint;
    public float speed;
    [Range(0.0f, 1.0f)]
    public float speedPercent = 1;
    public Node node;

    public Path path;

    [Header("Cache")]
    Node updateNode;

    [Header("Components")]
    Drone drone;

    private void Awake()
    {
        drone = GetComponent<Drone>();
    }

    private void Start()
    {
        

    }

    public void ActivateWithOwner()
    {
        node = Grid.Instance.NodeFromWorldPoint(drone.trans.position);
        path = null;
        //drone.owner.playerController.rallyPoint.OnRallyPointChanged += StartMoving;
        //if (node != drone.owner.playerController.rallyPoint.node)
        //    StartMoving(drone.owner.playerController.rallyPoint.trans.position);
        ApplyForce(drone.owner.playerController.rallyPoint.trans.position);
    }

    public void ApplyForce(Vector2 force)
    {
        acceleration += force;
    }

    public void Update()
    {
        velocity += acceleration;
        drone.trans.position += (Vector3)velocity;
        acceleration *= 0;
    }

    void StartMoving(Vector2 _point)
    {
        // stop coroutine if working
        if (isActive)
        {
            isActive = false;
            path = null;
            StopCoroutine("MoveByNodes");
        }

        // create path
        //path = new Path();
        //bool success = false;
        // fill path as possible
        //success = path.CreateNew(node, drone.owner.playerController.rallyPoint.node, drone.owner.playerNumber);
        //if (success)
            StartCoroutine("MoveByNodes");
        //else
            //Debug.Log("Create new path failed");
    }

    IEnumerator MoveByNodes()
    {
        isActive = true;
        //Debug.Log("MoveByNodes started");
        while (isActive)
        {
            // node detection
            updateNode = Grid.Instance.NodeFromWorldPoint(drone.trans.position);
            if (node != updateNode)
            {
                if (updateNode.walkable)
                {
                    if (node.prisoner == gameObject)
                        node.ReleaseObject();
                    updateNode.ImprisonObject(gameObject);
                    node = updateNode;
                }
                else
                {
                    // do repath
                }
                // need to update path with new node step
                bool success = false;
                success = path.UpdateStep(drone.owner.playerNumber);
                if (!success)
                    Debug.Log("update step failed");
            }

            // movement if path contains at least one node
            if (path != null && path.waypoints[0] != null)
            {
                MoveTowardsWaypoint0();
            }
            else
            {
                path = null;
                isActive = false;
                Debug.Log("MoveByNodes ended");
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void MoveTowardsWaypoint0()
    {
		float step = Time.deltaTime * speed * speedPercent;
		drone.trans.position = Vector2.MoveTowards(drone.trans.position, path.waypoints[0].worldPosition, step);
    }

    private void OnDrawGizmos()
    {
        Color newColor = Color.cyan;

		if (showCurrentNode)
        { 
            if (node != null)
            {
                newColor.a = 0.5f;
                Gizmos.color = newColor;
                Gizmos.DrawCube(node.worldPosition, Vector2.one * (1 - 0.05f));
            }
        }
		if (showPath)
		{
			if (path != null)
			{
				if (path.waypoints[0] != null)
				{
					Gizmos.DrawLine(node.worldPosition, path.waypoints[0].worldPosition);
				}
                path.DrawPath();
			}
		}
    }
}
