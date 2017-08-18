using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public int owner;

    Rect rect;

#if UNITY_EDITOR
	public CameraControlMouse cameraMouse;
#endif
	public CameraControlTouch cameraTouch;

	public delegate void ResetRallyPoint();
    public event ResetRallyPoint OnRallyPointChanged = delegate { };

	[Header("Cache")]
	public Transform trans;
	public MeshRenderer mesh;

	[Header("Colors")]
	public Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
    }

    public void StartWithOwner()
    {
		if (owner == 0)
		{
#if UNITY_EDITOR
			cameraMouse = Camera.main.GetComponent<CameraControlMouse>();
#endif
			cameraTouch = Camera.main.GetComponent<CameraControlTouch>();
		}
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
            //.Log("new pos: " + position);
            Node node = Grid.Instance.NodeFromWorldPoint(position);
            //Debug.Log("node pos: " + node.worldPosition);
            //Debug.Log("node center: " + node.rect.center);
			rect = node.rect;
            trans.position = node.rect.center;
		}

		Pathfinding.Instance.FillDistances(trans.position, owner);

        OnRallyPointChanged();
	}

	void AssignMeterial()
	{
        //if (mesh == null)
        //    mesh = GetComponent<MeshRenderer>();
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
}
