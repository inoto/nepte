using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;

	public static GameManager Instance { get { return _instance; } }

	public delegate void Game();
	public event Game OnGamePaused = delegate { };
    public event Game OnGameContinued = delegate { };
    public event Game OnGameRestart = delegate { };
	public event Game OnGameStart = delegate { };

    public int Players = 2;
	public Color[] PlayerColors;

	public enum GameState
	{
		BeforeGame,
		Game,
		PauseMenu,
		SettingsMenu,
		Win,
		Lose
	}
    public GameState State;

    public float GameTimer;

    [Header("UI Panels")]
    [SerializeField] private GameObject beforegamePanel;
	[SerializeField] private GameObject ingamePanel;
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private GameObject losePanel;
	[SerializeField] private GameObject winPanel;

    [Header("Start scene")]
    [SerializeField] private GameObject startScene;

    [Header("Children")]
    [SerializeField] private Battleground battlegroundChild;
	[SerializeField] private MixerContoller audioMixerChild;
	[SerializeField] private ObjectPool objectPoolChild;

    [Header("Cache")]
    public List<PlayerController> PlayerController;
    public List<Planet> Planets;
	public Queue<int> PlayersWithUnassignedPlanets = new Queue<int>();

    [Header("Prefabs")]
    [SerializeField] private GameObject playerControllerPrefab;

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
		audioMixerChild.SetMusicVolume(0.75f);
		audioMixerChild.SetSoundsVolume(0.75f);

        BeforeGameMenu();
    }

    private void Update ()
    {
        if (IsGame)
        {
            GameTimer += Time.deltaTime;
        }
    }

    public void ChangeState(GameState newState)
    {
	    if (newState == State)
	    {
		    return;
	    }
	    State = newState;
    }

	public bool IsState(GameState stateTo)
	{
		if (State == stateTo)
		{
			return true;
		}
		return false;
	}

	public bool IsGame
	{
		get
		{
			return IsState(GameState.Game);
		}
	}

	public bool IsPaused
	{
		get
		{
			switch (State)
			{
				case GameState.BeforeGame:
					return true;
                case GameState.PauseMenu:
					return true;
                case GameState.SettingsMenu:
					return true;
	            default:
		            return false;
			}
			return false;
		}
	}

    public void Lose()
    {
        OnGamePaused();
        ChangeState(GameState.Lose);
		Time.timeScale = 0.1f;
        ingamePanel.SetActive(false);
        losePanel.SetActive(true);
        StartCoroutine(LoseWinWait());

    }

	public void Win()
	{
        OnGamePaused();
        ChangeState(GameState.Win);
		Time.timeScale = 0.1f;
		ingamePanel.SetActive(false);
		winPanel.SetActive(true);
        StartCoroutine(LoseWinWait());

	}

    private IEnumerator LoseWinWait()
    {
        
        yield return new WaitForSeconds(0.5f);
		if (!IsGame)
		{
            Time.timeScale = 0.0f;
            yield break;
        }
    }

    private void BeforeGameMenu()
    {
        OnGamePaused();
        ChangeState(GameState.BeforeGame);
        startScene.SetActive(true);
		beforegamePanel.SetActive(true);

    }

	private void PauseGame()
    {
        OnGamePaused();
        State = GameState.PauseMenu;
        Time.timeScale = 0.0f;
        ingamePanel.SetActive(false);
        pausePanel.SetActive(true);

    }

	private void UnPauseGame()
	{
        OnGameContinued();
		State = GameState.Game;
		Time.timeScale = 1.0f;
		ingamePanel.SetActive(true);
		pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

	}

	private void RestartGame()
    {
        RemoveAllPlayerUnits();
		pausePanel.SetActive(false);
		losePanel.SetActive(false);
        winPanel.SetActive(false);
        GameTimer = 0;
        Time.timeScale = 1.0f;
        StartGame();
        OnGameRestart();
    }

	private void StartGame()
	{
        if (IsState(GameState.BeforeGame))
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
		ingamePanel.SetActive(true);
		State = GameState.Game;
		OnGameStart();
		
	}

    private void RemoveStartScene()
    {
        Destroy(startScene);
    }

    private void RemoveAllPlayerUnits()
    {
        //GameObject audioMixerChild = GameObject.Find("AudioMixerControl");
		foreach (Transform child in transform)
		{
			if (child.gameObject == battlegroundChild.gameObject)
			{
				continue;
			}
			if (child.gameObject == audioMixerChild.gameObject)
			{
				continue;
			};
            if (child.gameObject == objectPoolChild.gameObject)
            {
	            continue;
            }
            if (child.gameObject == GameObject.Find("Planets"))
            {
	            continue;
            }
			Destroy(child.gameObject);
		}
	    foreach (var b in Planets)
	    {
		    b.Trans.DestroyChildren();
	    }
    }

	private void RemoveAllHPBars()
	{
        Transform uiBars = GameObject.Find("UIBars").transform;
		foreach (Transform child in uiBars)
		{
			Destroy(child.gameObject);
		}
	}

	private void CreatePlayers()
	{
        PlayerController = new List<PlayerController>();

        GameObject playerObject;
		for (int i = -1; i < Players; i++)
		{
			playerObject = Instantiate(playerControllerPrefab);
			playerObject.transform.SetParent(transform);
			PlayerController tmpPlayerController = playerObject.GetComponent<PlayerController>();
			tmpPlayerController.Owner.playerNumber = i;
			tmpPlayerController.Owner.playerController = tmpPlayerController;
			//playerController.owner.color = playerColors[i];
			tmpPlayerController.DelayedStart();

            PlayerController.Add(tmpPlayerController);
			if (i != -1)
			{
				PlayersWithUnassignedPlanets.Enqueue(i);
			}
		}
		Planets.Clear();
		Planets.AddRange(FindObjectsOfType<Planet>());
	}

	private void AssignPlanets()
	{
		Planets.Clear();
        Planets.AddRange(FindObjectsOfType<Planet>());
        int counter = 0;
		int counterFull = -1;
        foreach (Planet b in Planets)
        {
	        if (b.UseAsStartPosition)
	        {
		        //playerStartPosition[counter] = b.trans.position;
		        b.Type = Planet.PlanetType.Main;
		        b.SetOwner(counter, PlayerController[counter+1]);
		        PlayerController[counter+1].Trans.position = b.Trans.position;
		        //b.DelayedStart();
		        counter++;
	        }
	        else
	        {
		        b.SetOwner(-1, PlayerController[0]);
	        }
//	        dictBasesOwners.Add(b,counterFull);
	        counterFull++;
        }

	}

	private void SettingsMenu()
    {
        State = GameState.SettingsMenu;
		pausePanel.SetActive(false);
        beforegamePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

	private void GoBackFromSettings()
    {
        State = GameState.PauseMenu;
		settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

	private void CloseGame()
    {
        Application.Quit();
    }

    private void OnGUI()
    {
        Rect r = new Rect(Screen.width - 50, 0, Screen.width, 20);
        GUI.Label(r, global::PlayerController.UnitCount.ToString());
    }
}
