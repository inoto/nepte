using UnityEngine;

public abstract class AbstractCamera2DInputMouse : AbstractCamera2DInput
{
	protected Vector2 lastClickPosition = Vector2.zero;
	protected Vector2 initialClick = Vector2.zero;
	protected float lastClickDeltaTime = 0;
	
	void Awake()
	{
		Attach();
	}

#if UNITY_EDITOR
	protected virtual void Update()
	{
		if (attached)
		{
			if (Input.GetMouseButtonDown(0))
			{
				onMouseBtnDown();
			}
			if (Input.GetMouseButton(0))
			{
				onMouseBtnHold();
			}
			if (Input.GetMouseButtonUp(0))
			{
				onMouseBtnUp();
			}
			if (ZoomEnabled && Input.mouseScrollDelta.y != 0)
			{
				onMouseScroll();
			}
		}
	}
#endif

	protected virtual void onMouseBtnDown()
	{
		initialClick = lastClickPosition = Input.mousePosition;
		if (DoubleClickEnabled)
		{
			if ((Time.time - lastClickDeltaTime) < doubleClickCatchTime)
			{
				RaiseDoubleClickTap(initialClick);
			}
			else
			{
				isInteractionStatic = true;
			}
			lastClickDeltaTime = Time.time;
		}
		else
		{
			isInteractionStatic = true;
		}
	}

	protected virtual void onMouseBtnUp()
	{
		lastClickPosition = Vector2.zero;
		dragStarted = false;
		
		if (isInteractionStatic && ClickEnabled)
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

	protected virtual void onMouseBtnHold()
	{
		lastClickPosition = Input.mousePosition;
		
		Vector2 clickPosition = Input.mousePosition;

		// Длинный жест пальцем - перетаскивание
		if ((clickPosition - initialClick).magnitude > dragTreshold)
		{
			if (DragScreenEnabled)
				theCamera.TranslateScreen(lastClickPosition, clickPosition);
			isInteractionStatic = false;
		}
	}
	
	protected virtual void onMouseScroll()
	{
		theCamera.ZoomScreen(Input.mousePosition, ZoomSpeed * Input.mouseScrollDelta.y);
	}

}
