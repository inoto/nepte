﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour, ITargetable
{
    public bool useAsStartPosition = false;

    public bool isDead = false;
	public LineRenderer lineArrow;

	public GameObject propertyIcon;

	[Header("Cache")]
	public GameObject assignedUIBarObject;
    public UISlider assignedHPbarSlider;
	public UILabel assignedUnitCountLabel;
	
    public RallyPoint rallyPoint;
    public LaserMissile triggeredLaserMissile;
	public List<GameObject> attackers = new List<GameObject>();

    [Header("Modules")]
    public Health health = new Health(4000);
	public CollisionCircle collision;

    [Header("Components")]
	public Transform trans;
	public Owner owner;
	public Spawner spawner;
//    public Body body;
	public Capture capture;
	MeshRenderer mesh;
	MeshFilter mf;
	public Weapon weapon;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject HPbarPrefab;
	

    [Header("Colors")]
    [SerializeField] Material materialNeutral;
	[SerializeField] Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
        spawner = GetComponent<Spawner>();
        owner = GetComponent<Owner>();
//        body = GetComponent<Body>();
	    capture = GetComponent<Capture>();
	    weapon = GetComponent<Weapon>();
    }

	private void Start()
	{
		trans.localScale = Vector3.one;
		
		collision = new CollisionCircle(trans, null, owner, weapon);
		CollisionManager.Instance.AddCollidable(collision);
	}

	void Update()
    {
        if (health.current <= 0)
		{
			Die();
		}
	    if (lineArrow != null)
	    {
		    Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		    point.z = trans.position.z;
		    lineArrow.SetPosition(1, point);
	    }
        //trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

	public void MakeArrow()
	{
		lineArrow = gameObject.AddComponent<LineRenderer>();
		lineArrow.SetPosition(0, trans.position);
		lineArrow.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
		lineArrow.material = (Material)Resources.Load("Arrow");
		lineArrow.widthMultiplier = 3f;
	}

	public void SetOwner(int _playerNumber, PlayerController _playerController)
	{
		owner.playerNumber = _playerNumber;
		owner.playerController = _playerController;

		if (owner.playerController != null)
		{
        	owner.playerController.rallyPoint.DelayedStart();

			AddUIBar();

			spawner.enabled = true;
			spawner.DelayedStart();
			spawner.StartSpawn(trans.position);
		}
		AssignMaterial();
	}
	
	public void PutNearDronesInside()
	{
		List<CollisionCircle> list = CollisionManager.Instance.FindBodiesInCircleArea(trans.position, weapon.radius);
		for (int i = 0; i < list.Count; i++)
		{
			Drone drone = list[i].trans.gameObject.GetComponent<Drone>();
			if (drone != null)
				drone.PutIntoBase();
			spawner.unitCount += 1;
		}
		spawner.UpdateLabel();
	}

    void AddUIBar()
    {
	    Vector2 newPosition = trans.position;
	    newPosition.y += 1;
	    assignedUIBarObject = Instantiate(HPbarPrefab, newPosition, trans.rotation);
	    assignedUIBarObject.transform.SetParent(GameObject.Find("HPBars").transform);
	    assignedUIBarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		//UISprite assignedHPbarSprite = assignedUIBarObject.GetComponent<UISprite>();
		//assignedHPbarSprite.SetAnchor(gameObject);

		assignedHPbarSlider = assignedUIBarObject.transform.GetChild(0).GetComponent<UISlider>();
	    assignedUnitCountLabel = assignedUIBarObject.transform.GetChild(1).GetChild(0).GetComponent<UILabel>();
	    //Debug.Log(assignedHPbarSlider);
    }

	void SetOwnerAsInParent()
	{
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
	}

	void TakeNodes()
	{
		Vector2 tmp = new Vector2(trans.position.x, trans.position.y);
		List<Node> list = Grid.Instance.FindNodesInRadius(tmp, GetComponent<QuadMesh>().size);
		foreach (Node n in list)
		{
			n.ImprisonObject(gameObject);
		}
	}

	void AssignMaterial()
	{
        if (mesh != null && owner != null)
        {
            if (owner.playerNumber < 0)
                mesh.sharedMaterial = materialNeutral;
            else
                mesh.sharedMaterial = materials[owner.playerNumber];
        }
		else
			Debug.LogError("Cannot assign material.");
	}

    void Die()
    {
        CancelInvoke();
	    spawner.StopSpawn();
        //isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, trans.position, trans.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
		//tmpObject.transform.localScale = trans.localScale;
        Destroy(assignedUIBarObject);
        //Destroy(gameObject);
        //gameObject.SetActive(false);
	    SetOwner(-1, null);
	    health.current = health.max;
    }

	public void OnDrawGizmos()
	{
		//if (showRadius)
		//{
		//	Color newColorAgain = Color.green;
		//	newColorAgain.a = 0.5f;
		//	Gizmos.color = newColorAgain;
		//	Gizmos.DrawWireSphere(trans.position, radius);
		//}
	}
	
	public void Damage(Weapon weapon)
	{
		health.current -= weapon.damage;
		if (assignedHPbarSlider != null)
			assignedHPbarSlider.Set((float)health.current / health.max);
		if (health.current <= 0)
		{
			Die();
			weapon.EndCombat();
		}
	}
	
	public void Damage(int damage)
	{
		health.current -= damage;
		if (assignedHPbarSlider != null)
			assignedHPbarSlider.Set((float)health.current / health.max);
		if (health.current <= 0)
		{
			Die();
		}
	}

	public GameObject GameObj
	{
		get { return gameObject; }
	}

	public bool IsDied
	{
		get { return (isDead); }
	}
}
