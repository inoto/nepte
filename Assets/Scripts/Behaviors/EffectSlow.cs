using System.Collections;
using UnityEngine;

public class EffectSlow: Effect
{
	[Header("EffectSlow")]
	public float factor;
	[HideInInspector] public Mover mover;

	protected override void Apply()
	{
		mover.maxSpeed -= factor/10;
	}

	protected override void Reset()
	{
		mover.maxSpeed += factor/10;
	}
}