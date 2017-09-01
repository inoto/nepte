using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuadTreeEternal
{
	public static List<CollisionRecord> crList = new List<CollisionRecord>();

	public int level = 0;

	public Rect rect;
	float halfWidth, halfHeight;

	public List<CollisionCircle> objects;

	int maxLifespan = 8;
	int curLife = -1;

	public QuadTreeEternal parent = null;

	public QuadTreeEternal[] childs = new QuadTreeEternal[4];
	bool hasChilds = false;
	byte activeNodes = 0;

	public bool built = false;

	public QuadTreeEternal(Rect newRect)
	{
		rect = newRect;
		halfWidth = rect.width / 2;
		halfHeight = rect.height / 2;
		objects = new List<CollisionCircle>();
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

		List<CollisionCircle> movedUnits = new List<CollisionCircle>(objects);
		//foreach (ICollidable unit in objects)
		//{
		//	if (unit.Active)
		//		movedUnits.Add(unit);
		//}

		// remove died objects
		int objectsCount = objects.Count;
		for (int i = 0; i < objectsCount; i++)
		{
            if (!objects[i].trans.gameObject.activeSelf)
            {
                if (movedUnits.Contains(objects[i]))
                    movedUnits.Remove(objects[i]);
                objects.RemoveAt(i--);
                objectsCount -= 1;
            }
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) childs[index].Update();

		// 
		foreach (CollisionCircle unit in movedUnits)
		//for (int k = 0; k < movedUnits.Count; k++)
		{
			QuadTreeEternal currentNode = this;

            while (!RectContainsCircle(rect, unit))
            //while (!rect.Contains(unit.GetPoint()))
            {
                if (currentNode.parent != null) currentNode = currentNode.parent;
                else break;
            }

            // remove from object and insert to root in EVERY FRAME
            objects.Remove(unit);
            currentNode.Insert(unit);
		}

		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1 && childs[index].curLife == 0)
			{
				childs[index] = null;
				activeNodes ^= (byte)(1 << index);
				if (activeNodes == 0)
					hasChilds = false;
			}

		//check collisions here
		if (parent == null)
		{
			crList = GetCollisions(new List<CollisionCircle>());
			
		}
	}

	public void Insert(CollisionCircle obj)
	{
		if (objects.Count < 1 && activeNodes == 0)
		{
			objects.Add(obj);
			return;
		}

		if (rect.size.x < Vector2.one.x*4 && rect.size.y < Vector2.one.y*4)
		{
			objects.Add(obj);
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
                    //Debug.Log("point.x: " + obj.Point.x + " point.y: " + obj.Point.y + " radius: " + obj.Radius
                    //          + " inserted to " + "rect.x: " + quadrant[i].x + " rect.y: " + quadrant[i].y + " rect.w: " + quadrant[i].width + " rect.h: " + quadrant[i].height);
					
					// using existing child
					if (childs[i] != null)
					{
						childs[i].Insert(obj);
					}
					// create new child
					else
					{
						childs[i] = new QuadTreeEternal(quadrant[i]);
						childs[i].parent = this;
						childs[i].level = level + 1;
						childs[i].Insert(obj);
						hasChilds = true;
						activeNodes |= (byte)(1 << i);
					}
					//Reallocate units here into its child
					//for (int k = 0; k < objects.Count; k++)
					//{
					//	childs[i].Insert(objects[k]);
					//	objects.RemoveAt(k);
					//}
					found = true;
				}
			}
			if (!found)
				objects.Add(obj);
		}
		else
		{
            // attach object to root if it is not in battleground
            objects.Add(obj);
		}
	}

    private List<CollisionRecord> GetCollisions(List<CollisionCircle> parentObjs)
	{
		List<CollisionRecord> collisionsList = new List<CollisionRecord>();

		// считаем, что для всех родительских объектов уже были выполнены проверки коллизий. 
		// проверяем коллизии всех родительских объектов со всеми объектами в локальном узле
		foreach (CollisionCircle pObj in parentObjs)
		{
			foreach (CollisionCircle lObj in objects)
			{
                // Если произошла коллизия, то пополняем список объектов соприкосновения.
                // Делаем это для обоих объектов
                // В дополнении делаем проверку что коллизии - компоненты разных объектов

                if (pObj.collisionType == CollisionCircle.CollisionType.Body && lObj.collisionType == CollisionCircle.CollisionType.Body)
                    CheckBodies(pObj, lObj);
				else if (pObj.collisionType == CollisionCircle.CollisionType.Radar || lObj.collisionType == CollisionCircle.CollisionType.Radar)
	                CheckRadar(pObj, lObj);
                
				//CollisionRecord cr = CheckCollision(pObj, lObj);
				//if (cr != null)
				    //collisionsList.Add(cr);
			}
		}

		// Теперь проверяем коллизии всех локальных объектов между собой
		if (objects.Count > 1)
		{
			//#region self-congratulation 
			/* * Это довольно примечательная часть кода. Обычно используется просто два цикла foreach, примерно так:
			* foreach(Physical lObj1 in m_objects) 
			* { 
			* foreach(Physical lObj2 in m_objects) 
			* { 
			* // код проверки пересечения
			* } 
			* } 
			* 
			* Проблема в том, что они выполняются O(N*N) раз и в том, что проверяются коллизии уже проверенных объектов.
			* Представьте, что у нас есть множество из четырёх элементов: {1,2,3,4}
			* Сначала мы проверяем {1} с {1,2,3,4}
			* Затем проверяем {2} с {1,2,3,4},
			* но мы уже проверяли {1} с {2}, так что проверка {2} с {1} будет пустой тратой времени. Можно ли пропустить эту проверку, устранив {1}?
			* Тогда всего мы получим 4+3+2+1 проверок коллизий, что равно времени O(N(N+1)/2). Если N = 10, мы уже делаем вдвое меньшее количество проверок.
			* Мы не можем просто удалять элемент в конце второго цикла for, потому что прервём итератор первого цикла foreach, поэтому придётся использовать
			* обычное*/
			for (int i = 0; i < objects.Count; i++)
			{
				List<CollisionCircle> tmpList = new List<CollisionCircle>(objects.Count);
				tmpList.AddRange(objects);

				while (tmpList.Count > 0)
				{
					foreach (CollisionCircle lObj2 in tmpList)
					{
                        //if (tmpList[tmpList.Count - 1] == lObj2 || (tmpList[tmpList.Count - 1].IsStatic && lObj2.IsStatic)) continue;
                        //IntersectionRecord ir = tmpList[tmpList.Count - 1].Intersects(lObj2);
        //                if (tmpList[tmpList.Count - 1].GameObject != lObj2.GameObject)
        //                {
        //                    if (CheckDist(tmpList[tmpList.Count - 1], lObj2))
        //                    {
        //                        tmpList[tmpList.Count - 1].AddCollided(lObj2);
        //                        lObj2.AddCollided(tmpList[tmpList.Count - 1]);
        //                    }
        //                    else
        //                    {
								//tmpList[tmpList.Count - 1].RemoveCollided(lObj2);
								//lObj2.RemoveCollided(tmpList[tmpList.Count - 1]);
                        //    }
                        //}
                        if (tmpList[tmpList.Count - 1].collisionType == CollisionCircle.CollisionType.Body && lObj2.collisionType == CollisionCircle.CollisionType.Body)
                            CheckBodies(tmpList[tmpList.Count - 1], lObj2);
						else if (tmpList[tmpList.Count - 1].collisionType == CollisionCircle.CollisionType.Radar || lObj2.collisionType == CollisionCircle.CollisionType.Radar)
							CheckRadar(tmpList[tmpList.Count - 1], lObj2);
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
		foreach (CollisionCircle lObj in objects) /*if (lObj.IsStatic == false)*/ parentObjs.Add(lObj);

		//parentObjs.AddRange(m_objects);
		//каждый дочерний узел даст нам список записей пересечений, который мы затем объединим с собственными записями пересечений.
		for (int flags = activeNodes, index = 0; flags > 0; flags >>= 1, index++)
			if ((flags & 1) == 1) collisionsList.AddRange(childs[index].GetCollisions(parentObjs));
				return collisionsList;
	}

    void CheckBodies(CollisionCircle unit1, CollisionCircle unit2)
	{
	    if (unit1.trans.gameObject.GetInstanceID() == unit2.trans.gameObject.GetInstanceID())
	            return;
	    float distance = (unit1.trans.position - unit2.trans.position).sqrMagnitude;
		if (distance > 0)
		{
            // check all bodies to apply separation
			if (distance < unit1.mover.separation.desired * unit1.mover.separation.desired)
			{
                if (unit1.mover.separation.enabled)
				    unit1.mover.separation.AddSeparation(unit2.trans.position, distance);
                if (unit2.mover.separation.enabled)
				    unit2.mover.separation.AddSeparation(unit1.trans.position, distance);
			}
			// check ally bodies only
			if (unit1.owner.playerNumber == unit2.owner.playerNumber)
            {
                // check to apply cohesion
                if (distance < unit1.mover.cohesion.desired * unit1.mover.cohesion.desired)
                {
					if (unit1.mover.cohesion.enabled)
						unit1.mover.cohesion.AddCohesion(unit2.trans.position);
					if (unit2.mover.cohesion.enabled)
						unit2.mover.cohesion.AddCohesion(unit1.trans.position);
                }
            }
		}
	}

	void CheckRadar(CollisionCircle unit1, CollisionCircle unit2)
	{
		if (unit1.trans.gameObject.GetInstanceID() == unit2.trans.gameObject.GetInstanceID())
			return;
		if (unit1.owner.playerNumber == unit2.owner.playerNumber)
			return;
		if (unit1.collisionType == CollisionCircle.CollisionType.Weapon || unit2.collisionType == CollisionCircle.CollisionType.Weapon)
			return;
		if (unit1.collisionType == CollisionCircle.CollisionType.Radar && unit2.collisionType == CollisionCircle.CollisionType.Radar)
			return;
		float distance = (unit1.trans.position - unit2.trans.position).sqrMagnitude;
		if (distance > 0)
		{
			float radiuses = unit1.GetRadius() + unit2.GetRadius();
			if (distance < radiuses * radiuses)
			{
				if (unit1.collisionType == CollisionCircle.CollisionType.Radar && unit1.mover.weapon.target == null)
					unit1.mover.weapon.NewTarget(unit2.trans.GetComponent<ITargetable>());
				else if (unit2.collisionType == CollisionCircle.CollisionType.Radar && unit2.mover.weapon.target == null)
					unit2.mover.weapon.NewTarget(unit1.trans.GetComponent<ITargetable>());
			}
		}
	}
	
	public bool RectContainsCircle(Rect rect, CollisionCircle obj)
	{
        
        if (!rect.Contains(obj.trans.position))
			return false;
        if ((obj.trans.position.x - obj.GetRadius()) < rect.x)
			return false;
        if ((obj.trans.position.x + obj.GetRadius()) > (rect.x + rect.width))
				return false;
	    if ((obj.trans.position.y - obj.GetRadius()) < rect.y)
				return false;
        if ((obj.trans.position.y + obj.GetRadius()) > (rect.y + rect.height))
				return false;
		return true;
	}

	public void DrawDebug()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y));
		Gizmos.DrawLine(new Vector3(rect.x + rect.width, rect.y), new Vector3(rect.x + rect.width, rect.y + rect.height));
		Gizmos.DrawLine(new Vector3(rect.x, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y + rect.height));
#if UNITY_EDITOR
		Handles.Label(rect.center, objects.Count.ToString());
#endif

		for (int i = 0; i < childs.Length; i++)
		{
			if (childs[i] != null)
				childs[i].DrawDebug();
		}
	}
}
