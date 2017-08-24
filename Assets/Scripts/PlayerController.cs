using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Owner owner = new Owner();

	public delegate void Player();
	public event Player OnPlayerDefeated = delegate { };

    public int playerUnitCount = 0;
    public static int unitCount = 0;
    public bool isInitialized = false;
    public bool isDefeated = false;

	[Header("Cache")]
	public Transform trans;
    public RallyPoint rallyPoint;
    public Base bas;
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
        owner.playerController = this;

		CreateBase();

		CreateRallyPoint();
    }

    public void ActionsWithOwner()
    {
        if (owner.playerNumber != 0)
			aiPlayer = gameObject.AddComponent<AIPlayer>();
        isInitialized = true;
    }

    void Update()
    {
		// TODO: rework to use events or something like this
        if (isInitialized)
        {
            if (bas.isDead)
            {

                if (owner.playerNumber != 0)
                {
                    GameController.Instance.playerControllerObject.Remove(gameObject);
                    if (GameController.Instance.playerControllerObject.Count <= 1)
                        GameController.Instance.winPoints += 1;
                    // define enemies

                    gameObject.SetActive(false);
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
		bas = baseObject.GetComponent<Base>();
        bas.owner = owner;
        bas.StartWithOwner();
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

        bas.GetComponent<Base>().rallyPoint = rallyPoint;
    }

	public int GetOwner()
	{
		return owner.playerNumber;
	}

	public void SetOwner(int newOwner)
	{
		owner.playerNumber = newOwner;
	}

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
