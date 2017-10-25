using UnityEngine;

public class Battleground : MonoBehaviour
{
    public Vector3 Size;
    public float Width, Height;
	
	private Rect rect;

	private Transform trans;
	private Camera2DController cameraControl;

	private Vector3 direction;
	private  Vector3 targetPosition;

	// Use this for initialization
	private void Start ()
    {
        rect.min = GetComponent<MeshRenderer>().bounds.min;
        rect.max = GetComponent<MeshRenderer>().bounds.max;
        Size = GetComponent<MeshRenderer>().bounds.size;
        trans = GetComponent<Transform>();
        //cameraControl.ActiveCamera.onTranslate += MoveBackground;
	}

	private void MoveBackground()
    {
        direction = cameraControl.Camera.transform.position;

		targetPosition = direction + trans.position;
		targetPosition = new Vector3(
			Mathf.Min(Mathf.Abs(targetPosition.x), rect.center.x) * Mathf.Sign(targetPosition.x),
			Mathf.Min(Mathf.Abs(targetPosition.y), rect.center.y) * Mathf.Sign(targetPosition.y),
			targetPosition.z
		) - trans.position;
        trans.Translate(targetPosition);
    }
}
