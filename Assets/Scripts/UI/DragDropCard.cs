using UnityEngine;

public class DragDropCard : UIDragDropItem
{
	public GameObject prefab;
	
	GameObject border;

	private void Awake()
	{
		base.Awake();
		//border = transform.GetChild(0).gameObject;
	}

	protected override void OnDragDropStart()
	{
		
		base.OnDragDropStart();
		
		//border.SetActive(false);
	}

	protected override void OnDragDropEnd()
	{
		base.OnDragDropEnd();
		Instantiate(prefab, GetWorldCoordinate(), transform.rotation);
	}

	protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface != null)
		{
			Base dds = surface.GetComponent<Base>();
			
			//border.SetActive(true);
			
			if (dds != null)
			{
				Debug.Log("dds not null");
				GameObject child = NGUITools.AddChild(dds.gameObject, prefab);
				child.transform.localScale = dds.transform.localScale;

				Transform trans = child.transform;
				trans.position = UICamera.lastWorldPosition;

//				if (dds.rotatePlacedObject)
//				{
//					trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
//				}
				
				// Destroy this icon as it's no longer needed
				NGUITools.Destroy(gameObject);
				return;
			}
		}
		base.OnDragDropRelease(surface);
	}
	
	public Vector2 GetWorldCoordinate()
	{
		Vector2 worldPos = UICamera.currentCamera.WorldToViewportPoint(mTrans.position);
		worldPos = Camera.main.ViewportToWorldPoint(worldPos);
		return worldPos;       
	}
}
