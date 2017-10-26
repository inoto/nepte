using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuadTreeEternal
{
	private int level = 0;

	private Rect rect;
	private readonly float halfWidth;
	private readonly float halfHeight;

	private readonly List<CollisionCircle> objects;
	private readonly List<CollisionCircle> movedUnits;

	private int maxLifespan = 8;
	private int curLife = -1;

	private QuadTreeEternal parent = null;

	private readonly QuadTreeEternal[] childs = new QuadTreeEternal[4];
	private bool hasChilds = false;
	private byte activeNodes = 0;

	public QuadTreeEternal Root = null;

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
				{
					curLife = maxLifespan;
				}
				else if (curLife > 0)
				{
					curLife--;
				}
			}
		}
		else
		{
			if (curLife != -1)
			{
				if (maxLifespan <= 64)
				{
					maxLifespan *= 2;
				}
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
	        if (objects[i].IsDead)
            {
	            if (movedUnits.Contains(objects[i]))
	            {
		            int ind = movedUnits.IndexOf(objects[i]);
		            movedUnits[ind].IsInQt = false;
		            movedUnits.Remove(objects[i]);
		            
	            }
	            objects[i].IsInQt = false;
                objects.RemoveAt(i--);
	            
	            listSize--;
            }
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
		{
			if ((flags & 1) == 1)
			{
				childs[index].Update();
			}
		}

		for (int i = 0; i < movedUnits.Count; i++)
		{
			QuadTreeEternal currentNode = this;
	
			while (!RectContainsCircle(rect, movedUnits[i]))
			{
				if (currentNode.parent != null)
				{
					currentNode = currentNode.parent;
				}
				else
				{
					break;
				}
			}
			// remove from objects and insert to root in EVERY FRAME
			movedUnits[i].IsInQt = false;
			objects.Remove(movedUnits[i]);
			
			currentNode.Insert(movedUnits[i]);
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
		{
			if ((flags & 1) == 1 && childs[index].curLife == 0 && childs[index].objects.Count == 0)
			{
				if (childs[index].objects.Count > 0)
				{
					Debug.Log(childs[index].objects.Count);
				}
				childs[index] = null;
				activeNodes ^= (byte)(1 << index);
				if (activeNodes == 0)
				{
					hasChilds = false;
				}
			}
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
			obj.IsInQt = true;
			return;
		}

		if (rect.size.x < Vector2.one.x && rect.size.y < Vector2.one.y)
		{
			objects.Add(obj);
			obj.IsInQt = true;
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
						childs[i].Root = Root;
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
				obj.IsInQt = true;
			}
		}
		else
		{
            // attach object to root if it's outside of battleground boundaries
            //objects.Add(obj);
			obj.IsInQt = false;
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
				if (parentObjs[p].IsWeapon || objects[l].IsWeapon)
				{
					CheckWeapon(parentObjs[p], objects[l]);
				}

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
						if (tmpList[tmpList.Count - 1].IsWeapon || tmpList[l].IsWeapon)
						{
							CheckWeapon(tmpList[tmpList.Count - 1], tmpList[l]);
						}
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
		for (int l = 0; l < objects.Count; l++)
		{
			parentObjs.AddRange(objects);
		}

		//parentObjs.AddRange(m_objects);
		//каждый дочерний узел даст нам список записей пересечений, который мы затем объединим с собственными записями пересечений.
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
		{
			if ((flags & 1) == 1)
			{
				childs[index].CheckCollisions(parentObjs);
			}
		}
		//return collisionsList;
	}

	private static void CheckBodies(CollisionCircle unit1, CollisionCircle unit2)
	{
	    if (unit1.InstanceId == unit2.InstanceId)
	    {
		    return;
	    }
		if (unit1.IsStatic && unit2.IsStatic)
		{
			return;
		}
		if (unit1.IsDead || unit2.IsDead)
		{
			return;
		}
		if (unit1.IsWeapon || unit2.IsWeapon)
		{
			return;
		}
		float distance = (unit1.Trans.position - unit2.Trans.position).sqrMagnitude;
		// if no mover then it's a base
		if (unit1.IsStatic)
		{
			if (unit1.Owner.PlayerNumber == -1)
			{
				float radiuses = unit1.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit2.IsCollidedWithBase == false)
					{
						unit2.CollidedBaseCircle = unit1;
						unit2.IsCollidedWithBase = true;
						unit1.CollidedCount++;
					}
				}
				else
				{
					if (unit2.CollidedBaseCircle == unit1)
					{
						unit2.CollidedBaseCircle = null;
						unit2.IsCollidedWithBase = false;
						unit1.CollidedCount--;
					}
				}
			}
			else
			{
				float radiuses = unit1.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit1.Owner.PlayerNumber == unit2.Owner.PlayerNumber)
					{
						if (!unit2.IsDead)
						{
							if (unit2.Mover.FollowBase.TargetPlanet.gameObject != unit1.Trans.gameObject)
							{
								if (unit2.Mover.Separation.Enabled)
								{
									unit2.Mover.Separation.AddSeparation(unit1.Trans.position, distance);
								}
							}
						}
					}
				}
			}
		}
		// if no mover then it's a base
		else if (unit2.IsStatic)
		{
			if (unit2.Owner.PlayerNumber == -1)
			{
				float radiuses = unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit1.IsCollidedWithBase == false)
					{
						unit1.CollidedBaseCircle = unit2;
						unit1.IsCollidedWithBase = true;
						unit2.CollidedCount++;
					}
				}
				else
				{
					if (unit1.CollidedBaseCircle == unit2)
					{
						unit1.CollidedBaseCircle = null;
						unit1.IsCollidedWithBase = false;
						unit2.CollidedCount--;
					}
				}
			}
			else
			{
				float radiuses = unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (unit2.Owner.PlayerNumber == unit1.Owner.PlayerNumber)
					{
						if (!unit1.IsDead)
						{
							if (unit1.Mover.FollowBase.TargetPlanet.gameObject != unit2.Trans.gameObject)
							{
								if (unit1.Mover.Separation.Enabled)
								{
									unit1.Mover.Separation.AddSeparation(unit2.Trans.position, distance);
								}
							}
						}
					}
				}
			}
		}
		else if (!unit1.IsStatic && !unit2.IsStatic)
		{
			if (distance > 0)
			{
				// check all dynamic bodies to apply separation
				if (distance < unit1.Mover.Separation.Desired * unit1.Mover.Separation.Desired)
				{
					if (unit1.Mover.Separation.Enabled)
					{
						unit1.Mover.Separation.AddSeparation(unit2.Trans.position, distance);
					}
					if (unit2.Mover.Separation.Enabled)
					{
						unit2.Mover.Separation.AddSeparation(unit1.Trans.position, distance);
					}
				}
				// check ally bodies only
				if (unit1.Owner.PlayerNumber == unit2.Owner.PlayerNumber)
				{
					// check to apply cohesion
					if (distance < unit1.Mover.Cohesion.Desired * unit1.Mover.Cohesion.Desired)
					{
						if (unit1.Mover.Cohesion.Enabled)
						{
							unit1.Mover.Cohesion.AddCohesion(unit2.Trans.position);
						}
						if (unit2.Mover.Cohesion.Enabled)
						{
							unit2.Mover.Cohesion.AddCohesion(unit1.Trans.position);
						}
					}
				}
			}
			else
			{
				unit2.Trans.Translate(-0.2f, 0, 0);
				unit1.Trans.Translate(0.2f, 0, 0);
			}
		}
	}

	void CheckWeapon(CollisionCircle unit1, CollisionCircle unit2)
	{
		if (unit1.InstanceId == unit2.InstanceId)
		{
			return;
		}
		if (unit1.Owner.PlayerNumber == unit2.Owner.PlayerNumber)
		{
			return;
		}
		if (unit1.Owner.PlayerNumber == -1 || unit2.Owner.PlayerNumber == -1)
		{
			return;
		}
		if (unit1.IsDead || unit2.IsDead)
		{
			return;
		}
		if (unit1.IsWeapon && unit2.IsWeapon)
		{
			return;
		}
		float distance = (unit1.Trans.position - unit2.Trans.position).sqrMagnitude;
		if (distance > 0)
		{
			if (unit1.IsWeapon)
			{
				float radiuses = unit1.GetRadius() + unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (!unit1.Weapon.HasTarget)
					{
						unit1.Weapon.Target = unit2.Trans.GetComponent<ITargetable>();
						unit1.Weapon.HasTarget = true;
						unit1.Weapon.AttackTarget();
					}
					else
					{
						unit1.Weapon.AttackTarget();
					}
				}
				else
				{
					
					if (unit1.Weapon.Target != null)
					{
						if (unit1.Weapon.Target.GameObj == unit2.Trans.gameObject)
						{
							unit1.Weapon.Target = null;
						}
					}
				}
			}
			else if (unit2.IsWeapon)
			{
				float radiuses = unit1.GetRadius() + unit2.GetRadius();
				if (distance < radiuses * radiuses)
				{
					if (!unit2.Weapon.HasTarget)
					{
						unit2.Weapon.Target = unit1.Trans.GetComponent<ITargetable>();
						unit2.Weapon.HasTarget = true;
						unit2.Weapon.AttackTarget();
					}
					else
					{
						unit2.Weapon.AttackTarget();
					}
				}
				else
				{
					if (unit2.Weapon.Target != null)
					{
						if (unit2.Weapon.Target.GameObj == unit1.Trans.gameObject)
						{
							unit2.Weapon.Target = null;
						}
					}
				}
			}
		}
	}

	private static bool RectContainsCircle(Rect rect, CollisionCircle obj)
	{
        if (!rect.Contains(obj.Trans.position))
        {
	        return false;
        }

		float radius = 0;
		if (obj.IsStatic)
		{
			radius = obj.GetRadius();
		}

		if ((obj.Trans.position.x - radius) <= rect.x)
		{
			return false;
		}
		if ((obj.Trans.position.x + radius) >= (rect.x + rect.width))
		{
			return false;
		}
		if ((obj.Trans.position.y - radius) <= rect.y)
		{
			return false;
		}
		if ((obj.Trans.position.y + radius) >= (rect.y + rect.height))
		{
			return false;
		}
		return true;
	}

	public List<CollisionCircle> GetWholeTreeObjects(List<CollisionCircle> list)
	{
		list.AddRange(objects);
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
		{
			if ((flags & 1) == 1)
			{
				childs[index].GetWholeTreeObjects(list);
			}
		}
		return list;
	}

	public void Clear()
	{
		objects.Clear();
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
		{
			if ((flags & 1) == 1)
			{
				childs[index].Clear();
			}
		}
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
			{
				childs[i].DrawDebug();
			}
		}
	}
}
