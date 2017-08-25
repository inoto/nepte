using UnityEngine;
using System.Collections;

public class Separation : MonoBehaviour
{
    public float maxSpeed = 2;
	public float maxAcceleration = 2;

	/* This should be the maximum separation distance possible between a separation
     * target and the character.
     * So it should be: separation sensor radius + max target radius */
	public float distanceAddition = 1f;

    [System.NonSerialized] public Mover mover;

    private void Awake()
    {
        mover = GetComponent<Mover>();
    }

	private void Update()
	{

        Separate();

	}

    public void Separate()
	{
		Vector2 sum = new Vector2();
		float desiredSeparation = mover.drone.body.radius * 2;
		int countSeparation = 0;

        // cohesion
        Vector2 sumCohesion = new Vector2();
        float desiredCohesion = mover.drone.radar.radius*2;
		int countCohesion = 0;

		//Vector2 sumAlign = new Vector2();
		//float desiredAlign = mover.drone.radar.radius * 2;
        //int countAlign = 0;

        foreach (Drone unit in CollisionManager.Instance.objects)
        {
            if (mover.drone == unit)
                continue;
            float dist = (mover.drone.trans.position - unit.trans.position).magnitude;
            if (dist > 0 && dist < desiredSeparation)
            {
                Vector2 diff = (mover.drone.trans.position - unit.trans.position).normalized;
                diff /= dist;
                sum += diff;
                countSeparation++;
            }
    //        if (mover.drone.trans.position == unit.trans.position)
    //        {
    //            Vector2 direction = new Vector2(0, 0).normalized;
				//direction /= dist;
				//sum += direction;
				//countSeparation++;
            //}
            // cohesion
            if (dist > 0 && dist < desiredCohesion
                && mover.drone.owner == unit.owner)
            {
                sumCohesion += (Vector2)unit.trans.position;
                countCohesion++;
            }
            // align
            //if (mover.drone.owner == unit.owner)
            //{
            //    sumAlign += unit.mover.velocity;
            //    countAlign++;
            //}
        }
        if (countSeparation > 0)
        {
            sum /= countSeparation;
            //sum.Normalize();
            sum *= maxSpeed;
            Vector2 force = sum - mover.velocity;
            force = Mover.LimitVector(force, mover.maxForce);
            //force *= 1.5f;
            mover.AddForce(force);
        }
        // cohesion
        if (countCohesion > 0)
        {
            sumCohesion /= countCohesion;
            mover.Seek(sumCohesion);
            //force *= 0.8f;
            //mover.AddForce(force);
        }
        // align
        //sumAlign /= countAlign;
        ////sumAlign. = maxSpeed;
        //Vector2 newForce = sumAlign - mover.velocity;
        //newForce = Mover.LimitVector(newForce, mover.maxForce);
        //mover.AddForce(newForce);
    }
}
