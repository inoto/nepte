using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour
{
    public int players = 3;

    public int winPoints = 0;

	public enum States
	{
		BeforeGame,
		Game,
		PauseMenu,
		OptionsMenu,
		Win,
		Lose
	}
    public static States state;

    [Header("UI Panels")]
	public GameObject ingamePanel;
	public GameObject pausePanel;
	public GameObject optionsPanel;

    [Header("Options")]
    public float optionsValueMusic;
    public float optionsValueSounds;

    [Header("Children")]
    public GameObject cameraControllerObject;
    public Battleground battlegroundChild;
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

        battlegroundChild = GameObject.Find("Battleground").GetComponent<Battleground>();
        battlegroundChild.transform.SetParent(transform);

        StartGame();
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

    public void SetOptionValueMusic(float newValue)
    {
        if (newValue != optionsValueMusic)
            optionsValueMusic = newValue;
    }

	public void SetOptionValueSounds(float newValue)
	{
		if (newValue != optionsValueSounds)
			optionsValueSounds = newValue;
	}

    public static void ChangeState(States newState)
    {
        if (newState == state)
            return;
        state = newState;
    }

	public static bool IsState(States stateTo)
	{
		if (state == stateTo)
			return true;
		return false;
	}

	public static bool IsGame
	{
		get
		{
			return IsState(States.Game);
		}
	}

	public static bool IsPaused
	{
		get
		{
			switch (state)
			{
				case States.BeforeGame:
					return true;
                case States.PauseMenu:
					return true;
                case States.OptionsMenu:
					return true;
			}
			return false;
		}
	}

    public void PauseGame()
    {
        state = States.PauseMenu;
        Time.timeScale = 0.0f;
        ingamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }

	public void UnPauseGame()
	{
		state = States.Game;
		Time.timeScale = 1.0f;
		ingamePanel.SetActive(true);
		pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
	}

    public void StartGame()
    {
		state = States.Game;
		ingamePanel.SetActive(true);
    }

    public void RestartGame()
    {
        RemoveAllChild();
		pausePanel.SetActive(false);
		optionsPanel.SetActive(false);
        winPoints = 0;
        CreatePlayers();
        StartGame();
    }

    public void RemoveAllChild()
    {
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
    }

    public void OptionsMenu()
    {
        state = States.OptionsMenu;
		pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void GoBackFromOptions()
    {
        state = States.PauseMenu;
		optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
