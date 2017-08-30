using UnityEngine;

public class RallyPoint : MonoBehaviour
{
    Rect rect;

#if UNITY_EDITOR
	public CameraControlMouse cameraMouse;
#endif
	public CameraControlTouch cameraTouch;

	public delegate void UpdateRallyPoint();
    public event UpdateRallyPoint OnRallyPointChanged = delegate { };

	[Header("Cache")]
	public Transform trans;
    public Owner owner;
	public MeshRenderer mesh;
    public Node node;
    public Body body;

	[Header("Colors")]
	public Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
        owner = GetComponent<Owner>();
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

    public void DelayedStart()
    {
        SetOwnerAsInParent();

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

	void SetOwnerAsInParent()
	{
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
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
        if (mesh != null && owner != null)
            mesh.sharedMaterial = materials[owner.playerNumber];
        else
            Debug.LogError("Cannot assign material.");
	}
}
