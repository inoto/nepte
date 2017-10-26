using UnityEngine;

public class InputTouch : AbstractCamera2DInputTouch
{
	[Header("InputTouch")]
	public Planet SelectedBas;
	public MothershipOrbit SelectedMothershipOrbit;

	protected override void onTouchStart ()
	{
		base.onTouchStart();
		
		RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
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

	protected override void onTouchEnded()
	{
		base.onTouchEnded();
		
		if (SelectedBas != null)
		{
			DestroyObject(SelectedBas.LineRendererArrow);
			foreach (var b in GameManager.Instance.Planets)
			{
				b.GlowRemove();
			}
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
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
			RaycastHit2D hit = theCamera.Raycast2DScreen(Input.mousePosition);
			if (hit)
			{
				Planet bas = hit.transform.GetComponent<Planet>();
				if (bas != null && bas != SelectedMothershipOrbit.Planet && bas.Owner.PlayerNumber == 0)
				{
					SelectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			SelectedMothershipOrbit = null;
		}
	}
	
}
