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
		if (unit.GameObj.GetComponent<Base>().propertyIcon != null)
		{
			unit.GameObj.GetComponent<Base>().propertyIcon = null;
			//unit.GameObj.transform.DestroyChildren();
			Destroy(unit.GameObj.GetComponent<Base>().propertyIcon);
		}
		base.Reset();
	}
}