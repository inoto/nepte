using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Body : MonoBehaviour, ICollidable
{
    public bool showRadius = false;
    public int collidedCount = 0;

    public Transform trans;
    public Owner owner;
    public Mover mover;
    public CollisionCircle collision;

    public float radius = 1;
    public float radiusHard = 0.5f;
    public int strength = 0;

    public List<ICollidable> collidedList = new List<ICollidable>();

	// Use this for initialization
	void Awake()
	{
		trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
        mover = GetComponent<Mover>();
	}

    private void Update()
    {
        collidedCount = collidedList.Count;
    }

    private void Start()
    {
        collision = new CollisionCircle(this);
        CollisionManager.Instance.AddCollidable(this);
    }

    public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.green;
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
        get { return radiusHard; }
    }
	public CollisionType collisionType
    {
        get { return CollisionType.Body; }
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
    public void CheckCollision(ICollidable other)
    {
        float dist = ((Vector2)mover.trans.position - other.Point).sqrMagnitude;
		if (dist > 0)
		{
            if (dist < mover.separation.desired * mover.separation.desired)
            {
                mover.separation.AddSeparation(other.Point, dist);
                mover.separation.AddSeparation(other.Point, dist);
            }
            if (dist < mover.separation.desired * mover.separation.desired)
            {
                
            }
		}
    }
	public void AddSeparation(ICollidable other)
    {
        //if (mover != null && mover.separation != null)
            //mover.separation.AddSeparation(other);
    }
	public void AddCohesionAlign(ICollidable other)
    {
        
    }
}
