using UnityEngine;

public class LaserMissile : MonoBehaviour
{
    public int owner;

    public Vector3 gotoPosition;
    public Vector3 newPosition;

    public float speed = 0.2f;
    private float step;

    // Use this for initialization
    void Start ()
    {
        newPosition = gotoPosition - transform.position;

        // rotation if needed
        float angle = Mathf.Atan2(newPosition.y, newPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Update is called once per frame
    void Update ()
    {
        newPosition = gotoPosition - transform.position;

        if (transform.position == gotoPosition)
        {
            Destroy(gameObject);
        }
            
        step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, gotoPosition, step);
    }
}
