using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum TargetType
//{
//    Drone,
//    Base
//}

public interface ITargetable
{
	void Damage(Weapon weapon);
	GameObject GameObj { get; }
    bool IsDied { get; }
//    TargetType targetableType { get; }
}
