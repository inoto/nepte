using UnityEngine;

public class CardProperty : Card
{
	[Header("CardProperty")]
	public GameObject PropertySpritePrefab;
	protected GameObject Icon;
	protected Planet Planet;
	
	public override void Drag()
	{
		base.Drag();
		Destroy(transform.GetChild(0).gameObject);

		if (Sprite != null)
		{
			Sprite.width = 10;
			
			Color col = Sprite.color;
			col.a = 0.5f;
			var button = GetComponent<UIButton>();
			button.defaultColor = col;
			
			Sprite.spriteName = "circle_small";
		}
	}

	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		
		RaycastHit2D hit = Physics2D.Raycast(position,Vector2.zero);
		if (hit)
		{
			//Debug.Log("hit base");
			Planet = hit.collider.gameObject.GetComponent<Planet>();
			if (Planet != null)
			{
				if (PropertySpritePrefab != null)
				{
					Icon = GameObject.Instantiate(PropertySpritePrefab, Planet.Trans.position, Planet.Trans.rotation);
					Icon.transform.parent = Planet.Trans;
					Planet.PropertyIcon = Icon;
				}
				return true;
			}
		}
		return false;
	}
	
}
