﻿using UnityEngine;

public class TouchController : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    public float zoomSpeed = 0.2f;

    [Header("Cache")]
    private Battleground battleground;
    private Camera theCamera;
    private Camera cameraUIBars;
    private Touch pinchFinger1, pinchFinger2;

    void Start()
    {
        //#if UNITY_EDITOR
        //        Destroy(this);
        //#endif

		battleground = GameObject.Find("Battleground").GetComponent<Battleground>();
        theCamera = gameObject.GetComponent<Camera>();
        cameraUIBars = GameObject.Find("CameraUIBars").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.z != -3)
        {
            Vector3 tmpVector = transform.position;
            tmpVector.z = -3;
            transform.position = tmpVector;
        }

        if (GameController.Instance.IsGame)
        {
            if (Input.touchCount > 0)
            {
                if (Input.touchCount == 2)
                {
                    Zoom();
                }
                else if (Input.touchCount == 1)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        Move();
                    }
                    else
                    {
                        Click();
                    }
                }

            }
        }


	}

    void Click()
    {
        if (!GameController.Instance.IsPaused )
        {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPosition.z = 0;
            // TODO: use raycast instead

            //GameObject[] players = GameController.Instance.playerControllerObject;
            //foreach (GameObject player in players)
            //{
            //    player.GetComponent<PlayerController>().rallyPoint.SetNew(clickPosition);
            //}
            //GameController.Instance.playerControllerObject[0].GetComponent<PlayerController>().rallyPoint.SetNew(clickPosition);
        }
    }

    void Zoom()
    {
		Touch finger1 = Input.GetTouch(0);
		Touch finger2 = Input.GetTouch(1);

		// Find the position in the previous frame of each touch.
		Vector2 touchZeroPrevPos = finger1.position - finger1.deltaPosition;
		Vector2 touchOnePrevPos = finger2.position - finger2.deltaPosition;

		// Find the magnitude of the vector (the distance) between the touches in each frame.
		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (finger1.position - finger2.position).magnitude;

		// Find the difference in the distances between each frame.
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		// ... change the orthographic size based on the change in distance between the touches.
        theCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;
		cameraUIBars.orthographicSize += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

		// Make sure the orthographic size never drops below zero.
		theCamera.orthographicSize = Mathf.Clamp(theCamera.orthographicSize, 1.0f, 2.5f);
		cameraUIBars.orthographicSize = Mathf.Clamp(theCamera.orthographicSize, 1.0f, 2.5f);
    }

    void Move()
    {
		Vector3 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
		float distance = moveSpeed * Time.deltaTime;

		Vector3 position = transform.position;
        position += (-touchDeltaPosition) * distance;
		position.x = Mathf.Clamp(position.x, -(battleground.size.x / 4), battleground.size.x / 4);
		position.y = Mathf.Clamp(position.y, -(battleground.size.y / 3), battleground.size.y / 3);
		//position.z = -3;
		transform.position = position;
    }

	public void ResetCamerasSize()
	{
		theCamera.orthographicSize = 1;
		cameraUIBars.orthographicSize = 1;
	}
}
