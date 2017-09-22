using UnityEngine;

public class CardSlowEnemies : CardAction
{
	[Header("CardSlowEnemies")]
	//public Sprite areaSprite;
	public float slowFactor = 0.5f;
	public float areaSize = 4f;
	
	
	public override void Drag()
	{
		base.Drag();
		if (actionType == ActionType.Area)
		{
			var sprite = GetComponent<UISprite>();
			
			sprite.width = 200;
			Color col = sprite.color;
			col.a = 0.5f;
			var button = GetComponent<UIButton>();
			button.defaultColor = col;
			
			sprite.spriteName = "circle";
		}
//		else if (actionType == ActionType.NoTarget)
//		{
//			
//		}
	}
	
	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		
		
		
		return true;
	}
	
}
