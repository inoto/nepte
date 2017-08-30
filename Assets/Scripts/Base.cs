using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public bool isStartPosition = false;

    public bool isDead = false;

	public List<Node> node = new List<Node>();

    [Header("Cache")]
    public UISlider assignedHPbarSlider;
    public RallyPoint rallyPoint;
    public LaserMissile triggeredLaserMissile;
	public List<GameObject> attackers = new List<GameObject>();

    [Header("Modules")]
    public Health health = new Health(4000);

    [Header("Components")]
	public Transform trans;
	public Owner owner;
	public Spawner spawner;
    public Body body;
	MeshRenderer mesh;
	MeshFilter mf;

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
        body = GetComponent<Body>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.current <= 0)
		{
			Die();
		}

        //trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

    public void Damage(int damage)
    {
        health.current -= damage;
        assignedHPbarSlider.Set((float)health.current / health.max);
    }

	public void SetOwner(int _playerNumber, PlayerController _playerController)
	{
		owner.playerNumber = _playerNumber;
		owner.playerController = _playerController;

        owner.playerController.rallyPoint.DelayedStart();

        AssignMaterial();

        spawner.enabled = true;
        spawner.DelayedStart();
        spawner.StartSpawn(trans.position);
	}

    void AddHPBar()
    {
		GameObject assignedHPbarObject = Instantiate(HPbarPrefab, trans.position, trans.rotation);
		assignedHPbarObject.transform.SetParent(GameObject.Find("HPBars").transform);
		assignedHPbarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		UISprite assignedHPbarSprite = assignedHPbarObject.GetComponent<UISprite>();
		assignedHPbarSprite.SetAnchor(gameObject);

		assignedHPbarSlider = assignedHPbarObject.GetComponent<UISlider>();
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
        isDead = true;
        GameObject tmpObject = Instantiate(explosionPrefab, trans.position, trans.rotation);
        tmpObject.transform.SetParent(GameController.Instance.transform);
		//tmpObject.transform.localScale = trans.localScale;
        Destroy(assignedHPbarSlider.gameObject);
        //Destroy(gameObject);
        gameObject.SetActive(false);
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
}
