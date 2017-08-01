using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInputController : MonoBehaviour
{
    public float moveSpeed = 0.1f;
	public float zoomSpeed = 0.2f;

    private Vector3 mousePosition;

	[Header("Cache")]
    public Battleground battleground;
	private float deltaX, deltaY;

	void Start()
	{
		//theCamera = GetComponent<Camera>();
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

		if (Input.GetMouseButtonDown(0))
		{
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
            // TODO: use raycast instead

            GameObject[] players = GetPlayerControllers();
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().rallyPoint.SetNew(mousePosition);
            }
		}
        else if (Input.GetMouseButtonDown(1))
        {
            GameObject[] players = GetPlayerControllers();
			foreach (GameObject player in players)
			{
                mousePosition = player.GetComponent<PlayerController>().baseControl.transform.position;
                mousePosition.z = 0;
				player.GetComponent<PlayerController>().rallyPoint.SetNew(mousePosition);
			}
        }
	}

    GameObject[] GetPlayerControllers()
    {
        GameController gameController = transform.parent.gameObject.GetComponent<GameController>();
        return gameController.playerControllerObject;
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
