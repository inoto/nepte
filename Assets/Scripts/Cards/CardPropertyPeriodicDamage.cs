using UnityEngine;

public class CardPropertyPeriodicDamage : CardProperty
{
	[Header("CardPropertyPeriodicDamage")]
	public float Duration = 6f;
	public float TickStep = 1f;
	public int Damage = 100;

	public override bool Activate(Vector2 position)
	{
		if (!base.Activate(position))
		{
			return false;
		}
		if (Planet != null)
		{
			var effect = Planet.gameObject.AddComponent<EffectPeriodicDamage>();
			effect.TargetUnit = Planet.GetComponent<ITargetable>();
			effect.TickStep = TickStep;
			effect.Damage = Damage;
			effect.Activate(Duration);
			return true;
		}
		return false;
	}
	
}
