using UnityEngine;

[System.Serializable]
public class Separation
{
	public bool Enabled;
	
	private int count;
	private Vector2 sum;
	public float Desired;

    public float ForceMultiplier = 1;
    public float MaxSpeed = 2;

    [System.NonSerialized] public Mover Mover;

    public void Activate(Mover newMover)
    {
        Mover = newMover;
	    Clear();
    }

    private void Clear()
    {
        sum = Vector2.zero;
		count = 0;
    }

    public void AddSeparation(Vector2 point, float dist)
    {
        Vector2 diff = (Vector2)Mover.Trans.position - point;
	    //diff = Mover.LimitVector(diff, mover.maxSpeed);
	    
        diff /= dist;
        sum += diff;
        count++;
	}

    public void Separate()
	{
		if (count > 0)
		{
			sum /= count;
			//sum.Normalize();
			sum *= MaxSpeed;
			
			Vector2 force = sum - Mover.Velocity;
            force *= ForceMultiplier;
			force = Mover.LimitVector(force, Mover.MaxForce);
			Mover.AddForce(force);
            Clear();
		}
    }
}
