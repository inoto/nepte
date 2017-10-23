using System.Collections;
using UnityEngine;

public class EffectPeriodicDamage: EffectPeriodic
{
	[Header("EffectPeriodicDamage")]
	public int damage = 100;
	public ITargetable unit;

	protected override void Tick()
	{
		base.Tick();
		unit.Damage(damage);
	}

	protected override void Reset()
	{
		Tick();
		Planet bas = unit.GameObj.GetComponent<Planet>();
		if (bas != null)
		{
			Destroy(bas.PropertyIcon);
			bas.PropertyIcon = null;
		}
		base.Reset();
	}
}