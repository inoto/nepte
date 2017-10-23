using UnityEngine;

public class LaserMissile : MonoBehaviour
{
    public int owner;

    public bool wasExecuted = false;

    [Header("Destination")]
    public Vector3 destinationVector;
    public Vector3 directionVector;

    [Header("Move")]
    public float speed = 5f;
    private float step;
    private float angle;

    [Header("Attack")]
    public int damage;
    public ITargetable target;
	public Weapon weapon;

    void Start ()
    {
        directionVector = destinationVector - transform.position;

        Rotate();
    }

	private void OnEnable()
	{
		wasExecuted = false;
		directionVector = destinationVector - transform.position;
		Rotate();
	}

    void Update ()
    {
        directionVector = destinationVector - transform.position;

        if ((Vector2)transform.position == (Vector2)destinationVector)
		{
			if (!target.IsDied)
			{
				target.Damage(weapon);
			}
			else
			{
				weapon.target = null;
				weapon.hasTarget = false;
			}
			Destroy(gameObject);
		}
        Move();
    }

    void Rotate()
    {
        angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Move()
    {
		step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, destinationVector, step);
    }

	//public void OnDrawGizmos()
	//{
	//	Color newColorAgain = Color.red;
	//	newColorAgain.a = 0.5f;
	//	Gizmos.color = newColorAgain;
    //  Gizmos.DrawSphere(directionVector, 0.3f);
	//}
}
