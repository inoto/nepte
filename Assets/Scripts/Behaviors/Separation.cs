using UnityEngine;
using System.Collections;

[System.Serializable]
public class Separation
{
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
		//Vector2 sumAlign = new Vector2();
		//float desiredAlign = mover.drone.radar.radius * 2;
        //int countAlign = 0;

    //    foreach (CollisionCircle unit in CollisionManager.Instance.objects)
    //    {
    //        if (mover == unit)
    //            continue;
    //        float dist = (mover.trans.position - unit.trans.position).magnitude;
    //        if (dist > 0 && dist < desiredSeparation)
    //        {
    //            Vector2 diff = (mover.trans.position - unit.trans.position).normalized;
    //            diff /= dist;
    //            sum += diff;
    //            countSeparation++;
    //        }
    ////        if (mover.drone.trans.position == unit.trans.position)
    ////        {
    ////            Vector2 direction = new Vector2(0, 0).normalized;
				////direction /= dist;
				////sum += direction;
				////countSeparation++;
        //    //}
        //    // align
        //    //if (mover.drone.owner == unit.owner)
        //    //{
        //    //    sumAlign += unit.mover.velocity;
        //    //    countAlign++;
        //    //}
        //}

        // align
        //sumAlign /= countAlign;
        ////sumAlign. = maxSpeed;
        //Vector2 newForce = sumAlign - mover.velocity;
        //newForce = Mover.LimitVector(newForce, mover.maxForce);
        //mover.AddForce(newForce);
    }
}
