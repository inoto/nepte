using UnityEngine;
using System.Collections;

[System.Serializable]
public class Owner : MonoBehaviour
{
    public int playerNumber = 0;
    public PlayerController playerController = null;

    public Owner()
    {
        playerNumber = 0;
        playerController = null;
    }

    public Owner(PlayerController _playerController)
    {
        playerController = _playerController;
    }


}
