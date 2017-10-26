using System.Collections;
using UnityEngine;

public class EffectPeriodic: Effect
{
	[Header("EffectPeriodic")]
	public float TickStep = 1f;
	public int TickCount = 5;
	
	private bool applied = false;

	protected override void Reset()
	{
		base.Reset();
		applied = false;
	}

	protected virtual void Tick()
	{
		//Debug.Log("tick");
	}

	public override void Activate(float newDuration)
	{
		base.Activate(newDuration);
		//duration = newDuration;
		StartCoroutine(StartPeriodic());
	}

	private IEnumerator StartPeriodic()
	{
		applied = true;
		while (applied)
		{
			yield return new WaitForSeconds(TickStep);
			Tick();
		}
	}
}