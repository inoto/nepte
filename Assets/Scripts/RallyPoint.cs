using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public int owner;

    Rect rect;

#if UNITY_EDITOR
	public CameraControlMouse cameraMouse;
#endif
	public CameraControlTouch cameraTouch;

    private Vector3 mousePosition;

    public void SetNew(Vector2 position)
    {
        Vector3 tmp = position;
        if (transform.position != tmp)
        {
            Node node = Grid.Instance.NodeFromWorldPoint(tmp);
            rect = node.rect;
            transform.position = node.rect.center;
        }
    }

    private void Start()
    {
        if (owner == 0)
        {
#if UNITY_EDITOR
            cameraMouse.Attach();
            cameraMouse.onClickTap += SetNew;
#endif
            cameraTouch.Attach();
            cameraTouch.onClickTap += SetNew;
        }
    }


	public int GetOwner()
	{
		return owner;
	}

	public void SetOwner(int newOwner)
	{
		owner = newOwner;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}

    void OnDrawGizmos()
    {
        Color newColor = Color.black;
        newColor.a = 0.5f;
        Gizmos.color = newColor;
        Gizmos.DrawCube(rect.center, rect.size);
    }
}
