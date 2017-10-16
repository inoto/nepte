using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputTouch : CameraControlTouch
{
	private float dragTreshold = 10f;
	public Base selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	void Awake () {
		Attach ();
	}

	void Update()
	{
		if (attached) {
			// Обновление точек касания при первом касании
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
				onTouchStart ();
			}

			// Одно касание - перетаскивание камеры или поиск столкновений
			if (Input.touchCount == 1)
			{
//				if (Input.GetTouch(0).phase == TouchPhase.Ended)
//				{
//					Debug.Log("touch with 1 ended");
//					onTouchEnded();
//				}
				onOneTouch ();
			}

			// Два касания - зум
			if (ZoomEnabled && Input.touchCount == 2)
			{
				onTwinTouch ();
			} else {
				zoomStarted = false;
			}
		}
	}

	void onTouchStart () {
		updateTouchPositions ();
		initialTouch = lastTouchPosition [0];
		isInteractionStatic = true;
		
		RaycastHit2D hit = ActiveCamera.Raycast2DScreen(Input.mousePosition);
		if (hit && selectedBas == null)
		{
			Base bas = hit.transform.gameObject.GetComponent<Base>();
			MothershipOrbit mothershipOrbit = hit.transform.GetComponent<MothershipOrbit>();
			if (bas != null && bas.owner.playerNumber == 0)
			{
				selectedBas = bas;
				bas.MakeArrow();
			}
			else if (mothershipOrbit != null && mothershipOrbit.mothership.owner.playerNumber == 0)
			{
				selectedMothershipOrbit = mothershipOrbit;
				mothershipOrbit.MakeArrow();
			}
		}
	}

	void onTouchEnded()
	{
		Touch touch = Input.GetTouch (0);
		Vector2 touchPosition = touch.position;
		if (selectedBas != null)
		{
			DestroyObject(selectedBas.lineArrow);
			foreach (var b in GameController.Instance.bases)
				b.GlowRemove();
			RaycastHit2D hit = ActiveCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Base bas = hit.transform.gameObject.GetComponent<Base>();
				if (bas != null && bas != selectedBas)
				{
					selectedBas.spawner.ReleaseUnits(bas.gameObject);
				}
			}
			selectedBas = null;
			
		}
		if (selectedMothershipOrbit != null)
		{
			DestroyObject(selectedMothershipOrbit.lineArrow);
			RaycastHit2D hit = ActiveCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Base bas = hit.transform.GetComponent<Base>();
				if (bas != null && bas != selectedMothershipOrbit.bas && bas.owner.playerNumber == 0)
				{
					selectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			selectedMothershipOrbit = null;
		}
	}

	void onOneTouch () {
		Touch touch = Input.GetTouch (0);
		Vector2 touchPosition = touch.position;

		if (DragEnabled)
		{
			// Длинный жест пальцем - перетаскивание
			if (touch.phase == TouchPhase.Moved && (touchPosition - initialTouch).magnitude > dragTreshold)
			{
				ActiveCamera.TranslateScreen(lastTouchPosition[0], touchPosition);
				lastTouchPosition[0] = touchPosition;
				isInteractionStatic = false;
			}
		}

		// Короткое прикосновение - поиск места тыка
		if (touch.phase == TouchPhase.Ended && isInteractionStatic)
		{
			if (isInteractionStatic)
			{
				Ray ray = Camera.main.ScreenPointToRay(touchPosition);
				RaiseClickTap(ray.origin + (ray.direction));
				//RaycastHit2D hit = ActiveCamera.Raycast2DScreen (touchPosition);
				//if (hit) {
				//	// TODO: Не факт, что костыль производительнее OnMouseDown
				//             hit.transform.gameObject.SendMessage ("Click", hit.point, SendMessageOptions.DontRequireReceiver);
				//}
			}
			onTouchEnded();
		}
	}

	void onTwinTouch () {
		if (!zoomStarted) {
			zoomStarted = true;
			lastZoomCenter = ActiveCamera.Camera.ScreenToWorldPoint ((Input.GetTouch (0).position + Input.GetTouch (1).position) / 2f);
		}
		if (lastTouchPosition.Length > 1) {
			float deltaScale = (lastTouchPosition [0] - lastTouchPosition [1]).magnitude - (Input.GetTouch (0).position - Input.GetTouch (1).position).magnitude;
			ActiveCamera.Zoom (
				lastZoomCenter, 
				ZoomSpeed * deltaScale / ActiveCamera.Camera.ScreenToWorldPoint (Vector3.one).magnitude
			);
			isInteractionStatic = false;
		}
		updateTouchPositions ();
	}

	void updateTouchPositions () {
		lastTouchPosition = (new List<Touch> (Input.touches)).FindAll (touchInProgress).Select(t => t.position).ToArray ();
	}

	private static bool touchInProgress (Touch touch) {
		return touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
	}
}
