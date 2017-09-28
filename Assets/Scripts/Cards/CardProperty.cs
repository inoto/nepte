using UnityEngine;

public class CardProperty : Card
{
	[Header("CardProperty")]
	public GameObject propertySpritePrefab;
	protected GameObject icon;
	protected Base bas;
	
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
			bas = hit.collider.gameObject.GetComponent<Base>();
			if (bas != null)
			{
				icon = GameObject.Instantiate(propertySpritePrefab, bas.trans.position, bas.trans.rotation);
				icon.transform.parent = bas.trans;
				bas.propertyIcon = icon;
				return true;
			}
		}
		return false;
	}
	
}
