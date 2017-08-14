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

	public delegate void ResetRallyPoint();
	public event ResetRallyPoint OnRallyPointChanged = delegate { };

	[Header("Cache")]
	public Transform trans;
	private MeshRenderer mesh;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Start()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();

        if (owner == 0)
        {
#if UNITY_EDITOR
            cameraMouse.Attach();
            cameraMouse.onClickTap += SetNew;
#endif
            cameraTouch.Attach();
            cameraTouch.onClickTap += SetNew;
        }

		AssignMeterial();

		Pathfinding.Instance.FillDistances(trans.position, owner);
	}

	public void SetNew(Vector2 position)
	{

		Vector3 tmp = position;
		if (trans.position != tmp)
		{
			Node node = Grid.Instance.NodeFromWorldPoint(tmp);
			rect = node.rect;
			trans.position = node.rect.center;
		}

		Pathfinding.Instance.FillDistances(trans.position, owner);
		OnRallyPointChanged();
	}

	void AssignMeterial()
	{
		mesh.material = materials[owner];
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
