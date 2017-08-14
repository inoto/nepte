using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
	private static QuadTree _instance;

	public static QuadTree Instance { get { return _instance; } }

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
    public int levels = 0;

    Rect rect = new Rect();

    public QuadTreeNode qtree;

	private void Start()
	{
        rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
        rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

        qtree = new QuadTreeNode(rect, 1);
	}

    private void Update()
    {
        levels = QuadTreeNode.levels;
        foreach (GameObject unit in qtree.FindObjectsInRect(rect))
        {
            
        }
    }

    //Debug view for QuadTreeNode
    void OnDrawGizmos()
	{
		if (showGizmos)
            if (qtree != null)
                qtree.DrawDebug();
	}
}

public class QuadTreeNode
{
    public static int levels = 0;
    const int MIN_SIZE = 1;

    public Rect rect;
    int maxObjects;

    List<GameObject> objects;

    public QuadTreeNode[] childs;

    public QuadTreeNode(Rect newRect, int newMaxObjects)
    {
        rect = newRect;
        maxObjects = newMaxObjects;
        objects = new List<GameObject>(maxObjects);
        childs = new QuadTreeNode[4];
    }

    public void Insert(GameObject obj)
    {
        if (childs[0] != null)
        {
            int childIndex = GetChildToInsertObject(obj.transform.position);

            if (childIndex > -1)
                childs[childIndex].Insert(obj);

            return;
        }

        objects.Add(obj);

        if (objects.Count > maxObjects)
        {
            levels += 1;
            if (childs[0] == null)
            {
                float subWidth = rect.width / 2;
                float subHeight = rect.height / 2;
                float x = rect.x;
                float y = rect.y;

                childs[0] = new QuadTreeNode(new Rect(x + subWidth, y, subWidth, subHeight), maxObjects);
                childs[1] = new QuadTreeNode(new Rect(x, y, subWidth, subHeight), maxObjects);
                childs[2] = new QuadTreeNode(new Rect(x, y + subHeight, subWidth, subHeight), maxObjects);
                childs[3] = new QuadTreeNode(new Rect(x + subWidth, y + subHeight, subWidth, subHeight), maxObjects);
            }
            //Reallocate this quads objects into its children
            int i = objects.Count - 1;
            while (i >= 0)
            {
                GameObject storedObj = objects[i];
                int childIndex = GetChildToInsertObject(storedObj.transform.position);

                if (childIndex > -1)
                    childs[childIndex].Insert(storedObj);

                objects.RemoveAt(i);
                i -= 1;
            }
        }
    }

	//Removes the cell from the QuadTreeNode
	public void Remove(GameObject obj)
    {
        if (ContainsLocation(obj.transform.position))
        {
            objects.Remove(obj);

            if (childs[0] != null)
            {
                for (int i = 0; i < 4; i++)
                    childs[i].Remove(obj);
            }
        }
    }

    void CheckCollisions()
    {
        
    }

	//Finds all objects in cell
    public List<GameObject> FindObjectsInRect(Rect inRect)
    {
        if (RectOverLap(rect, inRect))
        {
            List<GameObject> returnedObjects = new List<GameObject>();

            for (int i = 0; i < objects.Count; i++)
            {
                if (inRect.Contains(objects[i].transform.position))
                    returnedObjects.Add(objects[i]);
            }

            if (childs[0] != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    List<GameObject> childObjects = childs[i].FindObjectsInRect(inRect);

                    if (childObjects != null)
                        returnedObjects.AddRange(childObjects);
                }
            }

            return returnedObjects;
        }

        return null;
    }

	//Clear QuadTreeNode
	public void Clear()
	{
		objects.Clear();

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs[i] != null)
			{
				childs[i].Clear();
				childs[i] = null;
			}
		}
	}

	bool ValueInRange(float value, float min, float max)
	{
		return (value >= min) && (value <= max);
	}

	//Checks to see if two childs overlap at any point
	bool RectOverLap(Rect A, Rect B)
	{
		//Checks to see if either cell has a X coord in common
		bool xOverLap = ValueInRange(A.x, B.x, B.x + B.width) || ValueInRange(B.x, A.x, A.x + A.width);

		//Checks to see if either cell has a Y coord in common
		bool yOverLap = ValueInRange(A.y, B.y, B.y = B.height) || ValueInRange(B.y, A.y, A.y + A.height);

		//If the childs have both a X & Y coord in common they overlap
		return xOverLap && yOverLap;
	}

	//Checks to see if overlap between new cell and already existing cell
	public bool ContainsLocation(Vector2 point)
	{
		return (rect.Contains(point));
	}

	//Takes location and determines which cell to insert it into
	int GetChildToInsertObject(Vector2 point)
	{
		for (int i = 0; i < 4; i++)
		{
			if (childs[i].ContainsLocation(point))
				return i;
		}

		return -1;
	}

    public void DrawDebug()
    {
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height));

		if (childs[0] != null)
		{
			for (int i = 0; i < childs.Length; i++)
			{
				if (childs[i] != null)
                    childs[i].DrawDebug();
			}
		}
    }

}
