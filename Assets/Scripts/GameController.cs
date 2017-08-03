﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameController : MonoBehaviour
{
	private static GameController _instance;

	public static GameController Instance { get { return _instance; } }

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

    public static int players = 3;

    public static int winPoints = 0;

	public enum States
	{
		BeforeGame,
		Game,
		PauseMenu,
		SettingsMenu,
		Win,
		Lose
	}
    public static States state;

    public static float gameTimer;

    [Header("UI Panels")]
    public GameObject beforegamePanel;
	public GameObject ingamePanel;
	public GameObject pausePanel;
	public GameObject settingsPanel;
	public GameObject losePanel;
	public GameObject winPanel;

    [Header("Options")]
    public float optionsValueMusic;
    public float optionsValueSounds;

    [Header("Children")]
    public GameObject cameraControllerObject;
    public Battleground battlegroundChild;
    public GameObject[] playerControllerObject;

    [Header("Cache")]
    public Vector3[] playerStartPosition;

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

        cameraControllerObject.transform.SetParent(transform);

        battlegroundChild = GameObject.Find("Battleground").GetComponent<Battleground>();
        battlegroundChild.transform.SetParent(transform);

        BeforeGameMenu();
    }
    
    // Update is called once per frame
    void Update ()
    {
        gameTimer += Time.deltaTime;

        if (winPoints >= 2)
        {
            Win();
            winPoints = 0;
        }
        else if (winPoints < 0)
        {
            Lose();
            winPoints = 0;
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
                case States.SettingsMenu:
					return true;
			}
			return false;
		}
	}

    public void Lose()
    {
        ChangeState(States.Lose);
		Time.timeScale = 0.1f;
        ingamePanel.SetActive(false);
        losePanel.SetActive(true);
        StartCoroutine(LoseWinWait());
    }

	public void Win()
	{
        ChangeState(States.Win);
		Time.timeScale = 0.1f;
		ingamePanel.SetActive(false);
		winPanel.SetActive(true);
        StartCoroutine(LoseWinWait());
	}

    IEnumerator LoseWinWait()
    {
        if (!IsGame)
        {
            yield return new WaitForSeconds(0.3f);
            Time.timeScale = 0.0f;
        }
    }

    public void BeforeGameMenu()
    {
        ChangeState(States.BeforeGame);
		beforegamePanel.SetActive(true);
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
        settingsPanel.SetActive(false);
	}

    public void RestartGame()
    {
        RemoveAllHPBars();
        RemoveAllPlayerUnits();
		pausePanel.SetActive(false);
		losePanel.SetActive(false);
        winPanel.SetActive(false);
        winPoints = 0;
        gameTimer = 0;
        cameraControllerObject.transform.position = this.transform.position;
        Time.timeScale = 1.0f;
        StartGame();
    }

	public void StartGame()
	{
        if (IsState(States.BeforeGame))
        {
            AssignStartPositions();
            beforegamePanel.SetActive(false);
            RemoveStartScene();
        }
        CreatePlayers();
		state = States.Game;
		ingamePanel.SetActive(true);
	}

    public void RemoveStartScene()
    {
        Destroy(GameObject.Find("StartScene"));
        RemoveAllHPBars();
    }

    public void RemoveAllPlayerUnits()
    {
		foreach (Transform child in transform)
		{
            if (child.gameObject == battlegroundChild.gameObject)
                continue;
			if (child.gameObject == cameraControllerObject)
				continue;
			Destroy(child.gameObject);
		}
    }

	public void RemoveAllHPBars()
	{
        Transform HPbars = GameObject.Find("HPBars").transform;
		foreach (Transform child in HPbars)
		{
			Destroy(child.gameObject);
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
			playerController.transform.position = playerStartPosition[i];
		}
	}

	void AssignStartPositions()
	{
		playerStartPosition = new Vector3[players];

		GameObject tmpObject;
		for (int i = 0; i < players; i++)
		{
			switch (i)
			{
				case 0:
					tmpObject = GameObject.Find("StartPlayer");
					playerStartPosition[i] = tmpObject.transform.position;
					Destroy(tmpObject);
					continue;
				case 1:
					tmpObject = GameObject.Find("StartAIFirst");
					playerStartPosition[i] = tmpObject.transform.position;
					Destroy(tmpObject);
					continue;
				case 2:
					tmpObject = GameObject.Find("StartAISecond");
					playerStartPosition[i] = tmpObject.transform.position;
					Destroy(tmpObject);
					continue;
			}
		}
	}

    public void SettingsMenu()
    {
        state = States.SettingsMenu;
		pausePanel.SetActive(false);
        beforegamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void GoBackFromSettings()
    {
        state = States.PauseMenu;
		settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
