using UnityEngine;

public abstract class AbstractCamera2DInputMouse : AbstractCamera2DInput
{
	protected Vector2 LastClickPosition = Vector2.zero;
	protected Vector2 InitialClick = Vector2.zero;
	protected float LastClickDeltaTime = 0;

	private void Awake()
	{
		Attach();
	}

#if UNITY_EDITOR
	protected virtual void Update()
	{
		if (Attached)
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnMouseBtnDown();
			}
			if (Input.GetMouseButton(0))
			{
				OnMouseBtnHold();
			}
			if (Input.GetMouseButtonUp(0))
			{
				OnMouseBtnUp();
			}
			if (ZoomEnabled && Input.mouseScrollDelta.y != 0)
			{
				OnMouseScroll();
			}
		}
	}
#endif

	protected virtual void OnMouseBtnDown()
	{
		InitialClick = LastClickPosition = Input.mousePosition;
		if (DoubleClickEnabled)
		{
			if ((Time.time - LastClickDeltaTime) < DoubleClickCatchTime)
			{
				RaiseDoubleClickTap(InitialClick);
			}
			else
			{
				IsInteractionStatic = true;
			}
			LastClickDeltaTime = Time.time;
		}
		else
		{
			IsInteractionStatic = true;
		}
	}

	protected virtual void OnMouseBtnUp()
	{
		LastClickPosition = Vector2.zero;
		DragStarted = false;
		
		if (IsInteractionStatic && ClickEnabled)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaiseClickTap(ray.origin + (ray.direction));

//			RaycastHit2D hit = ActiveCamera.Raycast2DScreen (Input.mousePosition);
//			if (hit)
//			{
//				hit.transform.gameObject.GetComponent<Base>().isSelected = true;
//			}
		}
	}

	protected virtual void OnMouseBtnHold()
	{
		LastClickPosition = Input.mousePosition;
		
		Vector2 clickPosition = Input.mousePosition;

		// Длинный жест пальцем - перетаскивание
		if ((clickPosition - InitialClick).magnitude > DragTreshold)
		{
			if (DragScreenEnabled)
			{
				TheCamera.TranslateScreen(LastClickPosition, clickPosition);
			}
			IsInteractionStatic = false;
		}
	}
	
	protected virtual void OnMouseScroll()
	{
		TheCamera.ZoomScreen(Input.mousePosition, ZoomSpeed * Input.mouseScrollDelta.y);
	}

}
