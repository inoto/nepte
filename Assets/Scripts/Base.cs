using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public bool showRadius;

    public int health = 4000;
    public float radius;
    public TargetType tType = TargetType.Base;

    public bool isDead = false;

	public List<Node> node = new List<Node>();

    [Header("Cache")]
    public Transform trans;
    public Owner owner;
    public Spawner spawner;
	private MeshRenderer mesh;
	private MeshFilter mf;
    private GameObject playerControllerParent;
    public UISlider assignedHPbarSlider;
    public RallyPoint rallyPoint;
    public LaserMissile triggeredLaserMissile;
	public List<GameObject> attackers = new List<GameObject>();

    [Header("Prefabs")]
    [SerializeField]
    private GameObject explosionPrefab;
    public GameObject HPbarPrefab;

	[Header("Colors")]
	[SerializeField]
	private Material[] materials;

    private void Awake()
    {
		trans = GetComponent<Transform>();
		mesh = GetComponent<MeshRenderer>();
		mf = GetComponent<MeshFilter>();
        spawner = GetComponent<Spawner>();
        owner = GetComponent<Owner>();
    }

    private void Start()
    {
        SetOwnerAsInParent();

        radius = GetComponent<QuadMesh>().size;
		playerControllerParent = transform.parent.gameObject;

		GameObject assignedHPbarObject = Instantiate(HPbarPrefab, trans.position, trans.rotation);
		assignedHPbarObject.transform.SetParent(GameObject.Find("HPBars").transform);
        assignedHPbarObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        UISprite assignedHPbarSprite = assignedHPbarObject.GetComponent<UISprite>();
		assignedHPbarSprite.SetAnchor(gameObject);
		
        assignedHPbarSlider = assignedHPbarObject.GetComponent<UISlider>();

		//TakeNodes();
        spawner.StartSpawn(trans.position);
    }

    void SetOwnerAsInParent()
    {
		var ownerParent = trans.parent.GetComponent<Owner>();
		owner.playerNumber = ownerParent.playerNumber;
		owner.playerController = ownerParent.playerController;
    }

    public void StartWithOwner()
    {
        AssignMaterial();
    }

    // Update is called once per frame
    void Update()
    {
		if (health <= 0)
		{
			Die();
		}

        //trans.Rotate(Vector3.back * ((trans.localScale.x * 10.0f) * Time.deltaTime));
    }

    public void Damage(int damage)
    {
		health -= damage;
        assignedHPbarSlider.Set((float)health / 4000);
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
			mesh.sharedMaterial = materials[owner.playerNumber];
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
		if (showRadius)
		{
			Color newColorAgain = Color.green;
			newColorAgain.a = 0.5f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(trans.position, radius);
		}
	}
}
