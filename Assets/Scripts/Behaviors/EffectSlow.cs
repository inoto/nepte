using UnityEngine;

public class EffectSlow: Effect
{
	[Header("EffectSlow")]
	public float SlowFactor;
	[HideInInspector] public Mover Mover;

	protected override void Apply()
	{
		Mover.MaxSpeed -= SlowFactor/10;
	}

	protected override void Reset()
	{
		Mover.MaxSpeed += SlowFactor/10;
	}
}