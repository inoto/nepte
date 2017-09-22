﻿using UnityEngine;

public class CardUnitCleaner : CardAction
{
	[Header("CardUnitCleaner")] public int damage = 1000;
	
	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		foreach (var unit in GameController.FindObjectsOfType<Drone>())
		{
			unit.Damage(damage);
		}
		return true;
	}
	
}
