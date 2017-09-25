using UnityEngine;
using System.Collections;

[System.Serializable]
public class Separation
{
	public bool enabled = true;
	
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

    public Separation(Mover _mover)
    {
        mover = _mover;
        sum = new Vector2();
		desired = mover.body.radius * 2;
//        desired = mover.radar.radius;
		count = 0;
    }

    public void Clear()
    {
        sum = Vector2.zero;
		//desired = mover.body.radius * 2;
		count = 0;
    }

    public void AddSeparation(Vector2 point, float dist)
    {
        Vector2 diff = ((Vector2)mover.trans.position - point).normalized;
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
			force = Mover.LimitVector(force, mover.maxForce);
            force *= forceMultiplier;
			mover.AddForce(force);
            Clear();
		}
    }
}
