using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputController : MonoBehaviour
{
    public float moveSpeed = 0.1f;
	public float zoomSpeed = 0.2f;

    private Vector3 mousePosition;

	[Header("Cache")]
    private GameController gameController;
    private Battleground battleground;
    private Camera cameraUIBars;
	private float deltaX, deltaY;

	void Start()
	{
		//touchController = GetComponent<TouchController>();
		//pcIputController = GetComponent<PCInputController>();

		//#if UNITY_IPHONE || UNITY_ANDROID
		//Destroy(this);
#if !UNITY_EDITOR
		Destroy(this);
#endif
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
        battleground = GameObject.Find("Battleground").GetComponent<Battleground>();
        cameraUIBars = GameObject.Find("CameraUIBars").GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update ()
    {
        if (GameController.IsGame)
        {
            deltaX = Input.GetAxis("Horizontal");
            deltaY = Input.GetAxis("Vertical");
            if (deltaX != 0f || deltaY != 0f)
            {
                Move(deltaX, deltaY);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!GameController.IsPaused)
                {
                    mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePosition.z = 0;
                    // TODO: use raycast instead

                    GameObject[] players = gameController.playerControllerObject;
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<PlayerController>().rallyPoint.SetNew(mousePosition);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                GameObject[] players = gameController.playerControllerObject;
                foreach (GameObject player in players)
                {
                    mousePosition = player.GetComponent<PlayerController>().baseControl.transform.position;
                    mousePosition.z = 0;
                    player.GetComponent<PlayerController>().rallyPoint.SetNew(mousePosition);
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
                Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
	}

	private void Move(float tDeltaX, float tDeltaY)
	{
		Vector3 direction = new Vector3(tDeltaX, tDeltaY, 0f).normalized;
		float distance = moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.localPosition;
		newPosition += direction * distance;
		newPosition.x = Mathf.Clamp(newPosition.x, -(battleground.size.x / 4), battleground.size.x / 4);
		newPosition.y = Mathf.Clamp(newPosition.y, -(battleground.size.y / 3), battleground.size.y / 3);
        transform.localPosition = newPosition;
	}

    void Zoom(float axis)
    {
		if (axis < 0) // forward
        {
            Camera.main.orthographicSize += 0.25f;
            cameraUIBars.orthographicSize += 0.25f;
		}
		if (axis > 0) // back
        {
            Camera.main.orthographicSize -= 0.25f;
            cameraUIBars.orthographicSize -= 0.25f;
		}
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1.0f, 2.5f);
        cameraUIBars.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1.0f, 2.5f);
    }
}
