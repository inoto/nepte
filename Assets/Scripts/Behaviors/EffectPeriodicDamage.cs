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
		Base bas = unit.GameObj.GetComponent<Base>();
		if (bas != null)
		{
			Destroy(bas.propertyIcon);
			bas.propertyIcon = null;
		}
		base.Reset();
	}
}