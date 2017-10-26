using UnityEngine;

public class CardUnitCleaner : CardAction
{
	[Header("CardUnitCleaner")]
	public int Damage = 1000;
	
	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		foreach (var unit in GameManager.FindObjectsOfType<Drone>())
		{
			unit.Damage(Damage);
		}
		return true;
	}
	
}
