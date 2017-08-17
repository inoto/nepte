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

    public Vector2 point;
    public float radius;
    public float radiusHard;
    public Unit unit;
    public Base bas;
    public Radar radar;
    public Weapon weapon;

    public CollisionCircle(Vector2 _point, float _radius, Unit _unit)
    {
        point = _point;
        radius = _radius;
        radiusHard = radius / 2 + radius / 5;
        unit = _unit;
        type = Type.Unit;
    }

	public CollisionCircle(Vector2 _point, float _radius, Base _bas)
	{
		point = _point;
		radius = _radius;
		radiusHard = radius / 2 + radius / 5;
		bas = _bas;
        type = Type.Base;
	}

	public CollisionCircle(Vector2 _point, float _radius, Radar _radar)
	{
		point = _point;
		radius = _radius;
		radiusHard = radius / 2 + radius / 5;
		radar = _radar;
        type = Type.Radar;
	}

    public CollisionCircle(Vector2 _point, float _radius, Weapon _weapon)
	{
		point = _point;
		radius = _radius;
		radiusHard = radius / 2 + radius / 5;
		weapon = _weapon;
        type = Type.Weapon;
	}
}
