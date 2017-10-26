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

	[SerializeField] private bool showGizmos;

	public QuadTreeEternal Qtree;

	public Queue<CollisionCircle> ObjectsToInsert = new Queue<CollisionCircle>();

	private void Start()
	{
        Rect rect = new Rect();
		rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
		rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

		Qtree = new QuadTreeEternal(rect);
		Qtree.Root = Qtree;

		GameManager.Instance.OnGameRestart += ClearQuadTree;
	}

    public void AddCollidable(CollisionCircle obj)
	{
//        qtree.Insert(obj);
		ObjectsToInsert.Enqueue(obj);
	}

    // update for QuadTreeEternal
    private void Update()
    {
	    if (Qtree != null)
	    {
		    if (ObjectsToInsert.Count > 0)
		    {
			    MoveFromQueue();
		    }
		    Qtree.Update();
	    }
    }

	private void MoveFromQueue()
	{
		while (ObjectsToInsert.Count > 0)
		{
			Qtree.Insert(ObjectsToInsert.Dequeue());
		}
	}

	private void ClearQuadTree()
	{
		if (Qtree != null)
		{
			Qtree.Clear();
			foreach (var b in GameManager.Instance.Planets)
			{
				AddCollidable(b.Collision);
			}
		}
	}
    
	public List<CollisionCircle> FindBodiesInCircleArea(Vector2 center, float radius)
	{
		List<CollisionCircle> units = Qtree.GetWholeTreeObjects(new List<CollisionCircle>());
		//Debug.Log("units: " + units.Count);
		List<CollisionCircle> catched = new List<CollisionCircle>();
		foreach (var unit in units)
		{
			if (unit.IsWeapon)
			{
				continue;
			}
			if (unit.IsStatic)
			{
				continue;
			}
			float dx = unit.Trans.position.x - center.x;
			float dy = unit.Trans.position.y - center.y;
			if (dx * dx + dy * dy < radius * radius)
			{
				catched.Add(unit);
			}
		}
		//Debug.Log("catched units: " + catched.Count);
		return catched;
	}

	//Debug view for QuadTreeNode
	void OnDrawGizmos()
	{
        if (showGizmos)
        {
            if (Qtree != null)
            {
	            Qtree.DrawDebug();
            }
        }
	}
}