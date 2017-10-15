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
    public RallyPoint rallyPoint;
    public Base bas;
    public AIPlayer aiPlayer;
	
	public List<Base> bases = new List<Base>();

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

	IEnumerator LoseChecker()
	{
		while (true)
		{
			yield return new WaitForSeconds(2.0f);
			if (bases.Count <= 0 && playerUnitCount <= 0)
			{
				if (owner.playerNumber == 0)
					GameController.Instance.Lose();
				else if (owner.playerNumber > 0)
					GameController.Instance.Win();
			}
		}
	}

    public void DelayedStart()
    {
		//CreateBase();

		//CreateRallyPoint();

        if (owner.playerNumber > 0)
			aiPlayer = gameObject.AddComponent<AIPlayer>();
        isInitialized = true;
//	    StartCoroutine(LoseChecker());
    }

    void Update()
    {
		// TODO: rework to use events or something like this
        if (isInitialized)
        {
            //if (bas.isDead)
            //{

            //    if (owner.playerNumber != 0)
            //    {
            //        GameController.Instance.playerControllerObject.Remove(gameObject);
            //        if (GameController.Instance.playerControllerObject.Count <= 1)
            //            GameController.Instance.winPoints += 1;
            //        // define enemies

            //        gameObject.SetActive(false);
            //    }
            //    else
            //        GameController.Instance.winPoints -= 1;
            //}
        }
    }

    public void CreateRallyPoint()
    {
        Vector3 point = trans.position;
        point.z = -0.1f;
		GameObject rallyPointObject = Instantiate(rallyPointPrefab, point, trans.rotation);
        rallyPointObject.transform.SetParent(trans);
        rallyPoint = rallyPointObject.GetComponent<RallyPoint>();
        //rallyPoint.owner = owner;
        //rallyPoint.DelayedStart();

        //rallyPoint = rallyPoint;
    }
}
