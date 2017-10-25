using UnityEngine;

public class InputTouch : AbstractCamera2DInputTouch
{
	[Header("InputTouch")]
	public Planet selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	protected override void onTouchStart ()
	{
		base.onTouchStart();
		
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

	protected override void onTouchEnded()
	{
		base.onTouchEnded();
		
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
				if (bas != null && bas != selectedMothershipOrbit.Planet && bas.Owner.playerNumber == 0)
				{
					selectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			selectedMothershipOrbit = null;
		}
	}
	
}
