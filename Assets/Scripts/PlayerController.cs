using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int owner = 0;

    public bool isInitialized = false;
    public bool isDefeated = false;

    [Header("Cache")]
    public GameObject gameControllerParent;
    public GameObject playerRallyPoint;
    public GameObject playerBase;
    private GameObject startLocation;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject rallyPointPrefab;

	// Use this for initialization
	void Start ()
    {
        gameControllerParent = gameObject.transform.parent.gameObject;

        AssignStartLocation();

        CreateBase();

        CreateRallyPoint();

        isInitialized = true;
    }

    void Update()
    {
        if (isInitialized)
        {
            if (!playerBase && !isDefeated)
            {
                if (owner != 0)
                    gameControllerParent.GetComponent<GameController>().winPoints += 1;
                else
                    gameControllerParent.GetComponent<GameController>().winPoints -= 1;
                isDefeated = true;
            }
        }
    }

    void AssignStartLocation()
    {
        switch (owner)
        {
            case 0:
                startLocation = GameObject.Find("StartPlayer");
                break;
            case 1:
                startLocation = GameObject.Find("StartAIFirst");
                break;
            case 2:
                startLocation = GameObject.Find("StartAISecond");
                break;
        }

        if (startLocation)
            Destroy(startLocation);
    }

    void CreateBase()
    {
        playerBase = Instantiate(basePrefab, startLocation.transform.position, startLocation.transform.rotation);
        playerBase.GetComponent<Base>().owner = owner;
        playerBase.transform.SetParent(transform);
    }

    void CreateRallyPoint()
    {
		playerRallyPoint = Instantiate(rallyPointPrefab, startLocation.transform.position, startLocation.transform.rotation);
        playerRallyPoint.GetComponent<RallyPoint>().owner = owner;
		playerRallyPoint.transform.SetParent(transform);
        playerBase.GetComponent<Base>().playerRallyPoint = playerRallyPoint;
    }
}
