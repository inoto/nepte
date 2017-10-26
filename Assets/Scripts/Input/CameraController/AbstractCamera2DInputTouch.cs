using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractCamera2DInputTouch : AbstractCamera2DInput {

	protected Vector2[] LastTouchPosition;
	protected Vector2 InitialTouch = Vector2.zero;

	public void Awake ()
	{
		Attach ();
	}

	void Update()
	{
		if (Attached)
		{
			// Обновление точек касания при первом касании
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				OnTouchStart ();
			}

			// Одно касание - перетаскивание камеры или поиск столкновений
			if (Input.touchCount == 1)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					OnTouchEnded();
				}
				OnOneTouch ();
			}

			// Два касания - зум
			if (ZoomEnabled && Input.touchCount == 2)
			{
				OnTwinTouch ();
			}
			else
			{
				ZoomStarted = false;
			}
		}
	}

	protected virtual void OnTouchStart()
	{
		UpdateTouchPositions();
		InitialTouch = LastTouchPosition[0];
		IsInteractionStatic = true;
	}

	protected virtual void OnTouchEnded()
	{
		Touch touch = Input.GetTouch(0);
		Vector2 touchPosition = touch.position;
	}
	
	protected virtual void OnOneTouch()
	{
		Touch touch = Input.GetTouch (0);
		Vector2 touchPosition = touch.position;
	}
	
	protected virtual void OnTwinTouch ()
	{
		if (!ZoomStarted)
		{
			ZoomStarted = true;
			LastZoomCenter = TheCamera.Camera.ScreenToWorldPoint ((Input.GetTouch (0).position + Input.GetTouch (1).position) / 2f);
		}
		if (LastTouchPosition.Length > 1)
		{
			float deltaScale = (LastTouchPosition [0] - LastTouchPosition [1]).magnitude - (Input.GetTouch (0).position - Input.GetTouch (1).position).magnitude;
			TheCamera.Zoom (
				LastZoomCenter, 
				ZoomSpeed * deltaScale / TheCamera.Camera.ScreenToWorldPoint (Vector3.one).magnitude
			);
			IsInteractionStatic = false;
		}
		UpdateTouchPositions ();
	}

	private void UpdateTouchPositions ()
	{
		LastTouchPosition = (new List<Touch> (Input.touches)).FindAll (TouchInProgress).Select(t => t.position).ToArray ();
	}

	private static bool TouchInProgress (Touch touch)
	{
		return touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
	}
}
