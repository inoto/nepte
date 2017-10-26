using UnityEngine;

public class CameraWithEvents2 : MonoBehaviour
{
	private Camera2DController theCamera;
#if UNITY_EDITOR
	private Camera2DController cameraMouse;
#endif
	private Camera2DController cameraTouch;

	public GameObject HpCamObject;

	// Use this for initialization
	private void Awake ()
    {
        theCamera = GetComponent<Camera2DController>();
#if UNITY_EDITOR
		cameraMouse = GetComponent<Camera2DController>();
#endif
		cameraTouch = GetComponent<Camera2DController>();

        HpCamObject = GameObject.Find("CameraUIBars");

	    TurnCameraOff();
    }

    private void Start()
    {
        GameManager.Instance.OnGamePaused += TurnCameraOff;
        GameManager.Instance.OnGameContinued += TurnCameraOn;
    }

	private void TurnCameraOn()
    {
        theCamera.enabled = true;
#if UNITY_EDITOR
		cameraMouse.enabled = true;
#endif
        cameraTouch.enabled = true;

        if (HpCamObject != null)
        {
	        HpCamObject.SetActive(!HpCamObject.activeSelf);
        }
    }

	private void TurnCameraOff()
	{
		theCamera.enabled = false;
#if UNITY_EDITOR
		cameraMouse.enabled = false;
#endif
		cameraTouch.enabled = false;

		if (HpCamObject != null)
		{
			HpCamObject.SetActive(!HpCamObject.activeSelf);
		}
	}

}