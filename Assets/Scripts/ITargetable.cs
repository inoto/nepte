using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
	void Damage(Weapon weapon);
	void Damage(int damage);
	GameObject GameObj { get; }
    bool IsDied { get; }
}
