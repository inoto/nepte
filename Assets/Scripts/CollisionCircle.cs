using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCircle
{
    public enum Type
    {
        Unit,
        Base,
        Radar,
        Weapon
    }
    public Type type;

    public Drone drone;
    public Base bas;

    public bool collided = false;

    public CollisionCircle(Vector2 _point, float _radius, Drone _unit)
    {
        //point = _point;
        //radius = _radius;
        //radiusHard = radius / 2 + radius / 5;
        drone = _unit;
        type = Type.Unit;
    }

	public CollisionCircle(Vector2 _point, float _radius, Base _bas)
	{
		//point = _point;
		//radius = _radius;
		//radiusHard = radius / 2 + radius / 5;
		bas = _bas;
        type = Type.Base;
	}
}
