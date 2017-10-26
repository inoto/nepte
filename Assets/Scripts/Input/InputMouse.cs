using UnityEngine;

public class InputMouse : AbstractCamera2DInputMouse
{
	[Header("InputMouse")]
	public Planet SelectedBas;
	public MothershipOrbit SelectedMothershipOrbit;

	protected override void OnMouseBtnDown()
	{
		base.OnMouseBtnDown();
		
		RaycastHit2D hit = TheCamera.Raycast2DScreen(Input.mousePosition);
		if (hit && SelectedBas == null)
		{
			Planet bas = hit.transform.gameObject.GetComponent<Planet>();
			MothershipOrbit mothershipOrbit = hit.transform.GetComponent<MothershipOrbit>();
			if (bas != null && bas.Owner.PlayerNumber == 0)
			{
				SelectedBas = bas;
				bas.MakeArrow();
				
			}
			else if (mothershipOrbit != null && mothershipOrbit.Mothership.Owner.PlayerNumber == 0)
			{
				SelectedMothershipOrbit = mothershipOrbit;
				mothershipOrbit.MakeArrow();
			}
		}
	}

	protected override void OnMouseBtnUp()
	{
		base.OnMouseBtnUp();
		
		if (SelectedBas != null)
		{
			DestroyObject(SelectedBas.LineRendererArrow);
			foreach (var b in GameManager.Instance.Planets)
			{
				b.GlowRemove();
			}
			RaycastHit2D hit = TheCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Planet bas = hit.transform.gameObject.GetComponent<Planet>();
				if (bas != null && bas != SelectedBas)
				{
					SelectedBas.Spawner.ReleaseUnits(bas.gameObject);
				}
			}
			SelectedBas = null;
			
		}
		if (SelectedMothershipOrbit != null)
		{
			DestroyObject(SelectedMothershipOrbit.LineRendererArrow);
			RaycastHit2D hit = TheCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Planet bas = hit.transform.GetComponent<Planet>();
				if (bas != null && bas != SelectedMothershipOrbit.Planet)// && bas.owner.playerNumber == 0)
				{
					SelectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			SelectedMothershipOrbit = null;
		}
		
	}
	
//	void OnDrawGizmos() {
//		if (selectedObject != null) {
//			Gizmos.color = Color.blue;
//			Gizmos.DrawLine(selectedObject.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
//		}
//	}
}
