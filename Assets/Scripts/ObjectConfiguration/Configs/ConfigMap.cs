﻿using System;
using UnityEngine;

[System.Serializable]
public class ConfigMap
{
	public int HealthMax;

	[Header("Mover")]
	public float SpeedMax;
	public float ForceMax;
	public float TurnSpeed;
	public float FollowStopRadius;
	public bool SeparationEnabled;
	public float SeparationRadius;
	public bool CohesionEnabled;
	public float CohesionRadius;
	
	[Header("Weapon")]
	public float AttackSpeed;
	public int AttackDamage;
	public float AttackRadius;
	public string AttackMissilePrefabName;

}