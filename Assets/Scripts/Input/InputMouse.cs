using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMouse : AbstractCamera2DInputMouse
{
	[Header("InputMouse")]
	public Planet selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	protected override void onMouseBtnDown()
	{
		base.onMouseBtnDown();
		RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
		if (hit && selectedBas == null)
		{
			Planet bas = hit.transform.gameObject.GetComponent<Planet>();
			MothershipOrbit mothershipOrbit = hit.transform.GetComponent<MothershipOrbit>();
			if (bas != null && bas.Owner.playerNumber == 0)
			{
				selectedBas = bas;
				bas.MakeArrow();
				
			}
			else if (mothershipOrbit != null && mothershipOrbit.Mothership.Owner.playerNumber == 0)
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
			DestroyObject(selectedBas.LineRendererArrow);
			foreach (var b in GameManager.Instance.Planets)
				b.GlowRemove();
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Planet bas = hit.transform.gameObject.GetComponent<Planet>();
				if (bas != null && bas != selectedBas)
				{
					selectedBas.Spawner.ReleaseUnits(bas.gameObject);
				}
			}
			selectedBas = null;
			
		}
		if (selectedMothershipOrbit != null)
		{
			DestroyObject(selectedMothershipOrbit.LineRendererArrow);
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Planet bas = hit.transform.GetComponent<Planet>();
				if (bas != null && bas != selectedMothershipOrbit.Planet)// && bas.owner.playerNumber == 0)
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
