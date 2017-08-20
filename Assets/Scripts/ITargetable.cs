using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
	Drone DroneObj { get; }
	Base BaseObj { get; }
	GameObject GameObj { get; }
}
