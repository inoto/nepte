using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWithEvents : MonoBehaviour
{
    ControllableCamera theCamera;
#if UNITY_EDITOR
	CameraControlMouse cameraMouse;
#endif
	CameraControlTouch cameraTouch;

    GameObject HPCamObject;

	// Use this for initialization
	void Awake ()
    {
        theCamera = GetComponent<ControllableCamera>();
#if UNITY_EDITOR
		cameraMouse = GetComponent<CameraControlMouse>();
#endif
		cameraTouch = GetComponent<CameraControlTouch>();

        HPCamObject = GameObject.Find("CameraUIBars");

	    TurnCameraOff();
    }

    private void Start()
    {
        GameController.Instance.OnGamePaused += TurnCameraOff;
        GameController.Instance.OnGameContinued += TurnCameraOn;
    }

    public void TurnCameraOn()
    {
        theCamera.enabled = true;
#if UNITY_EDITOR
		cameraMouse.enabled = true;
#endif
        cameraTouch.enabled = true;

        if (HPCamObject != null) HPCamObject.SetActive(!HPCamObject.activeSelf);
    }
	
	public void TurnCameraOff()
	{
		theCamera.enabled = false;
#if UNITY_EDITOR
		cameraMouse.enabled = false;
#endif
		cameraTouch.enabled = false;

		if (HPCamObject != null) HPCamObject.SetActive(!HPCamObject.activeSelf);
	}

}
