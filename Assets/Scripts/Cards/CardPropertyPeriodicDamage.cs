using System.Collections;
using UnityEngine;

public class CardPropertyPeriodicDamage : CardProperty
{
	[Header("CardPropertyPeriodicDamage")]
	public float duration = 6f;
	public float tickStep = 1f;
	public int damage = 100;

	public override bool Activate(Vector2 position)
	{
		if (!base.Activate(position))
			return false;
		if (bas != null)
		{
			var effect = bas.gameObject.AddComponent<EffectPeriodicDamage>();
			effect.unit = bas.GetComponent<ITargetable>();
			effect.tickStep = tickStep;
			effect.damage = damage;
			effect.forProperty = true;
			effect.Activate(duration);
			return true;
		}
		return false;
	}
	
}
