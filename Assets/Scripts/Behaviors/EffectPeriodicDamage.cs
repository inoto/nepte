using UnityEngine;

public class EffectPeriodicDamage: EffectPeriodic
{
	[Header("EffectPeriodicDamage")]
	public int Damage = 100;
	public ITargetable TargetUnit;

	protected override void Tick()
	{
		base.Tick();
		TargetUnit.Damage(Damage);
	}

	protected override void Reset()
	{
		Tick();
		Planet bas = TargetUnit.GameObj.GetComponent<Planet>();
		if (bas != null)
		{
			Destroy(bas.PropertyIcon);
			bas.PropertyIcon = null;
		}
		base.Reset();
	}
}