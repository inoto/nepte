using UnityEngine;

public class DragDropCard : UIDragDropItem
{
	private UISprite sprite;
	private UIButton button;
	private Card card;
	private Card cardOriginal;

	private void Awake()
	{
		base.Awake();
		
		sprite = GetComponents<UISprite>()[0];
		button = GetComponent<UIButton>();
		card = GetComponent<Card>();
	}

	protected override void OnClone(GameObject original)
	{
		base.OnClone(original);
		cardOriginal = original.GetComponent<Card>();
	}
	

	public override void StartDragging()
	{
		if (card.inCooldown)
			return;
		else
			base.StartDragging();
	}

	protected override void OnDragDropStart()
	{
		base.OnDragDropStart();
		
		sprite.depth += 1;
		button.enabled = false;

		
	}

	protected override void OnDragDropEnd()
	{
		// check the card is on card activation area
		if (mTrans.position.y > CardManager.Instance.cardActivationBounds.min.y)
		{
			Vector2 pos = GetWorldCoordinate();
			if (card.Activate(pos))
				cardOriginal.StartCooldownTimer();
		}
		
		base.OnDragDropEnd();
	}
	
	protected override void OnDragDropMove (Vector2 delta)
	{
		base.OnDragDropMove(delta);
		if (mTrans.position.y < CardManager.Instance.cardActivationBounds.min.y)
			sprite.color = Color.red;
		else
			sprite.color = Color.white;
	}

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
