using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsMover : MonoBehaviour
{
	Camera2DController controlCamera;

    Transform transStars1;
    Transform transStars2;
    Transform transStars3;

	void Awake ()
    {
		controlCamera = Camera.main.GetComponent<Camera2DController>();
	}

	void Start()
	{
		transStars1 = transform.GetChild(0).transform;
		transStars2 = transform.GetChild(1).transform;
		transStars3 = transform.GetChild(2).transform;

		//controlCamera.onTranslate += MoveStars1;
        //controlCamera.onTranslate += MoveStars2;
        //controlCamera.onTranslate += MoveStars3;
        controlCamera.onTranslate += MoveStars;
	}
	
    void MoveStars1()
    {
        //transStars1.Translate((Vector2)Input.mousePosition * 0.002f);
        //transStars1.position = Vector2.MoveTowards(transStars1.position, Camera.main.transform.position, 1);
        transStars1.position = (Vector2)Camera.main.transform.position * 0.2f;
    }

	void MoveStars2()
	{
		transStars2.position = (Vector2)Camera.main.transform.position * 0.3f;
	}

	void MoveStars3()
	{
		transStars3.position = (Vector2)Camera.main.transform.position * 0.4f;
	}

    void MoveStars()
    {
        Vector3 vec = Camera.main.transform.position;
        vec.z = 1;
        transStars1.position = vec * 0.2f;
        vec.z = 2;
        transStars2.position = vec * 0.3f;
        vec.z = 3;
        transStars3.position = vec * 0.4f;
    }
}
