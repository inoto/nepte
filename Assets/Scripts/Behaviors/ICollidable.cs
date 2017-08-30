using UnityEngine;
using System.Collections;

public enum CollisionType
{
	Body,
	Radar,
	Weapon
}

public interface ICollidable
{
	Vector2 Point { get; }
	float Radius { get; }
	float RadiusHard { get; }
	CollisionType collisionType { get; }
	GameObject GameObject { get; }
	bool Active { get; }
	Owner Owner { get; }
    Mover Mover { get; }
	//void CheckCollision(ICollidable other);
    //bool IsStatic();
    //void AddSeparation(ICollidable other);
    //void AddCohesionAlign(ICollidable other);
}
