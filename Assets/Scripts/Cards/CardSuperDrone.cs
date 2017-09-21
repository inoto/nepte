using UnityEngine;

public class CardSuperDrone : Card
{
	public GameObject moldSprite;
	
	public GameObject superDronePrefab;

	public override void Activate(Vector2 position)
	{
		base.Activate(position);
		RaycastHit2D hit = Physics2D.Raycast(position,Vector2.zero);
		if (hit)
		{
			Debug.Log("hit");
			Base bas = hit.collider.gameObject.GetComponent<Base>();
			bas.spawner.prefab = superDronePrefab;
			GameObject ms = GameObject.Instantiate(moldSprite, bas.trans.position, bas.trans.rotation);
			ms.transform.parent = bas.trans;
		}
	}
	
}
