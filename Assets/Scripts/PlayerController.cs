using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public delegate void Player();
	public event Player OnPlayerDefeated = delegate { };

    public int PlayerUnitCount = 0;
    public static int UnitCount = 0;

	[Header("Cache")]
	public Transform Trans;
    public Owner Owner;
    public Planet Planet;
    public AIPlayer AiPlayer;
	
	public List<Planet> Planets = new List<Planet>();

    [Header("Prefabs")]
    [SerializeField] private GameObject planetPrefab;

    private void Awake()
    {
        Trans = GetComponent<Transform>();
        Owner = GetComponent<Owner>();
    }

    private void Start()
    {
        Owner.PlayerController = this;
	    UnitCount = 0;
    }

    public void DelayedStart()
    {
	    if (Owner.PlayerNumber > 0)
	    {
		    AiPlayer = gameObject.AddComponent<AIPlayer>();
	    }
    }

}
