using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Drone,
    Base
}

public interface ITargetable
{
	Drone DroneObj { get; }
	Base BaseObj { get; }
	GameObject GameObj { get; }
    bool IsDied { get; }
    TargetType targetableType { get; }
}
