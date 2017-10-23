using UnityEngine;

public class CardSuperDrone : CardProperty
{
	[Header("CardSuperDrone")]
	public GameObject superDronePrefab;

	public override bool Activate(Vector2 position)
	{
		if (!base.Activate(position))
			return false;
		if (bas != null)
		{
			bas.Spawner.prefab = superDronePrefab;
			return true;
		}
		return false;
	}
	
}
