using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Mover : MonoBehaviour
{
	public bool isMoving = false;

    [Header("Main")]
    public Vector2 velocity;
    public Vector2 acceleration;

    [Header("Current")]
    public float currentSpeed = 0;
    public float currentAcceleration = 0;

    [Header("Control")]
    public float mass = 10;
    public float maxForce = 1;
    public float maxSpeed = 1;
    public float maxAcceleration = 1;
    public float accelerationStep = 0.05f;
    public float targetRadius = 1;
    public float slowDownRadius = 3;
    public float timeToTarget = 0.1f;
    public float turnSpeed = 5;

	public bool smoothing = true;
	public int numSamplesForSmoothing = 5;
	private Queue<Vector2> velocitySamples = new Queue<Vector2>();

    [Header("Modules")]
    public FollowBase followBase;
    public Separation separation;
    public Cohesion cohesion;

    [Header("Components")]
    public Transform trans;
    public Owner owner;
    public Body body;
    public Radar radar;
    public Weapon weapon;

    private void Awake()
    {
	    owner = GetComponent<Owner>();
        trans = GetComponent<Transform>();
        body = GetComponent<Body>();
        radar = GetComponent<Radar>();
        weapon = GetComponent<Weapon>();
    }

	public void DelayedStart()
    {
        owner = GetComponent<Owner>();
	    followBase.Activate(this);
		separation.Activate(this);
		cohesion.Activate(this);
	    velocity *= 0;
	    acceleration *= 0;
	    StartCoroutine(Move());
    }

	public void Stop()
	{
		isMoving = false;
	}

	IEnumerator Move()
	{
		isMoving = true;
		while (isMoving)
		{
			if (followBase.enabled && followBase.targetBase != null)
			{
				if (!followBase.arrived)
				{
					followBase.Arrive();
				}
				else
				{
					followBase.MoveAround();
				}
				LookWhereYoureGoing();
			}
			if (separation.enabled)
			{
				separation.Separate();
			}
			if (cohesion.enabled)
			{
				cohesion.Cohesie();
			}
			velocity += acceleration;
			velocity *= maxSpeed;
			//velocity = LimitVector(velocity, 5);
			trans.position += (Vector3) velocity * Time.deltaTime * 3;
			acceleration *= 0;
			if (velocity.magnitude > 0)
				velocity /= 1.05f;
			yield return new WaitForSeconds(0.01f);
		}
	}

	public void CombatMode()
	{
		
	}

	/* Updates the velocity of the current game object by the given linear acceleration */
	public void AddForce(Vector2 _force)
	{
        //Vector2 f = _force/mass;
        //acceleration += f;
		acceleration += _force * Time.deltaTime;
	}
	
	public void AddPureForce(Vector2 _force)
	{
		//Vector2 f = _force/mass;
		//acceleration += f;

		trans.position += (Vector3) _force * Time.deltaTime;
	}

	/* Returns the steering for a character so it arrives at the target */
    public Vector2 Arrive(Vector2 targetPosition)
	{
		/* Get the right direction for the linear acceleration */
        Vector2 direction = targetPosition - (Vector2)trans.position;

		/* Get the distance to the target */
		float dist = direction.magnitude;

		/* If we are within the stopping radius then stop */
		//if (dist < targetRadius)
		//{
		//	velocity = Vector2.zero;
		//	return Vector2.zero;
		//}

		/* Calculate the target speed, full speed at slowRadius distance and 0 speed at 0 distance */
		//float targetSpeed;
		if (dist > slowDownRadius)
		{
            currentSpeed = maxSpeed;
		}
		else
		{
			currentSpeed = maxSpeed * (dist / slowDownRadius);
		}

		/* Give targetVelocity the correct speed */
		direction.Normalize();
		direction *= currentSpeed;

		/* Calculate the linear acceleration we want */
        Vector3 force = direction - velocity;
		/*
         Rather than accelerate the character to the correct speed in 1 second, 
         accelerate so we reach the desired speed in timeToTarget seconds 
         (if we were to actually accelerate for the full timeToTarget seconds).
        */
		force *= 1 / timeToTarget;

		/* Make sure we are accelerating at max acceleration */
		if (force.magnitude > maxAcceleration)
		{
			force.Normalize();
			force *= maxAcceleration;
		}

		return force;
	}

	/* A seek steering behavior. Will return the steering for the current game object to seek a given position */
	public void Seek(Vector2 targetPosition)
	{
        Vector2 desired = targetPosition - (Vector2)trans.position;

		desired.Normalize();

		desired *= maxSpeed;

        Vector2 force = desired - velocity;
        force = LimitVector(force, maxForce);

        AddForce(force);
	}

    public static Vector2 LimitVector(Vector2 value, float limit)
    {
        if (value.x > (0 + limit))
            value.x = limit;
	    if (value.x < (0 - limit))
		    value.x = -limit;
		if (value.y > (0 + limit))
			value.y = limit;
	    if (value.y < (0 - limit))
		    value.y = -limit;
        return value;
    }

	//public Vector3 Seek(Vector3 targetPosition)
	//{
	//	return Seek(targetPosition, maxAcceleration);
	//}

	/* Makes the current game object look where he is going */
	public void LookWhereYoureGoing()
	{
		Vector2 direction = velocity+acceleration;

		if (smoothing)
		{
			if (velocitySamples.Count == numSamplesForSmoothing)
			{
				velocitySamples.Dequeue();
			}

			velocitySamples.Enqueue(velocity+acceleration);

			direction = Vector2.zero;

			foreach (Vector2 v in velocitySamples)
			{
				direction += v;
			}

			direction /= velocitySamples.Count;
		}

		LookAtDirection(direction);
	}

	public void LookAtDirection(Vector2 direction)
	{
		direction.Normalize();

		// If we have a non-zero direction then look towards that direciton otherwise do nothing
		if (direction.sqrMagnitude > 0.01f)
		{
			float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)-90;
			float rotation = Mathf.LerpAngle(trans.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

            trans.rotation = Quaternion.Euler(0, 0, rotation);
		}
	}

	public void LookAtDirection(Quaternion toRotation)
	{
		LookAtDirection(toRotation.eulerAngles.z);
	}

	public void LookAtDirection(float toRotation)
	{
		float rotation = Mathf.LerpAngle(trans.rotation.eulerAngles.z, toRotation, Time.deltaTime * turnSpeed);

		trans.rotation = Quaternion.Euler(0, 0, rotation);
	}

	

	public Vector2 Interpose(Rigidbody target1, Rigidbody target2)
	{
		Vector2 midPoint = (target1.position + target2.position) / 2;

        float timeToReachMidPoint = Vector2.Distance(midPoint, trans.position) / maxSpeed;

		Vector2 futureTarget1Pos = target1.position + target1.velocity * timeToReachMidPoint;
		Vector2 futureTarget2Pos = target2.position + target2.velocity * timeToReachMidPoint;

		midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

        return Vector2.zero;//Arrive(midPoint);
	}

	/* Checks to see if the target is in front of the character */
	public bool IsInFront(Vector2 target)
	{
		return IsFacing(target, 0);
	}

	public bool IsFacing(Vector2 target, float cosineValue)
	{
        Vector2 facing = trans.right.normalized;

        Vector2 directionToTarget = (target - (Vector2)trans.position);
		directionToTarget.Normalize();

		return Vector2.Dot(facing, directionToTarget) >= cosineValue;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		//Gizmos.DrawSphere(velocity + (Vector2)trans.position, 0.1f);
		//Gizmos.DrawLine(velocity + (Vector2)trans.position, trans.position);
//		if (followRally.enabled)
//			followRally.Giz();
	}
}
