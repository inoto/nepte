using UnityEngine;
using System.Collections;

[System.Serializable]
public class FollowTarget
{
	public bool enabled = false;
	public bool followingTarget = false;
	
	public Weapon weapon;

	//public ITargetable target;

    //public float maxSpeed = 1;
    //public float maxAcceleration = 1;
    public float attackRadius = 3.5f;
    public float slowDownRadius = 4.5f;

    float forceMultiplier = 1;
    float forceMultiplierOriginal = 0;

    [System.NonSerialized] private Mover mover;

    public FollowTarget(Mover _mover)
    {
        mover = _mover;
	    weapon = mover.GetComponent<Weapon>();
    }

	//  private void Start()
	//  {
	//mover.drone.owner.playerController.rallyPoint.OnRallyPointChanged += UpdateRallyPoint;
	//UpdateRallyPoint();
	//}

	//private void Update()
	//{
	//    if (rallyPoint != null)
	//        Arrive();
	//}
    void Attract()
    {
		float mass = 10;
	    float G = 0.3f;
        Vector2 force = weapon.target.GameObj.transform.position - mover.trans.position;
        float dist = force.magnitude;
        dist = Mathf.Clamp(dist, 1, 3);
        force.Normalize();
        float strength = (G * mass * mover.mass) / (dist * dist);
        force *= strength;
        mover.AddForce(force);
    }

	public void Seek()
	{
		Vector2 desired = weapon.target.GameObj.transform.position - mover.trans.position;

		float dist = desired.magnitude;
		
		desired.Normalize();

		if (dist < slowDownRadius)
		{
			if (dist > attackRadius)
			{
				desired *= mover.maxSpeed * ((dist - attackRadius) / (slowDownRadius - attackRadius));
				if (weapon.isAttacking)
				{
					followingTarget = true;
					weapon.StopAttacking();
				}
			}
			else
			{
				if (weapon.target.IsDied)
				{
					weapon.EndCombat();
				}
				if (!weapon.isAttacking)
				{
					followingTarget = false;
					weapon.AttackTarget();
				}
				mover.velocity *= 0.5f;
				//mover.AddForce(friction);
				return;
			}
		}
		else
		{
			desired *= mover.maxSpeed;
		}

		Vector2 force = desired - mover.velocity;
		force = Mover.LimitVector(force, mover.maxForce);

		mover.AddForce(force);
	}
	
	public void LookAtTarget()
	{
		if (weapon.target != null)
		{
			Vector2 direction = weapon.target.GameObj.transform.position - mover.trans.position;

			direction.Normalize();

			// If we have a non-zero direction then look towards that direciton otherwise do nothing
			if (direction.sqrMagnitude > 0.01f)
			{
				float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
				float rotation = Mathf.LerpAngle(mover.trans.rotation.eulerAngles.z, toRotation, Time.deltaTime * mover.turnSpeed);

				mover.trans.rotation = Quaternion.Euler(0, 0, rotation);
			}
		}
	}

    public Vector2 BeAround()
	{
        Vector2 centerOfMass = new Vector2();
        //Vector2 force = rallyPoint - targetPosition;
        if (mover.IsFacing(weapon.target.GameObj.transform.position, Mathf.Cos(120f * Mathf.Deg2Rad)))
        {
            centerOfMass += (Vector2)weapon.target.GameObj.transform.position;
        }

		//mover.LookWhereYoureGoing();
        return mover.Arrive(centerOfMass);
	}

    public void Arrive()
	{
		/* Get the right direction for the linear acceleration */
		Vector2 desired = weapon.target.GameObj.transform.position - mover.trans.position;

		/* Get the distance to the target */
		float dist = desired.magnitude;

        desired.Normalize();

        /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
        //float targetSpeed;
        //float currentSpeed = 0;
//        if (dist < slowDownRadius)
//		{
//            if (dist > stopRadius)
//                //mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
//                desired *= mover.maxSpeed * ((dist - stopRadius) / (slowDownRadius - stopRadius));
//            else
//            {
//	            EndArrive(true);
//                return;
//            }
//		}
//		else
//		{
//			desired *= mover.maxSpeed;
//		}

		/* Give targetVelocity the correct speed */

		/* Calculate the linear acceleration we want */
		Vector2 force = desired - mover.velocity;
		/*
         Rather than accelerate the character to the correct speed in 1 second, 
         accelerate so we reach the desired speed in timeToTarget seconds 
         (if we were to actually accelerate for the full timeToTarget seconds).
        */
        // accelerate mover each frame by accelerationStep
  //      force *= 1 / mover.accelerationStep;

		///* Make sure we are accelerating at max acceleration */
		//if (force.magnitude > mover.maxAcceleration)
		//{
		//	force.Normalize();
		//	force *= mover.maxAcceleration;
		//}
        force *= 2f;
        force *= forceMultiplier;
        mover.AddForce(force);
	}
}
