using UnityEngine;

public class CardProperty : Card
{
	[Header("CardProperty")]
	public GameObject propertySpritePrefab;
	protected Base bas;

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
//				bas.spawner.prefab = superDronePrefab;
				GameObject ms = GameObject.Instantiate(propertySpritePrefab, bas.trans.position, bas.trans.rotation);
				ms.transform.parent = bas.trans;
				return true;
			}
		}
		return false;
	}
	
}
