using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters;

[System.Serializable]
public class FollowRally
{
	public bool enabled = true;
	public bool followingRally = false;
	
    public bool arrived = false;

	public float angle;

    public Vector2 rallyPoint;
	public GameObject rally;

    //public float maxSpeed = 1;
    //public float maxAcceleration = 1;
    public float rallyPointMass = 10;
    public float rallyPointG = 0.3f;
    public float stopRadius = 1;
    public float slowDownRadius = 3;

	public float mag;

    float forceMultiplier = 1;
    float forceMultiplierOriginal = 0;

    [System.NonSerialized] private Mover mover;

	public void Activate(Mover _mover)
    {
        mover = _mover;
        //mover.owner.playerController.rallyPoint.OnRallyPointChanged += UpdateRallyPoint;
        //UpdateRallyPoint();
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

    public void UpdateRallyPoint(GameObject obj)
    {
        arrived = false;
//		if (forceMultiplierOriginal > 0)
//			forceMultiplier = forceMultiplierOriginal;
        //rallyPoint = mover.owner.playerController.rallyPoint.trans.position;
	    rally = obj;
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

	public void BeAround()
	{
		Vector2 desire = ((Vector2)mover.trans.position - (Vector2)rally.transform.position - mover.velocity).normalized;
		desire.Normalize();
		desire *= stopRadius;
		desire *= -1;
		angle = 1.5f;
		Vector2 force = new Vector2(stopRadius * (desire.x * Mathf.Cos(angle) - desire.y * Mathf.Sin(angle)),
			stopRadius * (desire.x * Mathf.Sin(angle) + desire.y * Mathf.Cos(angle)));
		force = Mover.LimitVector(force, mover.maxForce);
		mover.velocity /= 1.2f;
		mover.AddForce(force);
	}

	public void EndArrive(bool successfully)
	{
		arrived = true;
//		forceMultiplierOriginal = forceMultiplier;
//		forceMultiplier = 0.5f;
	}

    public void Arrive()
	{
		/* Get the right direction for the linear acceleration */
		Vector2 desired = rally.transform.position - mover.trans.position;

		/* Get the distance to the target */
		float dist = desired.magnitude;

        desired.Normalize();

        /* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
        //float targetSpeed;
        //float currentSpeed = 0;
        if (dist < slowDownRadius)
		{
			if (dist > stopRadius)
			{
				float dividedStopRadius = stopRadius / 2;
				//mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
				desired *= mover.maxSpeed * ((dist - dividedStopRadius) / (slowDownRadius - dividedStopRadius));
//				BeAround();
			}
			else
			{
				//mover.velocity = Vector2.zero;
				arrived = true;
				mover.velocity *= 0;
				

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
        force *= forceMultiplier;
		force = Mover.LimitVector(force, mover.maxForce);
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
