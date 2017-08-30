using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public bool showRadius = false;
    public int collidedCount = 0;

    public float radius = 3.5f;

    public Transform trans;
    public Owner owner;
    public Mover mover;
    public CollisionCircle collision;

	// Use this for initialization
	void Awake()
	{
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	}

	private void Start()
	{
        collision = new CollisionCircle(this, trans, mover, owner);
		CollisionManager.Instance.AddCollidable(collision);
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.red;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
	public Vector2 Point
	{
		get { return trans.position; }
	}
	public float Radius
	{
		get { return radius; }
	}
	public float RadiusHard
	{
		get { return radius; }
	}
	public GameObject GameObject
	{
		get { return gameObject; }
	}
	public bool Active
	{
		get { return this.enabled; }
	}
	public Owner Owner
	{
		get { if (owner != null) return owner; return null; }
	}
	public Mover Mover
	{
		get { if (mover != null) return mover; return null; }
	}

}
