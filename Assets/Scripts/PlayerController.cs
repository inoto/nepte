using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void Player();
	public event Player OnPlayerDefeated = delegate { };

    public int playerUnitCount = 0;
    public static int unitCount = 0;
    public bool isInitialized = false;
    public bool isDefeated = false;
	

	[Header("Cache")]
	public Transform trans;
    public Owner owner;
    public Planet bas;
    public AIPlayer aiPlayer;
	
	public List<Planet> bases = new List<Planet>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject rallyPointPrefab;

    private void Awake()
    {
        trans = GetComponent<Transform>();
        owner = GetComponent<Owner>();
    }

    private void Start()
    {
        owner.playerController = this;
	    unitCount = 0;
    }

    public void DelayedStart()
    {
        if (owner.playerNumber > 0)
			aiPlayer = gameObject.AddComponent<AIPlayer>();
        isInitialized = true;
    }

}
