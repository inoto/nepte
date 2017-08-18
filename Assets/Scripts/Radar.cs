using UnityEngine;

public class Radar : MonoBehaviour
{
    public bool drawGizmos = false;

    public float radiusDetection = 5;

    [Header("Cache")]
    public Transform trans;
	private Unit unitComponent;

    public CollisionCircle collisionCircle;

	// Use this for initialization
	void Awake ()
    {
        unitComponent = GetComponent<Unit>();
        trans = GetComponent<Transform>();
	}

	private void Start()
	{
        collisionCircle = new CollisionCircle(transform.position, radiusDetection, this);
		CollisionManager.Instance.AddRadar(collisionCircle);
	}

	private void Update()
	{
		collisionCircle.point = trans.position;
	}

	public void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			Color newColorAgain = Color.yellow;
			newColorAgain.a = 0.3f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(collisionCircle.point, collisionCircle.radius);
		}
	}

}
