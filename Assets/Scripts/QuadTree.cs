using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuadTreeNode
{
	public int level = 0;

	public Rect rect;
	public float halfWidth, halfHeight;

	public List<ICollidable> objects;
    public int objectsCount = 0;

	int maxLifespan = 8;
	int curLife = -1;

	public QuadTreeNode parent = null;
	public QuadTreeNode[] childs = new QuadTreeNode[4];
	bool hasChilds = false;
	int activeNodes = 0;

	public bool built = false;

	public QuadTreeNode(Rect newRect)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = new List<ICollidable>();
		parent = null;
		childs = new QuadTreeNode[4];
        for (int i = 0; i < 4; i++)
        {
            childs[i] = null;
        }
	}

    public QuadTreeNode(Rect newRect, List<ICollidable> units)
    {
        rect = newRect;
        halfWidth = rect.width / 2;
        halfHeight = rect.height / 2;
        objects = units;
        parent = null;
        childs = new QuadTreeNode[4];
        for (int i = 0; i < 4; i++)
        {
            childs[i] = null;

        }
    }

	public void Update()
	{
		List<ICollidable> movedUnits = new List<ICollidable>(objects.Count);
        int movedUnitsCount = 0;

        //foreach (ICollidableUnit unit in objects)
        for (int i = 0; i < objects.Count; i++)
		{
            // Moving = 1
                movedUnits.Add(objects[i]);
                movedUnitsCount += 1;
		}

        bool tmpHasChilds = false;
        //Debug.Log("active nodes: " + activeNodes + " in level " + level);
        for (int i = 0; i < 4; i++)
        {
            if (childs[i] != null)
            {
                tmpHasChilds = true;
                childs[i].Update();
            }
        }
        if (!tmpHasChilds)
            hasChilds = false;

		// go up
		//foreach (ICollidableUnit unit in movedUnits)
        for (int i = 0; i < movedUnits.Count; i++)
		{
			QuadTreeNode currentNode = this;

			//while (CollisionManager.Instance.RectIntersectsWithCircle(currentNode.rect, currentNode.halfWidth, currentNode.halfHeight, unit.GetPoint(), unit.GetRadius()))
            while(!RectContainsCircle(rect, movedUnits[i]))
            {
				if (currentNode.parent != null) currentNode = currentNode.parent;
				else break;
			}

			objects.Remove(movedUnits[i]);
            //objectsCount -= 1;
			currentNode.Insert(movedUnits[i]);
		}

	}

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
	public void Split()
	{
		childs[1] = new QuadTreeNode(new Rect(rect.x + halfWidth, rect.y, halfWidth, halfHeight));
		childs[0] = new QuadTreeNode(new Rect(rect.x, rect.y, halfWidth, halfHeight));
		childs[2] = new QuadTreeNode(new Rect(rect.x, rect.y + halfHeight, halfWidth, halfHeight));
		childs[3] = new QuadTreeNode(new Rect(rect.x + halfWidth, rect.y + halfHeight, halfWidth, halfHeight));
        for (int i = 0; i < 4; i++)
        {
            childs[i].level = level + 1;
        }
	}

    private int getIndex(ICollidable unit)
	{
		for (int i = 0; i < 4; i++)
		{
            if (childs[i] != null)
                if (RectContainsCircle(childs[i].rect, unit))
                    return i;
		}

		return -1;
	}

	public List<ICollidable> Retrieve(List<ICollidable> returnObjects, ICollidable unit)
	{
		int index = getIndex(unit);
		if (index != -1 && childs[0] != null)
		{
			childs[index].Retrieve(returnObjects, unit);
		}
		//if (Objects.Count!=1)
		returnObjects.AddRange(objects);

		return returnObjects;
	}

	public void Insert(ICollidable unit)
	{
		if (childs[0] != null)
		{
			int index = getIndex(unit);

			if (index != -1)
			{
				childs[index].Insert(unit);

				return;
			}
		}

		objects.Add(unit);

		if (objects.Count > 2 && level < 4)
		{
			if (childs[0] == null)
			{
				Split();
			}

			int i = 0;
			while (i < objects.Count)
			{
				int index = getIndex(objects[i]);
				if (index != -1)
				{
					childs[index].Insert(objects[i]);
					objects.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
		}
	}

    public bool RectContainsCircle(Rect rect, ICollidable circle)
	{
        if (!rect.Contains(circle.Point))
            return false;
        if (circle.Point.x - circle.Radius < rect.x)
            return false;
        if (circle.Point.x + circle.Radius > rect.width)
            return false;
        if (circle.Point.y - circle.Radius < rect.y)
            return false;
        if (circle.Point.y + circle.Radius > rect.height)
            return false;
        return true;
	}

	public void DrawDebug()
	{
#if UNITY_EDITOR
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height));

		//foreach (CollisionCircle unit in objects)
		//{
  //          Handles.Label(unit.unit.trans.position, level.ToString());
		//}
        Handles.Label(rect.center, objects.Count.ToString());

        for (int i = 0; i < childs.Length; i++)
		{
            if (childs[i] != null)
            {
                childs[i].DrawDebug();
                //Handles.Label(childs[i].rect.center, i.ToString());
            }
		}
#endif
	}

}
