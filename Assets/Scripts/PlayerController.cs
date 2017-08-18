using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int owner;

    public int playerUnitCount = 0;
    public static int unitCount = 0;
    public bool isInitialized = false;
    public bool isDefeated = false;

	[Header("Cache")]
	public Transform trans;
    public RallyPoint rallyPoint;
    public Base baseControl;
    public AIPlayer aiPlayer;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject rallyPointPrefab;

    private void Awake()
    {
        trans = GetComponent<Transform>();
    }

    private void Start()
    {
		CreateBase();

		CreateRallyPoint();
    }

    public void ActionsWithOwner()
    {
		if (owner != 0)
			aiPlayer = gameObject.AddComponent<AIPlayer>();
        isInitialized = true;
    }

    void Update()
    {
		// TODO: rework to use events or something like this
        if (isInitialized)
        {
            if (baseControl == null)
            {

                if (owner != 0)
                {
                    GameController.Instance.playerControllerObject.Remove(gameObject);
                    if (GameController.Instance.playerControllerObject.Count <= 1)
                        GameController.Instance.winPoints += 1;
                    // define enemies

                    Destroy(gameObject);
                }
                else
                    GameController.Instance.winPoints -= 1;
            }
        }
    }

    private void OnDestroy()
    {
        unitCount = 0;
    }

    void CreateBase()
    {
        Vector3 point = trans.position;
		point.z = 0.1f;
		GameObject baseObject = Instantiate(basePrefab, point, trans.rotation);
        baseObject.transform.SetParent(trans);
		baseControl = baseObject.GetComponent<Base>();
        baseControl.owner = owner;
        baseControl.StartWithOwner();
    }

    void CreateRallyPoint()
    {
        Vector3 point = trans.position;
        point.z = -0.1f;
		GameObject rallyPointObject = Instantiate(rallyPointPrefab, point, trans.rotation);
        rallyPointObject.transform.SetParent(trans);
        rallyPoint = rallyPointObject.GetComponent<RallyPoint>();
        rallyPoint.owner = owner;
        rallyPoint.StartWithOwner();

        baseControl.GetComponent<Base>().rallyPointObject = rallyPoint.gameObject;
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
