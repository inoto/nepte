using UnityEngine;

public class CardUnitCleaner : Card
{
	public GameObject moldSprite;

	public override void Activate(Vector2 position)
	{
		base.Activate(position);
		foreach (var unit in GameController.FindObjectsOfType<Drone>())
		{
			unit.Damage(1000);
		}
	}
	
}
