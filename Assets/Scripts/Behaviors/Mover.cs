using System.Collections;
using UnityEngine;

public class Mover : MonoBehaviour
{
	public bool IsMoving = false;

    [Header("Main")]
    public Vector2 Velocity;
    public Vector2 Acceleration;

    [Header("Current")]
    public float CurrentSpeed = 0;
    public float CurrentAcceleration = 0;

    [Header("Control")]
    public float MaxForce = 1;
    public float MaxSpeed = 1;
    public float MaxAcceleration = 1;
    public float AccelerationStep = 0.05f;
    public float SlowDownRadius = 3;
    public float TimeToTarget = 0.1f;
    public float TurnSpeed = 5;

//	public bool smoothing = true;
//	public int numSamplesForSmoothing = 5;
//	private Queue<Vector2> velocitySamples = new Queue<Vector2>();

    [Header("Modules")]
    public FollowBase FollowBase;
    public Separation Separation;
    public Cohesion Cohesion;

    [Header("Components")]
    public Transform Trans;
    public Owner Owner;
    public Body Body;
    public Weapon Weapon;

    private void Awake()
    {
	    Owner = GetComponent<Owner>();
        Trans = GetComponent<Transform>();
        Body = GetComponent<Body>();
        Weapon = GetComponent<Weapon>();
    }

	public void DelayedStart()
    {
        Owner = GetComponent<Owner>();
	    FollowBase.Activate(this);
		Separation.Activate(this);
		Cohesion.Activate(this);
	    Velocity *= 0;
	    Acceleration *= 0;
	    StartCoroutine(Move());
    }

	public void Stop()
	{
		IsMoving = false;
	}

