using UnityEngine;

public class Owner : MonoBehaviour
{
    public int PlayerNumber = -1;
    public PlayerController PlayerController = null;

    public Owner()
    {
        PlayerNumber = -1;
        PlayerController = null;
    }

    public Owner(PlayerController newPlayerController)
    {
        PlayerController = newPlayerController;
    }
}
