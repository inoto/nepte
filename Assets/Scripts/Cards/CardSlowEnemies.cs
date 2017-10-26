using System.Collections.Generic;
using UnityEngine;

public class CardSlowEnemies : CardAction
{
	[Header("CardSlowEnemies")]
	//public Sprite areaSprite;
	public float SlowFactor = 0.5f;
	public float AreaRadius = 3f;
	public float Duration = 6f;
	
	public override void Drag()
	{
		base.Drag();
		if (Type == ActionType.Area)
		{
			Destroy(transform.GetChild(0).gameObject);

			if (Sprite != null)
			{
				Sprite.width = (int) ((AreaRadius) * ((Screen.height / 2.0f) / Camera.main.orthographicSize));
				
				Color col = Sprite.color;
				col.a = 0.5f;
				var button = GetComponent<UIButton>();
				button.defaultColor = col;

				Sprite.spriteName = "circle";
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
		List<CollisionCircle> list = CollisionManager.Instance.FindBodiesInCircleArea(position, AreaRadius);
		Debug.Log("catched units: " + list.Count);
		foreach (var unit in list)
		{
			Mover mover = unit.Trans.GetComponent<Mover>();
			if (mover == null)
			{
				continue;
			}
			var effect = unit.Trans.gameObject.AddComponent<EffectSlow>();
			effect.SlowFactor = SlowFactor;
			effect.Mover = mover;
			effect.Activate(Duration);
		}
		return true;
	}
	
}
