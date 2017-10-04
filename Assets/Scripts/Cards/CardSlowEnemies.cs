using System.Collections.Generic;
using UnityEngine;

public class CardSlowEnemies : CardAction
{
	[Header("CardSlowEnemies")]
	//public Sprite areaSprite;
	public float slowFactor = 0.5f;
	public float areaRadius = 3f;
	public float duration = 6f;
	
	public override void Drag()
	{
		base.Drag();
		if (actionType == ActionType.Area)
		{
			Destroy(transform.GetChild(0).gameObject);

			if (sprite != null)
			{
				sprite.width = (int) ((areaRadius) * ((Screen.height / 2.0f) / Camera.main.orthographicSize));
				
				Color col = sprite.color;
				col.a = 0.5f;
				var button = GetComponent<UIButton>();
				button.defaultColor = col;

				sprite.spriteName = "circle";
			}
		}
//		else if (actionType == ActionType.NoTarget)
//		{
//			
//		}
	}
	
	public override bool Activate(Vector2 position)
	{
		base.Activate(position);

		//var sprite = GetComponent<UISprite>();
		List<CollisionCircle> list = CollisionManager.Instance.FindBodiesInCircleArea(position, areaRadius);
		Debug.Log("catched units: " + list.Count);
		foreach (var unit in list)
		{
			Mover mover = unit.trans.GetComponent<Mover>();
			if (mover == null)
				continue;
			var effect = unit.trans.gameObject.AddComponent<EffectSlow>();
			effect.factor = slowFactor;
			effect.mover = mover;
			effect.Activate(duration);
		}
		return true;
	}
	
}
