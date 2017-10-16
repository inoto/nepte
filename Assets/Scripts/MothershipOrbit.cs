using UnityEngine;

public class MothershipOrbit : MonoBehaviour
{
    public Owner owner;
    
    [Header("Cache")]
    public UISprite assignedUICircle;
    public Base bas;
    public Mothership mothership;
    public LineRenderer lineArrow;
    
    [Header("Prefabs")]
    public GameObject UICirclePrefab;
    public GameObject mothershipPrefab;

    private void Awake()
    {
        owner = GetComponent<Owner>();
        UICirclePrefab = Resources.Load<GameObject>("UI/MothershipCircleSprite");
        mothershipPrefab = Resources.Load<GameObject>("Units/Mothership");
        bas = transform.parent.GetComponent<Base>();
    }

    private void Start()
    {
        assignedUICircle = Instantiate(UICirclePrefab, transform.position, transform.rotation,
            GameObject.Find("UIBars").transform).GetComponent<UISprite>();
        
    }

    public void DelayedStart()
    {
        mothership =
            Instantiate(mothershipPrefab, transform.position, transform.rotation, owner.playerController.transform)
                .GetComponent<Mothership>();
        mothership.owner.playerController = owner.playerController;
        mothership.owner.playerNumber = owner.playerNumber;
        mothership.bas = bas;
    }

    private void Update()
    {
        if (lineArrow != null)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            lineArrow.SetPosition(1, point);
            lineArrow.SetPosition(0, mothership.trans.position);
        }
    }

    private void OnDestroy()
    {
        if (assignedUICircle != null)
            Destroy(assignedUICircle.gameObject);
    }

    public void AssignToBase(Base newBas)
    {
        if (mothership.mode == Mothership.Mode.MovingNewBase)
            return;
        bas = newBas;
        transform.parent = bas.trans;
        Vector3 newPos = bas.trans.position;
        newPos.z += 0.1f;
        transform.position = newPos;
        assignedUICircle.transform.position = transform.position;
        mothership.mode = Mothership.Mode.MovingNewBase;
        mothership.bas = bas;
    }
    
    public void MakeArrow()
    {
        if (mothership.mode == Mothership.Mode.MovingNewBase)
            return;
        lineArrow = gameObject.AddComponent<LineRenderer>();
        lineArrow.SetPosition(0, transform.position);
        lineArrow.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineArrow.material = (Material)Resources.Load("Arrow");
//        lineArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[mothership.owner.playerNumber]);
        lineArrow.widthMultiplier = 3f;

    }
}
