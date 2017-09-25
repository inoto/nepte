using System.Collections;
using UnityEngine;

public class EffectPeriodic: Effect
{
	[Header("EffectPeriodic")]
	public float tickStep = 1f;
	public int tickCount = 5;
	
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

	IEnumerator StartPeriodic()
	{
		applied = true;
		while (applied)
		{
			yield return new WaitForSeconds(tickStep);
			Tick();
		}
	}
}