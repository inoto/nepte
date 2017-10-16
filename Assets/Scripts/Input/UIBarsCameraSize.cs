using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarsCameraSize : MonoBehaviour
{
    Camera theCamera;
    ControllableCamera controlCamera;

    private void Awake()
    {
        theCamera = GetComponent<Camera>();
        controlCamera = Camera.main.GetComponent<ControllableCamera>();
    }

    // Use this for initialization
    void Start ()
    {
        controlCamera.onZoom += ChangeSize;
	}
	
	public void ChangeSize()
    {
        theCamera.orthographicSize = Camera.main.orthographicSize;
    }
}
