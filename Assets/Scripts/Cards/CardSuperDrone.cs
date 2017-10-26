using UnityEngine;

public class CardSuperDrone : CardProperty
{
	[Header("CardSuperDrone")]
	public GameObject SuperDronePrefab;

	public override bool Activate(Vector2 position)
	{
		if (!base.Activate(position))
		{
			return false;
		}
		if (Planet != null)
		{
			Planet.Spawner.Prefab = SuperDronePrefab;
			return true;
		}
		return false;
	}
	
}
