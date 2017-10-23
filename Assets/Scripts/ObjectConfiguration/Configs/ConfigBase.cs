using System;
using UnityEngine;

[Serializable]
public class ConfigBase
{
	public int HealthMax;
	public float ColliderRadius;
	public float ProduceUnitInterval;
	public string SpawnUnitPrefabName;
	public int UnitCountMax;
	public int CaptureUnitCount;
	
	[Header("Weapon")]
	public float AttackSpeed;
	public int AttackDamage;
	public float AttackRadius;
	public string AttackMissilePrefabName;
}