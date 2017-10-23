using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractCamera2DInputTouch : AbstractCamera2DInput {

	protected Vector2[] lastTouchPosition;
	protected Vector2 initialTouch = Vector2.zero;
	
	void Awake ()
	{
		Attach ();
	}

	void Update()
	{
		if (attached)
		{
			// Обновление точек касания при первом касании
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				onTouchStart ();
			}

			// Одно касание - перетаскивание камеры или поиск столкновений
			if (Input.touchCount == 1)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					onTouchEnded();
				}
				onOneTouch ();
			}

			// Два касания - зум
			if (ZoomEnabled && Input.touchCount == 2)
			{
				onTwinTouch ();
			}
			else
			{
				zoomStarted = false;
			}
		}
	}

	protected virtual void onTouchStart()
	{
		updateTouchPositions();
		initialTouch = lastTouchPosition[0];
		isInteractionStatic = true;
	}

	protected virtual void onTouchEnded()
	{
		Touch touch = Input.GetTouch(0);
		Vector2 touchPosition = touch.position;
	}
	
	protected virtual void onOneTouch()
	{
		Touch touch = Input.GetTouch (0);
		Vector2 touchPosition = touch.position;
	}
	
	protected virtual void onTwinTouch ()
	{
		if (!zoomStarted)
		{
			zoomStarted = true;
			lastZoomCenter = theCamera.Camera.ScreenToWorldPoint ((Input.GetTouch (0).position + Input.GetTouch (1).position) / 2f);
		}
		if (lastTouchPosition.Length > 1)
		{
			float deltaScale = (lastTouchPosition [0] - lastTouchPosition [1]).magnitude - (Input.GetTouch (0).position - Input.GetTouch (1).position).magnitude;
			theCamera.Zoom (
				lastZoomCenter, 
				ZoomSpeed * deltaScale / theCamera.Camera.ScreenToWorldPoint (Vector3.one).magnitude
			);
			isInteractionStatic = false;
		}
		updateTouchPositions ();
	}

	void updateTouchPositions ()
	{
		lastTouchPosition = (new List<Touch> (Input.touches)).FindAll (touchInProgress).Select(t => t.position).ToArray ();
	}

	private static bool touchInProgress (Touch touch)
	{
		return touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
	}
}
