using System;
using UnityEngine;

[System.Serializable]
public class ConfigMothership
{
	public int HealthMax;

	[Header("Mover")]
	public float SpeedMax;
	public float MovingAroundRadius;
	
	[Header("Weapon")]
	public float AttackSpeed;
	public int AttackDamage;
	public float AttackRadius;
	public string AttackMissilePrefabName;
}