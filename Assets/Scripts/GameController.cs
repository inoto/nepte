using UnityEngine;

public class GameController : MonoBehaviour
{
    public int players = 3;

    public int winPoints = 0;

    [Header("From scene")]
    public GameObject cameraControllerObject;
    public GameObject battlegroundObject;
    public GameObject[] playerControllerObject;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerControllerPrefab;

	[Header("Textures")]
	public Sprite[] spriteSetBases;
    public Sprite[] spriteSetDrones;
    public Sprite[] spriteSetRallyPoints;
    public Sprite[] spriteSetLaserMissiles;

    // Use this for initialization
    void Start ()
    {
        CreatePlayers();

        cameraControllerObject.transform.SetParent(transform);

        battlegroundObject.transform.SetParent(transform);
    }
    
    // Update is called once per frame
    void Update ()
    {
        if (winPoints >= 2)
        {
            Debug.Log("You win!");
            Time.timeScale = 0.1f;
            winPoints = 0;
        }
        else if (winPoints < 0)
        {
			Debug.Log("You lose!");
			Time.timeScale = 0.1f;
            winPoints = 0;
        }
    }

    void CreatePlayers()
    {
        for (int i = 0; i < players; i++)
        {
            playerControllerObject[i] = Instantiate(playerControllerPrefab);
            playerControllerObject[i].transform.SetParent(transform);
            PlayerController playerController = playerControllerObject[i].GetComponent<PlayerController>();
            playerController.owner = i;
        }
    }

}
