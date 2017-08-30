using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cohesion
{
public int count;
	public Vector2 sum;
	public float desired;

	public float maxSpeed = 2;
	public float maxAcceleration = 2;

	/* This should be the maximum separation distance possible between a separation
     * target and the character.
     * So it should be: separation sensor radius + max target radius */
	public float distanceAddition = 1f;

	[System.NonSerialized] public Mover mover;

	public Cohesion(Mover _mover)
	{
		mover = _mover;
		sum = new Vector2();
		desired = mover.body.radius * 3;
		count = 0;
	}

	public void Clear()
	{
		sum = Vector2.zero;
		//desired = mover.body.radius * 3;
		count = 0;
	}

	public void AddCohesion(Vector2 point)
	{
		sum += point;
		count++;
	}

	public void Cohesie()
	{
		if (count > 0)
		{
			sum /= count;
			mover.Seek(sum);
			//force *= 0.8f;
			//mover.AddForce(force);
            Clear();
		}
	}
}