	private IEnumerator Move()
	{
		IsMoving = true;
		while (IsMoving)
		{
			if (FollowBase.Enabled && FollowBase.TargetPlanet != null)
			{
				if (!FollowBase.Arrived)
				{
					FollowBase.Arrive();
				}
				else
				{
					FollowBase.MoveAround();
				}
				LookWhereYoureGoing();
			}
			if (Separation.Enabled)
			{
				Separation.Separate();
			}
			if (Cohesion.Enabled)
			{
				Cohesion.Cohesie();
			}
			Velocity += Acceleration;
			Velocity *= MaxSpeed;
			//velocity = LimitVector(velocity, 5);
			Trans.position += (Vector3) Velocity * Time.deltaTime * 3;
			Acceleration *= 0;
			if (Velocity.magnitude > 0)
			{
				Velocity /= 1.05f;
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	/* Updates the velocity of the current game object by the given linear acceleration */
	public void AddForce(Vector2 newForce)
	{
        //Vector2 f = _force/mass;
        //acceleration += f;
		Acceleration += newForce * Time.deltaTime;
	}
	
	public void AddPureForce(Vector2 newForce)
	{
		//Vector2 f = _force/mass;
		//acceleration += f;

		Trans.position += (Vector3) newForce * Time.deltaTime;
	}

	/* Returns the steering for a character so it arrives at the target */
    public Vector2 Arrive(Vector2 targetPosition)
	{
		/* Get the right direction for the linear acceleration */
        Vector2 direction = targetPosition - (Vector2)Trans.position;

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
		if (dist > SlowDownRadius)
		{
            CurrentSpeed = MaxSpeed;
		}
		else
		{
			CurrentSpeed = MaxSpeed * (dist / SlowDownRadius);
		}

		/* Give targetVelocity the correct speed */
		direction.Normalize();
		direction *= CurrentSpeed;

		/* Calculate the linear acceleration we want */
        Vector3 force = direction - Velocity;
		/*
         Rather than accelerate the character to the correct speed in 1 second, 
         accelerate so we reach the desired speed in timeToTarget seconds 
         (if we were to actually accelerate for the full timeToTarget seconds).
        */
		force *= 1 / TimeToTarget;

		/* Make sure we are accelerating at max acceleration */
		if (force.magnitude > MaxAcceleration)
		{
			force.Normalize();
			force *= MaxAcceleration;
		}

		return force;
	}

	/* A seek steering behavior. Will return the steering for the current game object to seek a given position */
	public void Seek(Vector2 targetPosition)
	{
        Vector2 desired = targetPosition - (Vector2)Trans.position;

		desired.Normalize();

		desired *= MaxSpeed;

        Vector2 force = desired - Velocity;
        force = LimitVector(force, MaxForce);

        AddForce(force);
	}

    public static Vector2 LimitVector(Vector2 value, float limit)
    {
	    if (value.x > (0 + limit))
	    {
		    value.x = limit;
	    }
	    if (value.x < (0 - limit))
	    {
		    value.x = -limit;
	    }
	    if (value.y > (0 + limit))
	    {
		    value.y = limit;
	    }
	    if (value.y < (0 - limit))
	    {
		    value.y = -limit;
	    }
	    return value;
    }

	//public Vector3 Seek(Vector3 targetPosition)
	//{
	//	return Seek(targetPosition, maxAcceleration);
	//}

	/* Makes the current game object look where he is going */
	private void LookWhereYoureGoing()
	{
		Vector2 direction = Velocity+Acceleration;

//		if (smoothing)
//		{
//			if (velocitySamples.Count == numSamplesForSmoothing)
//			{
//				velocitySamples.Dequeue();
//			}
//
//			velocitySamples.Enqueue(Velocity+Acceleration);
//
//			direction = Vector2.zero;
//
//			foreach (Vector2 v in velocitySamples)
//			{
//				direction += v;
//			}
//
//			direction /= velocitySamples.Count;
//		}

		LookAtDirection(direction);
	}

	private void LookAtDirection(Vector2 direction)
	{
		direction.Normalize();

		// If we have a non-zero direction then look towards that direciton otherwise do nothing
		if (direction.sqrMagnitude > 0.01f)
		{
			float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)-90;
			float rotation = Mathf.LerpAngle(Trans.rotation.eulerAngles.z, toRotation, Time.deltaTime * TurnSpeed);

            Trans.rotation = Quaternion.Euler(0, 0, rotation);
		}
	}

	private void LookAtDirection(Quaternion toRotation)
	{
		LookAtDirection(toRotation.eulerAngles.z);
	}

	private void LookAtDirection(float toRotation)
	{
		float rotation = Mathf.LerpAngle(Trans.rotation.eulerAngles.z, toRotation, Time.deltaTime * TurnSpeed);

		Trans.rotation = Quaternion.Euler(0, 0, rotation);
	}

	

	private Vector2 Interpose(Rigidbody target1, Rigidbody target2)
	{
		Vector2 midPoint = (target1.position + target2.position) / 2;

        float timeToReachMidPoint = Vector2.Distance(midPoint, Trans.position) / MaxSpeed;

		Vector2 futureTarget1Pos = target1.position + target1.velocity * timeToReachMidPoint;
		Vector2 futureTarget2Pos = target2.position + target2.velocity * timeToReachMidPoint;

		midPoint = (futureTarget1Pos + futureTarget2Pos) / 2;

        return Vector2.zero;//Arrive(midPoint);
	}

	/* Checks to see if the target is in front of the character */
	private bool IsInFront(Vector2 target)
	{
		return IsFacing(target, 0);
	}

	private bool IsFacing(Vector2 target, float cosineValue)
	{
        Vector2 facing = Trans.right.normalized;

        Vector2 directionToTarget = (target - (Vector2)Trans.position);
		directionToTarget.Normalize();

		return Vector2.Dot(facing, directionToTarget) >= cosineValue;
	}

//	private void OnDrawGizmos()
//	{
//		Gizmos.color = Color.green;
//		Gizmos.DrawSphere(Velocity + (Vector2)Trans.position, 0.1f);
//		Gizmos.DrawLine(Velocity + (Vector2)Trans.position, Trans.position);
//		if (followBase.enabled)
//			followBase.Giz();
//	}
}
