using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRecord
{

	public ICollidable collided1;
	public ICollidable collided2;

    public Vector2 vectorsSubtraction;
    public float distance;

	/// Это ссылка на текущий узел в октодереве, в котором возникла коллизия. В некоторых случаях обработчику коллизий
	/// может потребоваться возможность создания новых объектов и их вставки в дерево. Этот узел - хорошая начальная точка для вставки таких объектов,
	/// потому что это очень близкое приближение к тому, где они должны находиться в дереве.
	public QuadTreeEternal treeNode;

	/// Сверяем идентификаторы объектов между двумя записями пересечений. Если они совпадают в любом порядке, то у нас есть дубликат. 
	/// 
	///другой объект, с которым мы сравниваем
	/// истинно, если записи - это пересечение для одной пары объектов, в противном случае ложно.
	public bool Equals(CollisionRecord otherRecord)
	{
		//CollisionRecord o = (CollisionRecord)otherRecord;
		//возврат
        if (collided1 != null && collided2 != null && collided1.GameObject.GetInstanceID() == collided2.GameObject.GetInstanceID())
		{
			if (otherRecord.collided1.GameObject.GetInstanceID() == collided1.GameObject.GetInstanceID() && otherRecord.collided2.GameObject.GetInstanceID() == collided2.GameObject.GetInstanceID()) return true;
			if (otherRecord.collided1.GameObject.GetInstanceID() == collided1.GameObject.GetInstanceID() && otherRecord.collided2.GameObject.GetInstanceID() == collided1.GameObject.GetInstanceID()) return true;
		}
		return false;
	}

	public CollisionRecord()
	{
		distance = float.MaxValue;
		collided1 = null;
		collided2 = null;
	}

	/// Создаёт новую запись пересечений, сообщающую о том, было ли пересечение, и с каким объектом оно произошло.
	///Не обязательно: объект, с которым произошло пересечение. По умолчанию null. 
	//public CollisionRecord(ICollidable hitObject = null)
	//{
	//	collided1 = hitObject;
	//	distance = 0.0f;
	//}

	public CollisionRecord(ICollidable _unit1, ICollidable _unit2)
	{
		//hasHit = hitObject != null;
		collided1 = _unit1;
		collided2 = _unit2;
        vectorsSubtraction = Vector2.zero;
		distance = 0;
	}

	public CollisionRecord(ICollidable _unit1, ICollidable _unit2, Vector2 _vectorsSubtraction, float _dist)
	{
		//hasHit = hitObject != null;
		collided1 = _unit1;
		collided2 = _unit2;
		vectorsSubtraction = _vectorsSubtraction;
		distance = _dist;
	}
}
