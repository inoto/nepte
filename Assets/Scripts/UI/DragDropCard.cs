using UnityEngine;

public class DragDropCard : UIDragDropItem
{
	private UISprite sprite;
	private UIButton button;
	private Card card;
	private Card cardOriginal;

	protected override void Awake()
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
		if (card.InCooldown)
		{
			return;
		}
		else
		{
			base.StartDragging();
		}
	}

	protected override void OnDragDropStart()
	{
		CardManager.Instance.DeniedArea.enabled = true;
		base.OnDragDropStart();
		
		sprite.depth += 10;
		button.enabled = false;

		card.Drag();
	}

	protected override void OnDragDropEnd()
	{
		CardManager.Instance.DeniedArea.enabled = false;
		
		// check the card is on card activation area
		if (mTrans.position.y > CardManager.Instance.CardActivationBounds.min.y)
		{
			Vector2 pos = GetWorldCoordinate();
			if (card.Activate(pos))
			{
				cardOriginal.StartCooldownTimer();
			}
		}
		
		
		base.OnDragDropEnd();
	}
	
	protected override void OnDragDropMove (Vector2 delta)
	{
		base.OnDragDropMove(delta);
//		if (mTrans.position.y < CardManager.Instance.cardActivationBounds.min.y)
//			sprite.color = Color.red;
//		else
//			sprite.color = Color.white;
	}

//	protected override void OnDragDropRelease (GameObject surface)
//	{
//		//base.OnDragDropRelease(surface);
//	}

	private Vector2 GetWorldCoordinate()
	{
		Vector2 worldPos = UICamera.currentCamera.WorldToViewportPoint(sprite.worldCenter);
		worldPos = Camera.main.ViewportToWorldPoint(worldPos);
		return worldPos;
	}

//	private void OnDrawGizmos()
//	{
//		//Gizmos.DrawSphere(mTrans.position, 0.02f);
//		Gizmos.DrawWireSphere(sprite.worldCenter, 0.1f - 0.02f);
//	}
}
