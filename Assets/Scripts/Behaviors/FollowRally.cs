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
    void Attract()
    {
		float mass = 10;
	    float G = 0.3f;
        Vector2 force = rally.transform.position - mover.trans.position;
        float dist = force.magnitude;
        dist = Mathf.Clamp(dist, 1, 3);
        force.Normalize();
        float strength = (G * mass * mover.mass) / (dist * dist);
        force *= strength;
        mover.AddForce(force);
    }


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

	public void BeAround2()
	{

		Vector2 desire = rally.transform.position - mover.trans.position;
		desire.Normalize();
		
		float angle = 1.5f;
		Vector2 force = new Vector2(stopRadius * (desire.x * Mathf.Cos(angle) - desire.y * Mathf.Sin(angle)), stopRadius * (desire.x * Mathf.Sin(angle) + desire.y * Mathf.Cos(angle)));
		//force *= -1;
		force = Mover.LimitVector(force, mover.maxForce);
		
//		mover.velocity += force * Time.deltaTime;
		mover.AddForce(force);
	}
	
	public void BeAround()
	{
		angle += Time.deltaTime / mover.turnSpeed; // меняется плавно значение угла

		var x = Mathf.Cos (angle) * stopRadius;
		var y = Mathf.Sin (angle) * stopRadius;
		mover.trans.position = new Vector2(x, y) + (Vector2)rally.transform.position;
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
                //mover.currentSpeed = mover.maxSpeed * ((dist-stopRadius) / (slowDownRadius-stopRadius));
                desired *= mover.maxSpeed * ((dist - stopRadius) / (slowDownRadius - stopRadius));
            else
            {
	            //mover.velocity = Vector2.zero;
	            arrived = true;
	            mag = (rally.transform.position - mover.trans.position).magnitude;
	            //BeAround();
	            
//	            EndArrive(true);
	            //BeAround();
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
}
