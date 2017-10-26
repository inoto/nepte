using UnityEngine;

[System.Serializable]
public class Cohesion
{
	public bool Enabled;
	
	private int count;
	private Vector2 sum;
	public float Desired;

	[System.NonSerialized] private Mover mover;

	public void Activate(Mover newMover)
	{
		mover = newMover;
		Clear();
		//desired = 1;
	}

	private void Clear()
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
