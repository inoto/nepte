using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidableUnit
{
	Drone.Mode GetMode();
	Vector2 GetPoint();
	float GetRadius();
	GameObject GetGameobject();
}

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager _instance;

    public static CollisionManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

	public bool showGizmos;
	public int objectsInTree = 0;

	public QuadTreeNode qtree;

	private void Start()
	{
        Rect rect = new Rect();
		rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
		rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

		qtree = new QuadTreeNode(rect);

	}

	private void Update()
	{
		if (qtree != null)
		{
			qtree.Update();
		}

	}

	public bool RectIntersectsWithCircle(Rect rect, float halfW, float halfH, Vector2 point, float radius)
	{
		// Find the closest point to the circle within the rectangle
        float closestX = Mathf.Clamp(point.x, rect.x, rect.width);
        float closestY = Mathf.Clamp(point.y, rect.height, rect.y);

		// Calculate the distance between the circle's center and this closest point
		float distanceX = point.x - closestX;
		float distanceY = point.y - closestY;

		// If the distance is less than the circle's radius, an intersection occurs
		float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
		return distanceSquared < (radius * radius);
	}

	//Debug view for QuadTreeNode
	void OnDrawGizmos()
	{
		if (showGizmos)
			if (qtree != null)
				qtree.DrawDebug();
	}
}