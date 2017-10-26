using UnityEngine;

public class LaserMissile : MonoBehaviour
{
    public int Owner;

    public bool WasExecuted = false;

    [Header("Destination")]
    public Vector3 DestinationVector;
    public Vector3 DirectionVector;

    [Header("Move")]
    public float Speed = 5f;
    private float step;
    private float angle;

    [Header("Attack")]
    public int Damage;
    public ITargetable Target;
	public Weapon Weapon;

	private void Start ()
    {
        DirectionVector = DestinationVector - transform.position;

        Rotate();
    }

	private void OnEnable()
	{
		WasExecuted = false;
		DirectionVector = DestinationVector - transform.position;
		Rotate();
	}

	private void Update ()
    {
        DirectionVector = DestinationVector - transform.position;

        if ((Vector2)transform.position == (Vector2)DestinationVector)
		{
			if (!Target.IsDied)
			{
				Target.Damage(Weapon);
			}
			else
			{
				Weapon.Target = null;
				Weapon.HasTarget = false;
			}
			Destroy(gameObject);
		}
        Move();
    }

	private void Rotate()
    {
        angle = Mathf.Atan2(DirectionVector.y, DirectionVector.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

	private void Move()
    {
		step = Speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, DestinationVector, step);
    }

	//public void OnDrawGizmos()
	//{
	//	Color newColorAgain = Color.red;
	//	newColorAgain.a = 0.5f;
	//	Gizmos.color = newColorAgain;
    //  Gizmos.DrawSphere(directionVector, 0.3f);
	//}
}
