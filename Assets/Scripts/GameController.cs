using UnityEngine;

public class GameController : MonoBehaviour
{
    public int players = 3;

    public GameObject playerController;

    public GameObject cameraController;

    public GameObject battleground;

    public int winPoints = 0;

    // Use this for initialization
    void Start ()
    {
        CreatePlayers();

        cameraController.transform.SetParent(transform);

        battleground.transform.SetParent(transform);
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
            GameObject instance = Instantiate(playerController);
            instance.GetComponent<PlayerController>().owner = i;
            instance.transform.SetParent(transform);
        }
    }

}
