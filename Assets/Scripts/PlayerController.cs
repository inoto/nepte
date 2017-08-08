using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int owner = 0;

    public bool isInitialized = false;
    public bool isDefeated = false;

    [Header("Cache")]
    public RallyPoint rallyPoint;
    public GameObject baseControl;
    public AIPlayer aiPlayer;
    private GameObject startLocationObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject rallyPointPrefab;

    [Header("Sprite sets")]
	public Sprite[] spriteSetBases;
	public Sprite[] spriteSetRallyPoints;

	// Use this for initialization
	void Start ()
    {
        CreateBase();

        CreateRallyPoint();

        if (owner != 0)
            aiPlayer = gameObject.AddComponent<AIPlayer>();

        isInitialized = true;
    }

    void Update()
    {
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

    void CreateBase()
    {
        baseControl = Instantiate(basePrefab, transform.position, transform.rotation);
        baseControl.GetComponent<Base>().owner = owner;
        baseControl.transform.SetParent(transform);

        baseControl.gameObject.GetComponent<SpriteRenderer>().sprite = spriteSetBases[owner];
    }

    void CreateRallyPoint()
    {
		GameObject rallyPointObject = Instantiate(rallyPointPrefab, transform.position, transform.rotation);
        rallyPoint = rallyPointObject.GetComponent<RallyPoint>();
        rallyPoint.owner = owner;
		rallyPointObject.transform.SetParent(transform);
        baseControl.GetComponent<Base>().rallyPointObject = rallyPoint.gameObject;

        rallyPoint.gameObject.GetComponent<SpriteRenderer>().sprite = spriteSetRallyPoints[owner];
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
