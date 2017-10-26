using UnityEngine;

[System.Serializable]
public class CollisionCircle
{
	[SerializeField] private bool showGizmos;

	public readonly int InstanceId;

    public readonly Transform Trans;
    public readonly Mover Mover;
    public readonly Owner Owner;
	public readonly Weapon Weapon;
	private readonly Planet Planet;

	
	public bool IsInQt = false;
	public bool IsDead = false;
    public bool IsCollidedWithBase = false;
	public readonly bool IsStatic;
	public readonly bool IsWeapon;
	public CollisionCircle CollidedBaseCircle = null;
	public int CollidedCount = 0;
	

    public CollisionCircle(Transform newTrans, Mover newMover, Owner newOwner, Weapon newWeapon)
    {
	    showGizmos = true;
        Trans = newTrans;
	    Mover = newMover;
	    if (Mover == null)
	    {
		    IsStatic = true;
		    Planet = Trans.GetComponent<Planet>();
	    }
	    Owner = newOwner;
	    Weapon = newWeapon;
	    if (Weapon != null)
	    {
		    IsWeapon = true;
	    }
	    IsCollidedWithBase = false;
	    CollidedBaseCircle = null;
	    CollidedCount = 0;
	    InstanceId = Trans.gameObject.GetInstanceID();
    }

	public float GetRadius()
	{
		if (IsWeapon)
		{
			return Weapon.Radius;
		}
		else if (IsStatic)
		{
			return Planet != null ? Planet.Collider.radius : 0;
		}
		else
		{
			return 0;
		}
	}

	public void Collided(CollisionCircle other)
	{
		if (InstanceId == other.InstanceId)
		{
			return;
		}
		if (Owner.PlayerNumber == other.Owner.PlayerNumber)
		{
			return;
		}
		if (IsDead)
		{
			return;
		}
		if (!IsWeapon)
		{
			return;
		}
		if (IsWeapon && !Mover.Weapon.HasTarget)
		{
			Mover.Weapon.Target = other.Trans.GetComponent<ITargetable>();
			Mover.Weapon.HasTarget = true;
		}
		else
		{
			Mover.Weapon.AttackTarget();
		}
	}

	public void CollidedEnded(CollisionCircle other)
	{
		if (InstanceId == other.InstanceId)
		{
			return;
		}
		if (Owner.PlayerNumber == other.Owner.PlayerNumber)
		{
			return;
		}
		if (IsDead)
		{
			return;
		}
		if (!IsWeapon)
		{
			return;
		}
		if (Mover.Weapon.Target != null)
		{
			if (Mover.Weapon.Target.GameObj == other.Trans.gameObject)
			{
				Mover.Weapon.Target = null;
			}
		}
	}

	public void DrawGizmos()
	{
		if (showGizmos)
		{
			if (IsInQt)
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawSphere(Trans.position, 0.1f);
		}
	}
}
