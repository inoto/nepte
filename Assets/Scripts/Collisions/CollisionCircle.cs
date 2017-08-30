using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCircle
{
    public enum CollisionType
    {
        Body,
        Radar,
        Weapon
    }
    public CollisionType collisionType;

    public Body body;
    public Radar radar;
    public Weapon weapon;

    public bool isCollided = false;

    public List<CollisionCircle> collidedList;

    public CollisionCircle(Body _body)
    {
        body = _body;
        collisionType = CollisionType.Body;
        isCollided = false;
        collidedList = new List<CollisionCircle>();
    }
	public CollisionCircle(Radar _radar)
	{
		radar = _radar;
        collisionType = CollisionType.Radar;
		isCollided = false;
		collidedList = new List<CollisionCircle>();
	}
	public CollisionCircle(Weapon _weapon)
	{
		weapon = _weapon;
        collisionType = CollisionType.Weapon;
		isCollided = false;
		collidedList = new List<CollisionCircle>();
	}
}
