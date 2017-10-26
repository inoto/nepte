using UnityEngine;

[System.Serializable]
public class FollowBase
{
	public bool Enabled = true;
	public bool IsFollowing = false;
	
    public bool Arrived = false;

	public Planet TargetPlanet;

	public float EnterRadius = 1;
    public float AttackRadius = 2;
    private float slowDownRadius = 3;

    private float forceMultiplier = 1;
	private float forceMultiplierOriginal = 0;

    [System.NonSerialized] private Mover mover;

	public void Activate(Mover newMover)
    {
        mover = newMover;
	    Arrived = false;
    }

    public void UpdateTarget(Planet obj)
    {
        Arrived = false;
//		if (forceMultiplierOriginal > 0)
//			forceMultiplier = forceMultiplierOriginal;
	    TargetPlanet = obj;
    }

	public void Seek()
	{
		Vector2 desired = (Vector2)TargetPlanet.transform.position - (Vector2)mover.Trans.position;

		desired.Normalize();

		desired *= mover.MaxSpeed;

		Vector2 force = desired - mover.Velocity;
		force = Mover.LimitVector(force, mover.MaxForce);

		mover.AddForce(force);
	}

	public void MoveAround()
	{
		if (TargetPlanet.Owner.PlayerNumber == -1)
		{
			Arrived = false;
			return;
		}
		Vector2 desire = ((Vector2)mover.Trans.position - (Vector2)TargetPlanet.transform.position).normalized;
		desire.Normalize();
		desire *= AttackRadius;
		if (desire.magnitude > AttackRadius)
		{
			Arrived = false;
		}
		desire *= -1;
		float angle = 1.4f;
		Vector2 force = new Vector2(AttackRadius * (desire.x * Mathf.Cos(angle) - desire.y * Mathf.Sin(angle)),
			AttackRadius * (desire.x * Mathf.Sin(angle) + desire.y * Mathf.Cos(angle)));
		force = Mover.LimitVector(force, mover.MaxForce);
		mover.Velocity /= 1.2f;
		mover.AddForce(force);
	}

	public void EndArrive(bool successfully)
	{
		Arrived = true;
		IsFollowing = false;
		//		forceMultiplierOriginal = forceMultiplier;
		//		forceMultiplier = 0.5f;
	}

    public void Arrive()
	{
		/* Get the right direction for the linear acceleration */
		Vector2 desired = TargetPlanet.transform.position - mover.Trans.position;

		/* Get the distance to the target */
		float dist = desired.magnitude;

        desired.Normalize();

        /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
        //float targetSpeed;
        //float currentSpeed = 0;
        if (dist < slowDownRadius)
		{
			mover.Separation.Desired = 0.3f;
			mover.Cohesion.Desired = 0.3f;
			if (TargetPlanet.Owner.PlayerNumber == mover.Owner.PlayerNumber || TargetPlanet.Owner.PlayerNumber == -1)
			{
				if (dist > EnterRadius)
				{
					float dividedStopRadius = EnterRadius / 2;
					//mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
					desired *= mover.MaxSpeed * ((dist - dividedStopRadius) / (slowDownRadius - dividedStopRadius));
//					if (mover.trans.localScale.x > 0.6f)
//						mover.trans.localScale *= 0.9975f;
				}
				else
				{
					TargetPlanet.Spawner.PutDroneInside(mover.GetComponent<Drone>());
					EndArrive(true);
					//mover.velocity *= 0;
					return;
				}
			}
			else
			{
				if (dist > AttackRadius)
				{
					float dividedStopRadius = AttackRadius / 2;
					//mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
					desired *= mover.MaxSpeed * ((dist - dividedStopRadius) / (slowDownRadius - dividedStopRadius));
				}
				else
				{
					EndArrive(true);
					mover.Velocity *= 0;
					return;
				}
			}
		}
		else
		{
			desired *= mover.MaxSpeed;
		}

		/* Give targetVelocity the correct speed */

		/* Calculate the linear acceleration we want */
		Vector2 force = desired - mover.Velocity;
		/*
         Rather than accelerate the character to the correct speed in 1 second, 
         accelerate so we reach the desired speed in timeToTarget seconds 
         (if we were to actually accelerate for the full timeToTarget seconds).
        */
        // accelerate mover each frame by accelerationStep
  		//      force *= 1 / mover.accelerationStep;

		/* Make sure we are accelerating at max acceleration */
		//if (force.magnitude > mover.maxAcceleration)
		//{
		//	force.Normalize();
		//	force *= mover.maxAcceleration;
		//}
        force *= 2f;
        force *= forceMultiplier;
		force = Mover.LimitVector(force, mover.MaxForce);
        mover.AddForce(force);
	}

//	public void Giz()
//	{
//		if (rally != null)
//		{
//			Gizmos.color = Color.green;
//			Gizmos.DrawSphere(desire + (Vector2)rally.transform.position, 0.3f);
//			Gizmos.DrawLine(desire + (Vector2)rally.transform.position, mover.trans.position);
//			Gizmos.color = Color.blue;
//			Gizmos.DrawSphere(force + (Vector2)mover.trans.position, 0.3f);
//			Gizmos.DrawLine(force + (Vector2)mover.trans.position, mover.trans.position);
//		}
//	}
}
