using UnityEngine;

public class LaserMissile : MonoBehaviour
{
    public int owner;

    public bool wasExecuted = false;

    [Header("Destination")]
    public Vector3 gotoPosition;
    public Vector3 newPosition;

    [Header("Move")]
    public float speed = 0.2f;
    private float step;
    private float angle;

    [Header("Attack")]
    public int damage;

    // Use this for initialization
    void Start ()
    {
        newPosition = gotoPosition - transform.position;

        Rotate();
    }

    // Update is called once per frame
    void Update ()
    {
        newPosition = gotoPosition - transform.position;

        if (transform.position == gotoPosition)
            Destroy(gameObject);
        if (wasExecuted)
            Destroy(gameObject);
        
        Move();
    }

    void Rotate()
    {
		angle = Mathf.Atan2(newPosition.y, newPosition.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Move()
    {
		step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(transform.position, gotoPosition, step);
    }
}
