using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWithEvents2 : MonoBehaviour
{
	Camera2DController theCamera;
#if UNITY_EDITOR
	Camera2DController cameraMouse;
#endif
	Camera2DController cameraTouch;

    GameObject HPCamObject;

	// Use this for initialization
	void Awake ()
    {
        theCamera = GetComponent<Camera2DController>();
#if UNITY_EDITOR
		cameraMouse = GetComponent<Camera2DController>();
#endif
		cameraTouch = GetComponent<Camera2DController>();

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