using UnityEngine;

public class CardProperty : Card
{
	[Header("CardProperty")]
	public GameObject propertySpritePrefab;
	protected GameObject icon;
	protected Planet bas;
	
	public override void Drag()
	{
		base.Drag();
		Destroy(transform.GetChild(0).gameObject);

		if (sprite != null)
		{
			sprite.width = 10;
			
			Color col = sprite.color;
			col.a = 0.5f;
			var button = GetComponent<UIButton>();
			button.defaultColor = col;
			
			sprite.spriteName = "circle_small";
		}
	}

	public override bool Activate(Vector2 position)
	{
		base.Activate(position);
		RaycastHit2D hit = Physics2D.Raycast(position,Vector2.zero);
		if (hit)
		{
			//Debug.Log("hit base");
			bas = hit.collider.gameObject.GetComponent<Planet>();
			if (bas != null)
			{
				if (propertySpritePrefab != null)
				{
					icon = GameObject.Instantiate(propertySpritePrefab, bas.Trans.position, bas.Trans.rotation);
					icon.transform.parent = bas.Trans;
					bas.PropertyIcon = icon;
				}
				return true;
			}
		}
		return false;
	}
	
}
