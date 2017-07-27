using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3;

    [SerializeField]
    private Battleground battleground;

    private float deltaX, deltaY;


	// Use this for initialization
	void Start ()
    {
        //battleground = GameObject.Find("Battleground");

	}
	
	// Update is called once per frame
	void Update ()
    {
		deltaX = Input.GetAxis("Horizontal");
		deltaY = Input.GetAxis("Vertical");
		if (deltaX != 0f || deltaY != 0f)
		{
			AdjustPosition(deltaX, deltaY);
		}	
	}

	private void AdjustPosition(float tDeltaX, float tDeltaY)
	{
        Vector3 direction = new Vector3(tDeltaX, tDeltaY, 0f).normalized;
        float distance = moveSpeed * Time.deltaTime;

		Vector3 position = transform.localPosition;
        position += direction * distance;
        position.x = Mathf.Clamp(position.x, -(battleground.size.x / 4), battleground.size.x / 4);
        position.y = Mathf.Clamp(position.y, -(battleground.size.y / 3), battleground.size.y / 3);
		transform.localPosition = position;
	}
}
