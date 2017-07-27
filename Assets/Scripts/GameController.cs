using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Vector3 newPosition;

    public GameObject basePlayer;

    public GameObject rpoint;

    private Vector3 mousePosition;

    // Use this for initialization
    void Start ()
    {
        rpoint = GameObject.Find("Rpoint");
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            rpoint.transform.position = mousePosition;
        }
    }
}
