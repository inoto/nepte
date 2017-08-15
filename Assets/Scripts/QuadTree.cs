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

	public static Queue<ICollidableUnit> pendingInsertion = new Queue<ICollidableUnit>();
	public List<ICollidableUnit> objects;

	int maxLifespan = 8;
	int curLife = -1;

	public QuadTreeNode parent = null;
	public QuadTreeNode[] childs = new QuadTreeNode[4];
	bool hasChilds = false;
	byte activeNodes = 0;

	public bool built = false;

	public QuadTreeNode(Rect newRect)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = new List<ICollidableUnit>();
		parent = null;
		childs = new QuadTreeNode[4];
        for (int i = 0; i < 4; i++)
        {
            childs[i] = null;
        }
        //BuildTree();
	}

	public QuadTreeNode(Rect newRect, List<ICollidableUnit> units)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = units;
		//childs = new QuadTreeNode[4];
	}

	public void Update()
	{

		if (objects.Count == 0)
		{
		    //if (parent == null)
		        //return;
		    if (!hasChilds)
		    {
		        if (curLife == -1)
		            curLife = maxLifespan;
		        else if (curLife > 0)
		            curLife -= 1;
		    }
		}
		else
		{
		    if (curLife != -1)
		    {
		        if (maxLifespan <= 64)
		            maxLifespan *= 2;
		        curLife = -1;
		    }
		}

		List<ICollidableUnit> movedUnits = new List<ICollidableUnit>(objects.Count);

		foreach (ICollidableUnit unit in objects)
		{
			// Moving = 1
			//if (unit.GetMode() == Drone.Mode.Moving)
			    movedUnits.Add(unit);
		}

		//int objectsCount = objects.Count;
		//for (int i = 0; i < objectsCount; i++)
		//{
		//	// TODO: add check is unit dead or not to optimize adding/removing to/from list
		//	if (movedUnits.Contains(objects[i]))
		//		movedUnits.Remove(objects[i]);
		//	objects.RemoveAt(i--);
		//	objectsCount -= 1;
		//}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) childs[index].Update();
        //for (int i = 0; i < 4; i++)
        //{
        //    if (childs[i] != null)
        //        childs[i].Update();
        //}


        // go up
		foreach (ICollidableUnit unit in movedUnits)
		{
			QuadTreeNode currentNode = this;

			//while (CollisionManager.Instance.RectIntersectsWithCircle(currentNode.rect, currentNode.halfWidth, currentNode.halfHeight, unit.GetPoint(), unit.GetRadius()))
            while(!rect.Contains(unit.GetPoint()))
            {
				if (currentNode.parent != null) currentNode = currentNode.parent;
				else break;
			}

			objects.Remove(unit);
			currentNode.Insert(unit);
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1 && childs[index].curLife == 0)
			{
                childs[index] = null;
				activeNodes ^= (byte)(1 << index);       //удаляем узел из списка флагов активных узлов
			}

		// check collisions here
		//if (parent == null)
		//{

		//}
	}

	public void Insert(ICollidableUnit obj)
	{
        if (objects.Count < 2 && activeNodes == 0)
        {
			objects.Add(obj);
			return;
		}

		if (rect.size.x < Vector2.one.x && rect.size.y < Vector2.one.y)
		{
		    objects.Add(obj);
		    return;
		}

		Rect[] quadrant = new Rect[4];
		quadrant[0] = (childs[0] != null) ? childs[0].rect : new Rect(rect.x + halfWidth, rect.y, halfWidth, halfHeight);
		quadrant[1] = (childs[1] != null) ? childs[1].rect : new Rect(rect.x, rect.y, halfWidth, halfHeight);
		quadrant[2] = (childs[2] != null) ? childs[2].rect : new Rect(rect.x, rect.y + halfHeight, halfWidth, halfHeight);
		quadrant[3] = (childs[3] != null) ? childs[3].rect : new Rect(rect.x + halfWidth, rect.y + halfHeight, halfWidth, halfHeight);
		float halfWidthQuadrant = quadrant[0].width / 2;
		float halfHeightQuadrant = quadrant[0].height / 2;

        for (int i = 0; i < 4; i++)
        {
            //if (CollisionManager.Instance.RectIntersectsWithCircle(quadrant[i], halfWidthQuadrant, halfHeightQuadrant, obj.GetPoint(), obj.GetRadius()))
            if (quadrant[i].Contains(obj.GetPoint()))
            {
                // using existing child
                if (childs[i] != null)
                {
                    childs[i].Insert(obj);
                }
                // create new child
                else
                {
                    childs[i] = new QuadTreeNode(quadrant[i]);
                    childs[i].parent = this;
                    childs[i].level = level + 1;
                    childs[i].Insert(obj);
                    hasChilds = true;
                    activeNodes |= (byte)(1 << i);
                }
                //Reallocate units here into its child
				for (int k = 0; k < objects.Count-1; k++)
				{
                    childs[i].Insert(objects[k]);
					objects.RemoveAt(k);
				}
                return;
            }
        }
		objects.Add(obj);
	}

	public void DrawDebug()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height));
		foreach (ICollidableUnit unit in objects)
		{
			Handles.Label(unit.GetGameobject().transform.position, level.ToString());
		}

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs[i] != null)
				childs[i].DrawDebug();
		}
	}

}
