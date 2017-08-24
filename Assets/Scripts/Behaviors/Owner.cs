using UnityEngine;
using System.Collections;

[System.Serializable]
public class Owner
{
    public int playerNumber;
    public PlayerController playerController;

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
