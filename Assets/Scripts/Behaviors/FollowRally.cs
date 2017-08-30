using UnityEngine;
using System.Collections;

[System.Serializable]
public class FollowRally
{
    public bool arrived = false;

    Vector2 rallyPoint;

    //public float maxSpeed = 1;
    //public float maxAcceleration = 1;
    public float rallyPointMass = 10;
    public float rallyPointG = 0.3f;
    public float stopRadius = 1;
    public float slowDownRadius = 3;

    [System.NonSerialized] private Mover mover;

    public FollowRally(Mover _mover)
    {
        mover = _mover;
        mover.owner.playerController.rallyPoint.OnRallyPointChanged += UpdateRallyPoint;
        UpdateRallyPoint();
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
        Vector2 force = rallyPoint - (Vector2)mover.trans.position;
        float dist = force.magnitude;
        dist = Mathf.Clamp(dist, 1, 3);
        force.Normalize();
        float strength = (G * mass * mover.mass) / (dist * dist);
        force *= strength;
        mover.AddForce(force);
    }


    void UpdateRallyPoint()
    {
        arrived = false;
        rallyPoint = mover.owner.playerController.rallyPoint.trans.position;
    }

	public void Seek()
	{
		Vector2 desired = rallyPoint - (Vector2)mover.trans.position;

		desired.Normalize();

		desired *= mover.maxSpeed;

		Vector2 force = desired - mover.velocity;
		force = Mover.LimitVector(force, mover.maxForce);

		mover.AddForce(force);
	}

    public Vector2 BeAround()
	{
        Vector2 centerOfMass = new Vector2();
        //Vector2 force = rallyPoint - targetPosition;
        if (mover.IsFacing(rallyPoint, Mathf.Cos(120f * Mathf.Deg2Rad)))
        {
            centerOfMass += rallyPoint;
        }

		//mover.LookWhereYoureGoing();
        return mover.Arrive(centerOfMass);
	}

    public void Arrive()
	{
		/* Get the right direction for the linear acceleration */
		Vector2 desired = rallyPoint - (Vector2)mover.trans.position;

		/* Get the distance to the target */
		float dist = desired.magnitude;

        desired.Normalize();

        /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
        //float targetSpeed;
        //float currentSpeed = 0;
        if (dist < slowDownRadius)
		{
            if (dist > stopRadius)
                //mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
                desired *= mover.maxSpeed * ((dist - stopRadius) / (slowDownRadius - stopRadius));
            else
            {
                arrived = true;
                Attract();
                return;
            }
		}
		else
		{
			desired *= mover.maxSpeed;
		}

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
        mover.AddForce(force);
	}
}
