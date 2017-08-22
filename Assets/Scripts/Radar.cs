using UnityEngine;

public class Radar : MonoBehaviour, ICollidable
{
    public bool showRadius = false;

    public float radius = 5;
	public CollisionType cType = CollisionType.Radar;

	[Header("Cache")]
	public Transform trans;
	public Drone droneParent;

	// Use this for initialization
	void Awake ()
    {
        trans = GetComponent<Transform>();
		droneParent = trans.parent.gameObject.GetComponent<Drone>();
	}

	public void OnEnable()
	{
		trans = GetComponent<Transform>();
        droneParent = trans.parent.gameObject.GetComponent<Drone>();
		CollisionManager.Instance.AddCollidable(this);
	}

	public void OnDisable()
	{
		CollisionManager.Instance.RemoveCollidable(this);
	}

	public void OnDrawGizmos()
	{
		if (showRadius)
		{
			Color newColorAgain = Color.yellow;
			newColorAgain.a = 0.5f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}

	public int InstanceId
	{
		get { return gameObject.GetInstanceID(); }
	}
	public Vector2 Point
	{
		get { return trans.position; }
		set { trans.position = value; }
	}
	public float Radius
	{
		get { return radius; }
	}
	public float RadiusHard
	{
		get { return radius; }
	}
	public CollisionType collisionType
	{
		get { return cType; }
	}
	public bool Active
	{
		get { return gameObject.activeSelf; }
	}
	public GameObject GameObject
	{
		get { return trans.parent.gameObject; }
	}
	public Drone drone
	{
		get { return droneParent; }
	}
	public Base bas
	{
		get { return null; }
	}
}
