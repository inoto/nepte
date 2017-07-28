using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int owner = 0;

    [SerializeField]
    private GameObject playerBase;

	[SerializeField]
    private GameObject playerRallyPoint;

    public GameObject pRallyPoint;

    private GameObject startLocation;

	// Use this for initialization
	void Start ()
    {
        AssignStartLocation();

        CreateBase();

        CreateRallyPoint();

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void AssignStartLocation()
    {
        if (owner == 0)
        {
            startLocation = GameObject.Find("StartPlayer");
        }
		else if (owner == 1)
		{
			startLocation = GameObject.Find("StartAIFirst");
		}
		else if (owner == 2)
		{
			startLocation = GameObject.Find("StartAISecond");
		}
        if (startLocation)
            Destroy(startLocation);
    }

    void CreateBase()
    {
        GameObject instance = Instantiate(playerBase, startLocation.transform.position, startLocation.transform.rotation);
        instance.GetComponent<Base>().owner = owner;
        instance.transform.SetParent(transform);
    }

    void CreateRallyPoint()
    {
		GameObject instance = Instantiate(playerRallyPoint, startLocation.transform.position, startLocation.transform.rotation);
        instance.GetComponent<RallyPoint>().owner = owner;
		instance.transform.SetParent(transform);
        pRallyPoint = instance;
    }
}
