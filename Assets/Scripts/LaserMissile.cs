using UnityEngine;

public class LaserMissile : MonoBehaviour, IOwnable
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

    // Use this for initialization
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

    // Update is called once per frame
    void Update ()
    {
        directionVector = destinationVector - transform.position;

        if ((Vector2)transform.position == (Vector2)destinationVector)
		{
            if (target.DroneObj != null)
            {
                if (target.DroneObj.mode != Drone.Mode.Died)
                    target.DroneObj.health -= damage;
            }
			else if (target.BaseObj != null)
			{
                if (!target.BaseObj.isDead)
                {
                    target.BaseObj.Damage(damage);
                }
			}
            //else
            //{
            //    Debug.Log("target nulled");
            //    creator.target = null;
            //}
			ObjectPool.Recycle(gameObject);
		}

        //if (angle == Vector3.Angle(transform.forward, directionVector))
            Move();
        //else
        //{
        //    directionVector = destinationVector - transform.position;
        //    Rotate();
        //}
    }

	

    void Rotate()
    {
        //transform.Rotate(Vector3.zero);
        angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //directionVector.x = 0;
        //directionVector.y = 0;
        ////directionVector.z -= 90;
        //transform.LookAt(directionVector);
        //transform.Rotate(Vector3.forward);
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

	//public void OnDrawGizmos()
	//{
	//	Color newColorAgain = Color.red;
	//	newColorAgain.a = 0.5f;
	//	Gizmos.color = newColorAgain;
 //       Gizmos.DrawSphere(directionVector, 0.3f);
	//}
}
