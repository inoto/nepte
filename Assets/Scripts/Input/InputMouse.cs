using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : CameraControlMouse
{
	private float dragTreshold = 10f;
	public Base selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	void Awake () {
		Attach ();
	}

	#if UNITY_EDITOR
	void Update () {
		if (attached) {
			if (Input.GetMouseButtonDown (0)) {
				onMouseBtnDown ();
			}
			if (Input.GetMouseButton (0)) {
				onMouseHold ();
				onMouseBtnHold ();
			}
			if (Input.GetMouseButtonUp (0)) {
				onMouseBtnUp ();
				onMouseClick ();
			}
			if (ZoomEnabled && Input.mouseScrollDelta.y != 0) {
				onMouseScroll ();
			}
		}
	}
	#endif

	void onMouseHold () {
		Vector2 clickPosition = Input.mousePosition;

		// Длинный жест пальцем - перетаскивание
		if ((clickPosition - initialClick).magnitude > dragTreshold) {
			//ActiveCamera.TranslateScreen(lastClickPosition, clickPosition);
            isInteractionStatic = false;
		}
	}

	protected override void onMouseBtnDown()
	{
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

	void onMouseClick () {
		if (isInteractionStatic)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaiseClickTap(ray.origin + (ray.direction));

//			RaycastHit2D hit = ActiveCamera.Raycast2DScreen (Input.mousePosition);
//			if (hit)
//			{
//				hit.transform.gameObject.GetComponent<Base>().isSelected = true;
//			}
		}
		
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

	void onMouseScroll () {
		ActiveCamera.ZoomScreen (Input.mousePosition, ZoomSpeed*Input.mouseScrollDelta.y);
	}
	
//	void OnDrawGizmos() {
//		if (selectedObject != null) {
//			Gizmos.color = Color.blue;
//			Gizmos.DrawLine(selectedObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
//		}
//	}
}
