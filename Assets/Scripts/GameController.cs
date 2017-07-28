using UnityEngine;

public class GameController : MonoBehaviour
{
    public int players = 3;

    public GameObject playerController;

    public GameObject cameraController;

    public GameObject battleground;



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
