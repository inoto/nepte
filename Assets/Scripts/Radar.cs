using UnityEngine;

public class Radar : MonoBehaviour
{

    public float radiusDetection = 5;

	[Header("Cache")]
	private Unit unitComponent;
    private IOwnable triggeredDrone;

    public CollisionCircle collisionCircle;

	// Use this for initialization
	void Awake ()
    {
        unitComponent = GetComponent<Unit>();
	}

	private void Start()
	{
        collisionCircle = new CollisionCircle(transform.position, radiusDetection, this);
		//CollisionManager.Instance.AddRadar(collisionCircle);
	}

}
