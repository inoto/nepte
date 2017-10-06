﻿using UnityEngine;

public class CardPropertyBaseDestroyer : CardProperty
{
//	[Header("CardPropertyBaseDestroyer")]
//	public GameObject superDronePrefab;

	public override bool Activate(Vector2 position)
	{
		if (!base.Activate(position))
			return false;
		if (bas != null)
		{
			bas.Damage(90000);
			return true;
		}
		return false;
	}
	
}
