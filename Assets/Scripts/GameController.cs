﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	private static GameController _instance;

	public static GameController Instance { get { return _instance; } }

	public delegate void Game();
	public event Game OnGamePaused = delegate { };
    public event Game OnGameContinued = delegate { };
    public event Game OnGameRestart = delegate { };

    public int players = 2;
	public Color[] playerColors;

    public int winPoints = 0;

	public enum States
	{
		BeforeGame,
		Game,
		PauseMenu,
		SettingsMenu,
		Win,
		Lose
	}
    public States state;

    public float gameTimer;

    [Header("UI Panels")]
    public GameObject beforegamePanel;
	public GameObject ingamePanel;
	public GameObject pausePanel;
	public GameObject settingsPanel;
	public GameObject losePanel;
	public GameObject winPanel;

    [Header("Start scene")]
    public GameObject startScene;

    [Header("Children")]
    public Battleground battlegroundChild;
    public MixerContoller audioMixerChild;
	public ObjectPool objectPoolChild;

    [Header("Cache")]
    public List<PlayerController> playerController;
    public List<Vector3> playerStartPosition;
    public List<Base> bases;
	public Dictionary<Base, int> dictBasesOwners = new Dictionary<Base, int>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject playerControllerPrefab;

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

		battlegroundChild = GetComponentInChildren<Battleground>();
		audioMixerChild = GetComponentInChildren<MixerContoller>();
		audioMixerChild = GetComponentInChildren<MixerContoller>();
		objectPoolChild = GetComponentInChildren<ObjectPool>();
	}

    private void Start()
    {
	    //playerColors = new Color[players];
	    
		audioMixerChild.SetMusicVolume(0.75f);
		audioMixerChild.SetSoundsVolume(0.75f);

        BeforeGameMenu();
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsGame)
        {
            gameTimer += Time.deltaTime;

            // TODO: rework to events or something like this
            if (winPoints > 0)
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
    }

    public void ChangeState(States newState)
    {
        if (newState == state)
            return;
        state = newState;
    }

	public bool IsState(States stateTo)
	{
		if (state == stateTo)
			return true;
		return false;
	}

	public bool IsGame
	{
		get
		{
			return IsState(States.Game);
		}
	}

	public bool IsPaused
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
        OnGamePaused();
        ChangeState(States.Lose);
		Time.timeScale = 0.1f;
        ingamePanel.SetActive(false);
        losePanel.SetActive(true);
        StartCoroutine(LoseWinWait());

    }

	public void Win()
	{
        OnGamePaused();
        ChangeState(States.Win);
		Time.timeScale = 0.1f;
		ingamePanel.SetActive(false);
		winPanel.SetActive(true);
        StartCoroutine(LoseWinWait());

	}

    IEnumerator LoseWinWait()
    {
        
        yield return new WaitForSeconds(0.5f);
		if (!IsGame)
		{
            Time.timeScale = 0.0f;
            yield break;
        }
    }

    public void BeforeGameMenu()
    {
        OnGamePaused();
        ChangeState(States.BeforeGame);
        startScene.SetActive(true);
		beforegamePanel.SetActive(true);

    }

    public void PauseGame()
    {
        OnGamePaused();
        state = States.PauseMenu;
        Time.timeScale = 0.0f;
        ingamePanel.SetActive(false);
        pausePanel.SetActive(true);

    }

	public void UnPauseGame()
	{
        OnGameContinued();
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
        Time.timeScale = 1.0f;
        StartGame();
        OnGameRestart();
    }

	public void StartGame()
	{
        if (IsState(States.BeforeGame))
        {
            beforegamePanel.SetActive(false);
            RemoveStartScene();
            audioMixerChild.StopMainTheme();
        }
        OnGameContinued();
		//Camera.main.orthographicSize = 10;
        //GameObject.Find("CameraUIBars").GetComponent<Camera>().orthographicSize = 10;
		//Vector3 vec = playerStartPosition[0];
		//vec.z = -10;
		//Camera.main.transform.position = vec;


		CreatePlayers();
        AssignBases();
		state = States.Game;
		ingamePanel.SetActive(true);

	}

    public void RemoveStartScene()
    {
        Destroy(startScene);
        //RemoveAllHPBars();
    }

    public void RemoveAllPlayerUnits()
    {
        //GameObject audioMixerChild = GameObject.Find("AudioMixerControl");
		foreach (Transform child in transform)
		{
            if (child.gameObject == battlegroundChild.gameObject)
                continue;
            if (child.gameObject == audioMixerChild.gameObject)
				continue;
            if (child.gameObject == objectPoolChild.gameObject)
				continue;
            if (child.gameObject == GameObject.Find("Bases"))
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
        playerController = new List<PlayerController>();

        GameObject playerObject;
		for (int i = -1; i < players; i++)
		{
			playerObject = Instantiate(playerControllerPrefab);
			playerObject.transform.SetParent(transform);
			PlayerController tmpPlayerController = playerObject.GetComponent<PlayerController>();
			tmpPlayerController.owner.playerNumber = i;
			tmpPlayerController.owner.playerController = tmpPlayerController;
			//playerController.owner.color = playerColors[i];
			tmpPlayerController.DelayedStart();

            playerController.Add(tmpPlayerController);
		}
	}

	void AssignBases()
	{
		bases.Clear();
        bases.AddRange(FindObjectsOfType<Base>());
        int counter = 0;
		int counterFull = -1;
        foreach (Base b in bases)
        {
	        if (b.useAsStartPosition)
	        {
		        //playerStartPosition[counter] = b.trans.position;
		        b.type = Base.BaseType.Main;
		        b.SetOwner(counter, playerController[counter+1]);
		        playerController[counter+1].trans.position = b.trans.position;
		        //b.DelayedStart();
		        counter++;
	        }
	        else
	        {
		        b.SetOwner(-1, playerController[0]);
	        }
//	        dictBasesOwners.Add(b,counterFull);
	        counterFull++;
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

    private void OnGUI()
    {
        Rect r = new Rect(Screen.width - 50, 0, Screen.width, 20);
        GUI.Label(r, PlayerController.unitCount.ToString());
    }
}
