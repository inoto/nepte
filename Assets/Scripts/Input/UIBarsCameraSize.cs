using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarsCameraSize2 : MonoBehaviour
{
    Camera theCamera;
    Camera2DController controlCamera;
    

    private void Awake()
    {
        theCamera = GetComponent<Camera>();
        controlCamera = Camera.main.GetComponent<Camera2DController>();
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
