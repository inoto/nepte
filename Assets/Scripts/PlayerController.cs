using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int owner;

    public int unitCount = 0;
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

    [Header("Sprite sets")]
	public Sprite[] spriteSetBases;
	public Sprite[] spriteSetRallyPoints;

	// Use this for initialization
	void Start ()
    {
		trans = GetComponent<Transform>();

        CreateBase();

        CreateRallyPoint();

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

    void CreateBase()
    {
		GameObject baseObject = Instantiate(basePrefab, transform.position, transform.rotation);
		baseControl = baseObject.GetComponent<Base>();
        baseControl.owner = owner;
        baseControl.transform.SetParent(transform);
    }

    void CreateRallyPoint()
    {
		GameObject rallyPointObject = Instantiate(rallyPointPrefab, transform.position, transform.rotation);
        rallyPoint = rallyPointObject.GetComponent<RallyPoint>();
        rallyPoint.owner = owner;
        if (owner == 0)
        {
#if UNITY_EDITOR
			rallyPoint.cameraMouse = Camera.main.GetComponent<CameraControlMouse>();
#endif
            rallyPoint.cameraTouch = Camera.main.GetComponent<CameraControlTouch>();
        }
		rallyPointObject.transform.SetParent(transform);
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
