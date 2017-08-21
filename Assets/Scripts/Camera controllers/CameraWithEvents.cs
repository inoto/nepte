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
	}

    private void Start()
    {
        GameController.Instance.OnGamePaused += TurnCamera;
        GameController.Instance.OnGameContinued += TurnCamera;
    }

    public void TurnCamera()
    {
        theCamera.enabled = !theCamera.enabled;
#if UNITY_EDITOR
		cameraMouse.enabled = !cameraMouse.enabled;
#endif
        cameraTouch.enabled = !cameraTouch.enabled;

        if (HPCamObject != null) HPCamObject.SetActive(!HPCamObject.activeSelf);
    }

}
