using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : AbstractCamera2DInputMouse
{
	[Header("InputMouse")]
	public Base selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	protected override void onMouseBtnDown()
	{
		base.onMouseBtnDown();
		RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
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

	protected override void onMouseBtnUp()
	{
		base.onMouseBtnUp();
		
		if (selectedBas != null)
		{
			DestroyObject(selectedBas.lineArrow);
			foreach (var b in GameController.Instance.bases)
				b.GlowRemove();
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
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
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Base bas = hit.transform.GetComponent<Base>();
				if (bas != null && bas != selectedMothershipOrbit.bas)// && bas.owner.playerNumber == 0)
				{
					selectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			selectedMothershipOrbit = null;
		}
		
	}
	
//	void OnDrawGizmos() {
//		if (selectedObject != null) {
//			Gizmos.color = Color.blue;
//			Gizmos.DrawLine(selectedObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
//		}
//	}
}
