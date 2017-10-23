using UnityEngine;

public class InputTouch : AbstractCamera2DInputTouch
{
	[Header("InputTouch")]
	public Base selectedBas;
	public MothershipOrbit selectedMothershipOrbit;

	protected override void onTouchStart ()
	{
		base.onTouchStart();
		
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

	protected override void onTouchEnded()
	{
		base.onTouchEnded();
		
		if (selectedBas != null)
		{
//			DestroyObject(selectedBas.lineArrow);
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
				if (bas != null && bas != selectedMothershipOrbit.bas && bas.owner.playerNumber == 0)
				{
					selectedMothershipOrbit.AssignToBase(bas);
				}
			}	
			selectedMothershipOrbit = null;
		}
	}
	
}
