using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public QuadTreeEternal qtree;

	public Queue<CollisionCircle> objectsToInsert = new Queue<CollisionCircle>();

	private void Start()
	{
        Rect rect = new Rect();
		rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
		rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

		qtree = new QuadTreeEternal(rect);
		qtree.root = qtree;

		GameController.Instance.OnGameRestart += ClearQuadTree;
	}

    public void AddCollidable(CollisionCircle obj)
	{
//        qtree.Insert(obj);
		objectsToInsert.Enqueue(obj);
	}

    // update for QuadTreeEternal
    private void Update()
    {
	    if (qtree != null && objectsToInsert.Count > 0)
	    {
		    MoveFromQueue();
	    }
        qtree.Update();
    }

	private void MoveFromQueue()
	{
		while (objectsToInsert.Count > 0)
		{
			qtree.Insert(objectsToInsert.Dequeue());
		}
	}

	public void ClearQuadTree()
	{
		if (qtree != null)
		{
			qtree.Clear();
			foreach (var b in GameController.Instance.bases)
			{
				AddCollidable(b.collision);
			}
		}
	}
    
	public List<CollisionCircle> FindBodiesInCircleArea(Vector2 center, float radius)
	{
		List<CollisionCircle> units = qtree.GetWholeTreeObjects(new List<CollisionCircle>());
		//Debug.Log("units: " + units.Count);
		List<CollisionCircle> catched = new List<CollisionCircle>();
		foreach (var unit in units)
		{
//			if (unit.collisionType != CollisionCircle.CollisionType.Body)
//				continue;
			float dx = unit.trans.position.x - center.x;
			float dy = unit.trans.position.y - center.y;
			if (dx * dx + dy * dy < radius * radius)
				catched.Add(unit);
		}
		//Debug.Log("catched units: " + catched.Count);
		return catched;
	}

	//Debug view for QuadTreeNode
	void OnDrawGizmos()
	{
        if (showGizmos)
        {
            if (qtree != null)
                qtree.DrawDebug();
        }
	}
}