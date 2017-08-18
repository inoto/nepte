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

	public List<CollisionCircle> objects;
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
		objects = new List<CollisionCircle>();
		parent = null;
		childs = new QuadTreeNode[4];
        for (int i = 0; i < 4; i++)
        {
            childs[i] = null;
        }
	}

    public QuadTreeNode(Rect newRect, List<CollisionCircle> units)
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

  //      if (objectsCount == 0)
		//{
		//    //if (parent == null)
		//        //return;
  //          if (activeNodes == 0)
		//    {
		//        if (curLife == -1)
		//            curLife = maxLifespan;
		//        else if (curLife > 0)
		//            curLife -= 1;
		//    }
		//}
		//else
		//{
		//    if (curLife != -1)
		//    {
		//        if (maxLifespan <= 64)
		//            maxLifespan *= 2;
		//        curLife = -1;
		//    }
		//}

		List<CollisionCircle> movedUnits = new List<CollisionCircle>(objects.Count);
        int movedUnitsCount = 0;

        //foreach (ICollidableUnit unit in objects)
        for (int i = 0; i < objects.Count - 1; i++)
		{
            // Moving = 1
            if (objects[i].unit.droneComponent.mode == Drone.Mode.Moving)
            {
                movedUnits.Add(objects[i]);
                movedUnitsCount += 1;
            }
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

        //for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
        //if ((flags & 1) == 1) childs[index].Update();
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
        for (int i = 0; i < movedUnits.Count - 1; i++)
		{
			QuadTreeNode currentNode = this;

			//while (CollisionManager.Instance.RectIntersectsWithCircle(currentNode.rect, currentNode.halfWidth, currentNode.halfHeight, unit.GetPoint(), unit.GetRadius()))
            while(!rect.Contains(movedUnits[i].point))
            {
				if (currentNode.parent != null) currentNode = currentNode.parent;
				else break;
			}

			objects.Remove(movedUnits[i]);
            //objectsCount -= 1;
			currentNode.Insert(movedUnits[i]);
		}

		//for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			//if ((flags & 1) == 1 && childs[index].curLife == 0)
			//{
			//	childs[index] = null;
			//	activeNodes ^= (byte)(1 << index);       //удаляем узел из списка флагов активных узлов
   //             if (activeNodes == 0)
	  //          {
   //                 hasChilds = false;
	  //          }
			//}
        //for (int i = 0; i < 4; i++)
        //{
        //    if (childs[i] != null && childs[i].curLife == 0)
        //    {
        //        childs[i] = null;
        //        activeNodes -= 1;       //удаляем узел из списка флагов активных узлов
        //    }
        //}

		// check collisions here
		//if (parent == null)
		//{

		//}
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

    private int getIndex(CollisionCircle unit)
	{
        //int index = -1;
        //var verticalMidpoint = rect.x + halfWidth;
        //var horizontalMidpoint = rect.y + halfHeight;
        //if (rectF.X < verticalMidpoint && rectF.X + rectF.Width < verticalMidpoint)
        //{
        //	if (rectF.Y < horizontalMidpoint && rectF.Height < horizontalMidpoint)
        //		index = 1;
        //	else if (rectF.Y > horizontalMidpoint)
        //		index = 2;
        //}
        //else if (rectF.X > verticalMidpoint)
        //{
        //	if (rectF.Y < horizontalMidpoint && rectF.Height < horizontalMidpoint)
        //		index = 0;
        //	else if (rectF.Y > horizontalMidpoint)
        //		index = 3;
        //}
		for (int i = 0; i < 4; i++)
		{
            if (childs[i] != null)
                //if (childs[i].rect.Contains(unit.point))
                if (RectContainsCircle(childs[i].rect, unit))
                    return i;
		}

		return -1;
	}

	public List<CollisionCircle> Retrieve(List<CollisionCircle> returnObjects, CollisionCircle unit)
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

	public void Insert(CollisionCircle unit)
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

	//public void Insert(ICollidableUnit obj)
	//{
	//    objects.Add(obj);

	//    if (objects.Count > 1 && rect.size.x > Vector2.one.x * 2 && rect.size.y > Vector2.one.y * 2)
	//    {
	//        Split();

	//        int k = objects.Count - 1;
	//        while (k >= 0)
	//        {
	//            ICollidableUnit unit = objects[k];
	//            for (int i = 0; i < 4; i++)
	//            {
	//                //if (CollisionManager.Instance.RectIntersectsWithCircle(quadrant[i], halfWidthQuadrant, halfHeightQuadrant, obj.GetPoint(), obj.GetRadius()))
	//                if (childs[i].rect.Contains(objects[k].GetPoint()))
	//                {
	//                    // using existing child
	//                    //if (childs[i] != null)
	//                    //{
	//                    //    objects.RemoveAt(objects.Count - 1);
	//                    //    childs[i].Insert(unit);
	//                    //}
	//                    // create new child
	//                    //else
	//                    //{

	//                    childs[i].Insert(unit);
	//                    //hasChilds = true;
	//                    //activeNodes |= (byte)(1 << i);
	//                    //}
	//                    //Reallocate units here into its child
	//                    //foreach (ICollidableUnit unit in objects)
	//                    //{
	//                    //    ICollidableUnit movedUnit = unit;
	//                    //    objects.Remove(unit);
	//                    //    childs[i].Insert(movedUnit);
	//                    //}
	//                    //int objectsCo = objects.Count;
	//                    //for (int k = 0; k < objects.Count - 1; k++)
	//                    //{
	//                    //    childs[i].Insert(objects[k]);
	//                    //    objects.Remove(objects[k]);
	//                    //}
	//                    //return;
	//                }
	//                objects.RemoveAt(k);
	//                k--;
	//            }
	//            //objects.RemoveAt(k);
	//            //}
	//        }

	//        //objects.Add(obj);
	//    }
	//}

	public bool RectContainsCircle(Rect rect, CollisionCircle circle)
	{
        if (!rect.Contains(circle.point))
            return false;
        if (circle.point.x - circle.radius < rect.x)
            return false;
        if (circle.point.x + circle.radius > rect.width)
            return false;
        if (circle.point.y - circle.radius < rect.y)
            return false;
        if (circle.point.y + circle.radius > rect.height)
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
