using UnityEngine;

public class PlayerController : MonoBehaviour, IOwnable
{
    public int owner = 0;

    public bool isInitialized = false;
    public bool isDefeated = false;

    [Header("Cache")]
    public GameController gameControllerObjectParent;
    public RallyPoint rallyPoint;
    public Base baseControl;
    private GameObject startLocationObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject rallyPointPrefab;

	// Use this for initialization
	void Start ()
    {
        gameControllerObjectParent = transform.parent.gameObject.GetComponent<GameController>();

        AssignStartLocation();

        CreateBase();

        CreateRallyPoint();

        isInitialized = true;
    }

    void Update()
    {
        if (isInitialized)
        {
            if (baseControl == null && !isDefeated)
            {
                
                if (owner != 0)
                    gameControllerObjectParent.winPoints += 1;
                else
                    gameControllerObjectParent.winPoints -= 1;
                isDefeated = true;
            }
        }
    }

    void AssignStartLocation()
    {
        switch (owner)
        {
            case 0:
                startLocationObject = GameObject.Find("StartPlayer");
                break;
            case 1:
                startLocationObject = GameObject.Find("StartAIFirst");
                break;
            case 2:
                startLocationObject = GameObject.Find("StartAISecond");
                break;
        }

        if (startLocationObject)
        {
            transform.position = startLocationObject.transform.position;
            Destroy(startLocationObject);
        }
    }

    void CreateBase()
    {
        GameObject baseObject = Instantiate(basePrefab, transform.position, transform.rotation);
        baseControl = baseObject.GetComponent<Base>();
        baseControl.owner = owner;
        baseObject.transform.SetParent(transform);

        baseControl.gameObject.GetComponent<SpriteRenderer>().sprite = gameControllerObjectParent.spriteSetBases[owner];
    }

    void CreateRallyPoint()
    {
		GameObject rallyPointObject = Instantiate(rallyPointPrefab, transform.position, transform.rotation);
        rallyPoint = rallyPointObject.GetComponent<RallyPoint>();
        rallyPoint.owner = owner;
		rallyPointObject.transform.SetParent(transform);
        baseControl.rallyPointObject = rallyPoint.gameObject;

        rallyPoint.gameObject.GetComponent<SpriteRenderer>().sprite = gameControllerObjectParent.spriteSetRallyPoints[owner];
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
