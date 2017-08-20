using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRecord
{
	public enum Type
	{
		UUHard,
		UUSoft,
		UBHard,
		BUHard,
		RU,
		UR,
		RB,
		BR,
		WU,
		UW,
		WB,
		BW
	}
	public Type type;

	ICollidable collidedObject1;
	/// Это объект, который был пересечён
	public ICollidable CollidedObject
	{
		get { return collidedObject1; }
		set { collidedObject1 = value; }
	}
	ICollidable collidedObject2;
	/// Это другой пересечённый объект (может быть равным null, например, в случае пересечения луч-объект)
	public ICollidable OtherCollidedObject
	{
		get { return collidedObject2; }
		set { collidedObject2 = value; }
	}

	/// Это ссылка на текущий узел в октодереве, в котором возникла коллизия. В некоторых случаях обработчику коллизий
	/// может потребоваться возможность создания новых объектов и их вставки в дерево. Этот узел - хорошая начальная точка для вставки таких объектов,
	/// потому что это очень близкое приближение к тому, где они должны находиться в дереве.
	QuadTreeEternal treeNode;

	/// Сверяем идентификаторы объектов между двумя записями пересечений. Если они совпадают в любом порядке, то у нас есть дубликат. 
	/// 
	///другой объект, с которым мы сравниваем
	/// истинно, если записи - это пересечение для одной пары объектов, в противном случае ложно.
	public bool Equals(CollisionRecord otherRecord)
	{
		//CollisionRecord o = (CollisionRecord)otherRecord;
		//возврат
		if (collidedObject1 != null && collidedObject2 != null && collidedObject1.InstanceId == collidedObject2.InstanceId)
		{
			if (otherRecord.collidedObject1.InstanceId == collidedObject1.InstanceId && otherRecord.collidedObject2.InstanceId == collidedObject2.InstanceId) return true;
			if (otherRecord.collidedObject1.InstanceId == collidedObject2.InstanceId && otherRecord.collidedObject2.InstanceId == collidedObject1.InstanceId) return true;
		}
		return false;
	}

	Vector2 vectorsSubtraction;
	public Vector2 VectorsSubtraction
	{
		get { return vectorsSubtraction; }
		set { vectorsSubtraction = value; }
	}

	float distance;
	/// Это расстояние от луча до точки пересечения. 
	/// Если вы получаете несколько пересечений, обычно нужно использовать ближайшую точку коллизии.
	public float Distance
	{
		get { return distance; }
		set { distance = value; }
	}

	private bool hasHit = false;
	public bool HasHit
	{
		get { return hasHit; }
	}

	public CollisionRecord()
	{
		distance = float.MaxValue;
		collidedObject1 = null;
		collidedObject2 = null;
	}

	/// Создаёт новую запись пересечений, сообщающую о том, было ли пересечение, и с каким объектом оно произошло.
	///Не обязательно: объект, с которым произошло пересечение. По умолчанию null. 
	public CollisionRecord(ICollidable hitObject = null)
	{
		hasHit = hitObject != null;
		collidedObject1 = hitObject;
		distance = 0.0f;
	}

	public CollisionRecord(ICollidable _unit1, ICollidable _unit2)
	{
		//hasHit = hitObject != null;
		collidedObject1 = _unit1;
		collidedObject2 = _unit2;
		distance = 0;
	}

	public CollisionRecord(ICollidable _unit1, ICollidable _unit2, Vector2 _vectorsSubtraction, float _dist)
	{
		//hasHit = hitObject != null;
		collidedObject1 = _unit1;
		collidedObject2 = _unit2;
		vectorsSubtraction = _vectorsSubtraction;
		distance = _dist;
	}

	public CollisionRecord(ICollidable _unit1, ICollidable _unit2, Vector2 _vectorsSubtraction, float _dist, Type _type)
	{
		//hasHit = hitObject != null;
		collidedObject1 = _unit1;
		collidedObject2 = _unit2;
		vectorsSubtraction = _vectorsSubtraction;
		distance = _dist;
		type = _type;
	}
}
