using UnityEngine;

public class LaserMissile : MonoBehaviour, IOwnable
{
    public int owner;

    public bool wasExecuted = false;

    [Header("Destination")]
    public Vector3 destinationVector;
    public Vector3 directionVector;

    [Header("Move")]
    public float speed = 0.2f;
    private float step;
    private float angle;

    [Header("Attack")]
    public int damage;

    // Use this for initialization
    void Start ()
    {
        directionVector = destinationVector - transform.position;

        Rotate();
    }

    // Update is called once per frame
    void Update ()
    {
        directionVector = destinationVector - transform.position;

        if (transform.position == destinationVector)
            ObjectPool.Recycle(gameObject);
        if (wasExecuted)
            ObjectPool.Recycle(gameObject);

        //if (angle == Vector3.Angle(transform.forward, directionVector))
            Move();
        //else
        //{
        //    directionVector = destinationVector - transform.position;
        //    Rotate();
        //}
    }

	private void OnEnable()
	{
        wasExecuted = false;
        directionVector = destinationVector - transform.position;
        Rotate();
	}

    void Rotate()
    {
        //angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.LookRotation(Vector3., directionVector);
    }

    void Move()
    {
		step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, destinationVector, step);
    }

    public void AddAttacker(GameObject newObj)
    {
        return;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

	public int GetOwner()
	{
		return owner;
	}

	public void SetOwner(int newOwner)
	{
		owner = newOwner;
	}

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
