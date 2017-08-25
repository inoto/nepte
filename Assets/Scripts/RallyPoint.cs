using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    public Owner owner;

    Rect rect;

#if UNITY_EDITOR
	public CameraControlMouse cameraMouse;
#endif
	public CameraControlTouch cameraTouch;

	public delegate void UpdateRallyPoint();
    public event UpdateRallyPoint OnRallyPointChanged = delegate { };

	[Header("Cache")]
	public Transform trans;
	public MeshRenderer mesh;
    public Node node;
    public Body body;

	[Header("Colors")]
	public Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
    }

    private void OnDestroy()
    {
        if (owner.playerNumber == 0)
        {
#if UNITY_EDITOR
            cameraMouse.onClickTap -= SetNew;
#endif
            cameraTouch.onClickTap -= SetNew;
        }
    }

    public void StartWithOwner()
    {
		if (owner.playerNumber == 0)
		{
#if UNITY_EDITOR
			cameraMouse = Camera.main.GetComponent<CameraControlMouse>();
#endif
			cameraTouch = Camera.main.GetComponent<CameraControlTouch>();
		}
		if (owner.playerNumber == 0)
		{
#if UNITY_EDITOR
			cameraMouse.Attach();
			cameraMouse.onClickTap += SetNew;
#endif
			cameraTouch.Attach();
			cameraTouch.onClickTap += SetNew;
		}

		AssignMeterial();

        Pathfinding.Instance.FillDistances(trans.position, owner.playerNumber);

        node = Grid.Instance.NodeFromWorldPoint(trans.position);
	}

	public void SetNew(Vector2 position)
	{
        Node tmpNode = Grid.Instance.NodeFromWorldPoint(position);
		if (node != tmpNode)
		{
            //.Log("new pos: " + position);
            node = tmpNode;
            //Debug.Log("node pos: " + node.worldPosition);
            //Debug.Log("node center: " + node.rect.center);
			rect = node.rect;
            trans.position = node.worldPosition;
		}

		Pathfinding.Instance.FillDistances(trans.position, owner.playerNumber);

        OnRallyPointChanged();
	}

	void AssignMeterial()
	{
        //if (mesh == null)
        //    mesh = GetComponent<MeshRenderer>();
		mesh.material = materials[owner.playerNumber];
	}
}
