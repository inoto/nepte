using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarsCameraSize2 : MonoBehaviour
{
    private Camera theCamera;
    private Camera2DController controlCamera;
    

    private void Awake()
    {
        theCamera = GetComponent<Camera>();
        controlCamera = Camera.main.GetComponent<Camera2DController>();
    }

    private void Start ()
    {
        controlCamera.onZoom += ChangeSize;
	}

    private void ChangeSize()
    {
        theCamera.orthographicSize = Camera.main.orthographicSize;
    }
}
