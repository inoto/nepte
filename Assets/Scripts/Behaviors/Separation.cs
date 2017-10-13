using UnityEngine;
using System.Collections;

[System.Serializable]
public class Separation
{
	public bool enabled;
	
	public int count;
    public Vector2 sum;
	public float desired;

    public float forceMultiplier = 1;
    public float maxSpeed = 2;
	public float maxAcceleration = 2;

	/* This should be the maximum separation distance possible between a separation
     * target and the character.
     * So it should be: separation sensor radius + max target radius */
	public float distanceAddition = 1f;

    [System.NonSerialized] public Mover mover;

    public void Activate(Mover _mover)
    {
        mover = _mover;
	    Clear();
		//desired = 1;
    }

    public void Clear()
    {
        sum = Vector2.zero;
		//desired = mover.body.radius * 2;
		count = 0;
    }

    public void AddSeparation(Vector2 point, float dist)
    {
        Vector2 diff = (Vector2)mover.trans.position - point;
	    //diff = Mover.LimitVector(diff, mover.maxSpeed);
	    
        diff /= dist;
        sum += diff;
        count++;
	}

    public void Separate()
	{
		if (count > 0)
		{
            //Debug.Log("separation works");
			sum /= count;
			//sum.Normalize();
			sum *= maxSpeed;
			
			Vector2 force = sum - mover.velocity;
            force *= forceMultiplier;
			force = Mover.LimitVector(force, mover.maxForce);
			mover.AddForce(force);
            Clear();
		}
    }
}
