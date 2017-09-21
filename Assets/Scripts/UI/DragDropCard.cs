using UnityEngine;

public class DragDropCard : UIDragDropItem
{
	public GameObject prefab;
	
	GameObject border;

	private UISprite sprite;
	private UIButton button;

	private Card card;

	private void Awake()
	{
		base.Awake();
		//border = transform.GetChild(0).gameObject;
		sprite = GetComponent<UISprite>();
		button = GetComponent<UIButton>();
		card = GetComponent<Card>();
	}

//	protected override void OnClone(GameObject original)
//	{
//		base.OnClone(original);
//		sprite.color = Color.red;
//	}
	
//	public override void StartDragging ()
//	{
//		realParent = transform.parent;
//		
//		base.StartDragging();
//	}

	protected override void OnDragDropStart()
	{
		base.OnDragDropStart();

		sprite.depth += 1;
		
		button.enabled = false;
		
//		transform.parent = GameObject.Find("CardsScrollView").transform;
//		NGUITools.MarkParentAsChanged(gameObject);
		
		//sprite.color = Color.red;
	}

	protected override void OnDragDropEnd()
	{
		Vector2 pos = GetWorldCoordinate();
		
		RaycastHit2D hit = Physics2D.Raycast(pos,Vector2.zero);
		if (hit)
		{
			Debug.Log("hitdrag");
			if (hit.collider.gameObject.CompareTag("SafeZoneFromCardActivation"))
				Debug.Log("not activated");
		}
		else
		{
			card.Activate(pos);
		}
		
		
		base.OnDragDropEnd();
	}
	
//	protected override void OnDragDropMove (Vector2 delta)
//	{
//		base.OnDragDropMove(delta);
//		if (mTrans.position.x > 0)
//			sprite.color = Color.green;
//		if (mTrans.position.x < 0)
//			sprite.color = Color.red;
//	}

//	protected override void OnDragDropRelease (GameObject surface)
//	{
//		base.OnDragDropRelease(surface);
//	}
	
	public Vector2 GetWorldCoordinate()
	{
		Vector2 worldPos = UICamera.currentCamera.WorldToViewportPoint(mTrans.position);
		worldPos = Camera.main.ViewportToWorldPoint(worldPos);
		return worldPos;       
	}
}
