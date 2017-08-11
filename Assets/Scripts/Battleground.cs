using UnityEngine;

public class Battleground : MonoBehaviour
{
    public Vector3 size;

    public float width, height;
    Rect rect;

    Transform trans;
    CameraControlMouse cameraControl;

    Vector3 direction;
    Vector3 targetPosition;

	// Use this for initialization
	void Start ()
    {
        rect.min = GetComponent<MeshRenderer>().bounds.min;
        rect.max = GetComponent<MeshRenderer>().bounds.max;
        size = GetComponent<MeshRenderer>().bounds.size;
        trans = GetComponent<Transform>();
        //cameraControl.ActiveCamera.onTranslate += MoveBackground;
	}

    void MoveBackground()
    {
        direction = cameraControl.ActiveCamera.transform.position;

		targetPosition = direction + trans.position;
		targetPosition = new Vector3(
			Mathf.Min(Mathf.Abs(targetPosition.x), rect.center.x) * Mathf.Sign(targetPosition.x),
			Mathf.Min(Mathf.Abs(targetPosition.y), rect.center.y) * Mathf.Sign(targetPosition.y),
			targetPosition.z
		) - trans.position;
        trans.Translate(targetPosition);
    }
}
