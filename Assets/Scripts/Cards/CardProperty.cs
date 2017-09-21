using UnityEngine;

public class CardProperty : Card
{
	[Header("CardProperty")]
	public UISprite propertySprite;

	public override bool Activate(Vector2 position)
	{
		return true;
	}
	
}
