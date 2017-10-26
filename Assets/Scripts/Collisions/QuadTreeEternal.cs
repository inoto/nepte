﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuadTreeEternal
{
	public int level = 0;

	public Rect rect;
	float halfWidth, halfHeight;

	public List<CollisionCircle> objects;
	private List<CollisionCircle> movedUnits;

	int maxLifespan = 8;
	int curLife = -1;

	public QuadTreeEternal parent = null;

	public QuadTreeEternal[] childs = new QuadTreeEternal[4];
	bool hasChilds = false;
	byte activeNodes = 0;

	public bool built = false;

	public QuadTreeEternal root = null;

	public QuadTreeEternal(Rect newRect)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = new List<CollisionCircle>();
		movedUnits = new List<CollisionCircle>();
		parent = null;
		childs = new QuadTreeEternal[4];
		for (int i = 0; i < 4; i++)
		{
			childs[i] = null;
		}
	}

	public QuadTreeEternal(Rect newRect, List<CollisionCircle> units)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = units;
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
					curLife--;
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

		movedUnits.Clear();
		movedUnits.AddRange(objects);
		//foreach (ICollidable unit in objects)
		//{
		//	if (unit.Active)
		//		movedUnits.Add(unit);
		//}

		// remove died objects
		int listSize = objects.Count;
		for (int i = 0; i < listSize; i++)
		{
            //if (objects[i].trans == null || objects[i].isDead)
	        if (objects[i].isDead)
            {
	            if (movedUnits.Contains(objects[i]))
	            {
		            int ind = movedUnits.IndexOf(objects[i]);
		            movedUnits[ind].isInQT = false;
		            movedUnits.Remove(objects[i]);
		            
	            }
	            objects[i].isInQT = false;
                objects.RemoveAt(i--);
	            
	            listSize--;
            }
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) childs[index].Update();

		for (int i = 0; i < movedUnits.Count; i++)
		{
			QuadTreeEternal currentNode = this;
	
			while (!RectContainsCircle(rect, movedUnits[i]))
			{
				if (currentNode.parent != null) currentNode = currentNode.parent;
				else break;
			}
			// remove from objects and insert to root in EVERY FRAME
			movedUnits[i].isInQT = false;
			objects.Remove(movedUnits[i]);
			
			currentNode.Insert(movedUnits[i]);
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1 && childs[index].curLife == 0 && childs[index].objects.Count == 0)
			{
				if (childs[index].objects.Count > 0)
					Debug.Log(childs[index].objects.Count);
				childs[index] = null;
				activeNodes ^= (byte)(1 << index);
				if (activeNodes == 0)
					hasChilds = false;
			}

		//check collisions here
		if (parent == null)
		{
			CheckCollisions(new List<CollisionCircle>());
		}
	}

	public void Insert(CollisionCircle obj)
	{
		if (objects.Count < 1 && activeNodes == 0)
		{
			objects.Add(obj);
			obj.isInQT = true;
			return;
		}

		if (rect.size.x < Vector2.one.x && rect.size.y < Vector2.one.y)
		{
			objects.Add(obj);
			obj.isInQT = true;
			return;
		}

		Rect[] quadrant = new Rect[4];
		quadrant[0] = (childs[0] != null) ? childs[0].rect : new Rect(rect.x + halfWidth, rect.y, halfWidth, halfHeight);
		quadrant[1] = (childs[1] != null) ? childs[1].rect : new Rect(rect.x, rect.y, halfWidth, halfHeight);
		quadrant[2] = (childs[2] != null) ? childs[2].rect : new Rect(rect.x, rect.y + halfHeight, halfWidth, halfHeight);
		quadrant[3] = (childs[3] != null) ? childs[3].rect : new Rect(rect.x + halfWidth, rect.y + halfHeight, halfWidth, halfHeight);

		if (RectContainsCircle(rect, obj))
		{
			bool found = false;
			for (int i = 0; i < 4; i++)
			{
				if (RectContainsCircle(quadrant[i], obj))
				//if (quadrant[i].Contains(obj.GetPoint()))
				{
					// using existing child
					if (childs[i] != null)
					{
						childs[i].Insert(obj);
					}
					// create new child
					else
					{
						childs[i] = new QuadTreeEternal(quadrant[i]);
						childs[i].root = root;
						childs[i].parent = this;
						childs[i].level = level + 1;
						childs[i].Insert(obj);
						hasChilds = true;
						activeNodes |= (byte)(1 << i);
					}
					found = true;
				}
			}
			if (!found)
			{
				objects.Add(obj);
				obj.isInQT = true;
			}
		}
		else
		{
            // attach object to root if it's outside of battleground boundaries
            //objects.Add(obj);
			obj.isInQT = false;
		}
	}

	

    private void CheckCollisions(List<CollisionCircle> parentObjs)
	{
		//List<CollisionRecord> collisionsList = new List<CollisionRecord>();

		// считаем, что для всех родительских объектов уже были выполнены проверки коллизий. 
		// проверяем коллизии всех родительских объектов со всеми объектами в локальном узле
		for (int p = 0; p < parentObjs.Count; p++)
		//foreach (CollisionCircle pObj in parentObjs)
		{
			for (int l = 0; l < objects.Count; l++)
			//foreach (CollisionCircle lObj in objects)
			{
//                if (parentObjs[p].collisionType == CollisionCircle.CollisionType.Body && objects[l].collisionType == CollisionCircle.CollisionType.Body)
				CheckBodies(parentObjs[p], objects[l]);
				if (parentObjs[p].isWeapon || objects[l].isWeapon)
					CheckWeapon(parentObjs[p], objects[l]);
				
				//CollisionRecord cr = CheckCollision(pObj, lObj);
				//if (cr != null)
				    //collisionsList.Add(cr);
			}
		}

//		foreach (var lObj1 in objects)
//		{
//			foreach (var lObj2 in objects)
//			{
//				CheckBodies(lObj1, lObj2);
//				if (lObj1.isWeapon || lObj2.isWeapon)
//					CheckWeapon(lObj1, lObj2);
//			}
//		}
		
		// Теперь проверяем коллизии всех локальных объектов между собой
		if (objects.Count > 1)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				List<CollisionCircle> tmpList = new List<CollisionCircle>(objects.Count);
				tmpList.AddRange(objects);

				while (tmpList.Count > 0)
				{
					//foreach (CollisionCircle lObj2 in tmpList)
					for (int l = 0; l < tmpList.Count; l++)
					{
//                        if (tmpList[tmpList.Count - 1].collisionType == CollisionCircle.CollisionType.Body && tmpList[l].collisionType == CollisionCircle.CollisionType.Body)
                            CheckBodies(tmpList[tmpList.Count - 1], tmpList[l]);
						if (tmpList[tmpList.Count - 1].isWeapon || tmpList[l].isWeapon)
							CheckWeapon(tmpList[tmpList.Count - 1], tmpList[l]);
						//CollisionRecord cr = CheckCollision(tmpList[tmpList.Count - 1], lObj2);
						//if (cr != null)
							//collisionsList.Add(cr);
					}

					// Удаляем этот объект из временного списка, чтобы выполнять процедуру O(N(N+1)/2) раз вместо O(N*N)
					tmpList.RemoveAt(tmpList.Count - 1);
				}
			}
		}

		// Теперь объединяем список локальных объектов со списком родительских объектов, а затем передаём его вниз всем дочерним узлам.
		//foreach (CollisionCircle lObj in objects) /*if (lObj.IsStatic == false)*/ parentObjs.Add(lObj);
		for (int l = 0; l < objects.Count; l++) parentObjs.AddRange(objects);

		//parentObjs.AddRange(m_objects);
		//каждый дочерний узел даст нам список записей пересечений, который мы затем объединим с собственными записями пересечений.
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1)
				childs[index].CheckCollisions(parentObjs);
				//return collisionsList;
	}

    void CheckBodies(CollisionCircle unit1, CollisionCircle unit2)
	{
	    if (unit1.instanceId == unit2.instanceId)
	    	return;
		if (unit1.isStatic && unit2.isStatic)
			return;
		if (unit1.isDead || unit2.isDead)
			return;
		if (unit1.isWeapon || unit2.isWeapon)
			return;
		float distance = (unit1.trans.position - unit2.trans.position).sqrMagnitude;
		// if no mover then it's a base
		if (unit1.isStatic)
		{
			if (unit1.owner.PlayerNumber == -1)
			{
				float radiuses = unit1.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit2.isCollidedWithBase == false)
					{
						unit2.collidedBaseCircle = unit1;
						unit2.isCollidedWithBase = true;
						unit1.collidedCount++;
					}
				}
				else
				{
					if (unit2.collidedBaseCircle == unit1)
					{
						unit2.collidedBaseCircle = null;
						unit2.isCollidedWithBase = false;
						unit1.collidedCount--;
					}
				}
			}
			else
			{
				float radiuses = unit1.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit1.owner.PlayerNumber == unit2.owner.PlayerNumber)
					{
						if (!unit2.isDead)
						{
							if (unit2.mover.FollowBase.TargetPlanet.gameObject != unit1.trans.gameObject)
								if (unit2.mover.Separation.Enabled)
									unit2.mover.Separation.AddSeparation(unit1.trans.position, distance);
						}
					}
				}
			}
		}
		// if no mover then it's a base
		else if (unit2.isStatic)
		{
			if (unit2.owner.PlayerNumber == -1)
			{
				float radiuses = unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit1.isCollidedWithBase == false)
					{
						unit1.collidedBaseCircle = unit2;
						unit1.isCollidedWithBase = true;
						unit2.collidedCount++;
					}
				}
				else
				{
					if (unit1.collidedBaseCircle == unit2)
					{
						unit1.collidedBaseCircle = null;
						unit1.isCollidedWithBase = false;
						unit2.collidedCount--;
					}
				}
			}
			else
			{
				float radiuses = unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit2.owner.PlayerNumber == unit1.owner.PlayerNumber)
					{
						if (!unit1.isDead)
						{
							if (unit1.mover.FollowBase.TargetPlanet.gameObject != unit2.trans.gameObject)
								if (unit1.mover.Separation.Enabled)
									unit1.mover.Separation.AddSeparation(unit2.trans.position, distance);
						}
					}
				}
			}
		}
		else if (!unit1.isStatic && !unit2.isStatic)
		{
			if (distance > 0)
			{
				// check all dynamic bodies to apply separation
				if (distance < unit1.mover.Separation.Desired * unit1.mover.Separation.Desired)
				{
					if (unit1.mover.Separation.Enabled)
						unit1.mover.Separation.AddSeparation(unit2.trans.position, distance);
					if (unit2.mover.Separation.Enabled)
						unit2.mover.Separation.AddSeparation(unit1.trans.position, distance);
				}
				// check ally bodies only
				if (unit1.owner.PlayerNumber == unit2.owner.PlayerNumber)
				{
					// check to apply cohesion
					if (distance < unit1.mover.Cohesion.Desired * unit1.mover.Cohesion.Desired)
					{
						if (unit1.mover.Cohesion.Enabled)
							unit1.mover.Cohesion.AddCohesion(unit2.trans.position);
						if (unit2.mover.Cohesion.Enabled)
							unit2.mover.Cohesion.AddCohesion(unit1.trans.position);
					}
				}
			}
			else
			{
				unit2.trans.Translate(-0.2f, 0, 0);
				unit1.trans.Translate(0.2f, 0, 0);
			}
		}
	}

	void CheckWeapon(CollisionCircle unit1, CollisionCircle unit2)
	{
		if (unit1.instanceId == unit2.instanceId)
			return;
		if (unit1.owner.PlayerNumber == unit2.owner.PlayerNumber)
			return;
		if (unit1.owner.PlayerNumber == -1 || unit2.owner.PlayerNumber == -1)
			return;
		if (unit1.isDead || unit2.isDead)
			return;
		if (unit1.isWeapon && unit2.isWeapon)
			return;
		float distance = (unit1.trans.position - unit2.trans.position).sqrMagnitude;
		if (distance > 0)
		{
			if (unit1.isWeapon)
			{
				float radiuses = unit1.GetRadius() + unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (!unit1.weapon.HasTarget)
					{
						unit1.weapon.Target = unit2.trans.GetComponent<ITargetable>();
						unit1.weapon.HasTarget = true;
						unit1.weapon.AttackTarget();
					}
					else
					{
						unit1.weapon.AttackTarget();
					}
				}
				else
				{
					
					if (unit1.weapon.Target != null)
					{
						if (unit1.weapon.Target.GameObj == unit2.trans.gameObject)
							unit1.weapon.Target = null;
					}
				}
			}
			else if (unit2.isWeapon)
			{
				float radiuses = unit1.GetRadius() + unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (!unit2.weapon.HasTarget)
					{
						unit2.weapon.Target = unit1.trans.GetComponent<ITargetable>();
						unit2.weapon.HasTarget = true;
						unit2.weapon.AttackTarget();
					}
					else
					{
						unit2.weapon.AttackTarget();
					}
				}
				else
				{
					if (unit2.weapon.Target != null)
					{
						if (unit2.weapon.Target.GameObj == unit1.trans.gameObject)
							unit2.weapon.Target = null;
					}
				}
			}
			
		}
	}
	
	public bool RectContainsCircle(Rect rect, CollisionCircle obj)
	{
        if (!rect.Contains(obj.trans.position))
			return false;
		
		float radius = 0;
		if (obj.isStatic)
			radius = obj.GetRadius();
		
        if ((obj.trans.position.x - radius) <= rect.x)
			return false;
        if ((obj.trans.position.x + radius) >= (rect.x + rect.width))
			return false;
	    if ((obj.trans.position.y - radius) <= rect.y)
			return false;
        if ((obj.trans.position.y + radius) >= (rect.y + rect.height))
			return false;
		return true;
	}

	public List<CollisionCircle> GetWholeTreeObjects(List<CollisionCircle> list)
	{
		list.AddRange(objects);
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) childs[index].GetWholeTreeObjects(list);
		return list;
	}

	public void Clear()
	{
		objects.Clear();
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) childs[index].Clear();
	}

	public void DrawDebug()
	{
	
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height));
#if UNITY_EDITOR
		Handles.Label(rect.center, objects.Count.ToString() + ":" + curLife.ToString() + ":" + maxLifespan.ToString());
		Handles.Label(rect.center, objects.Count.ToString());
#endif

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs[i] != null)
				childs[i].DrawDebug();
		}
	}
}
