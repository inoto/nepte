using UnityEngine;

public class MothershipOrbit : MonoBehaviour
{
    public Owner Owner;
    
    [Header("Cache")]
    public UISprite AssignedUICircle;
    public Planet Planet;
    public Mothership Mothership;
    public LineRenderer LineRendererArrow;
    
    [Header("Prefabs")]
    private GameObject UICirclePrefab;
    private GameObject mothershipPrefab;

    private void Awake()
    {
        Owner = GetComponent<Owner>();
        UICirclePrefab = Resources.Load<GameObject>("UI/MothershipCircleSprite");
        mothershipPrefab = Resources.Load<GameObject>("Units/Mothership");
        Planet = transform.parent.GetComponent<Planet>();
    }

    private void Start()
    {
        AssignedUICircle = Instantiate(UICirclePrefab, transform.position, transform.rotation,
            GameObject.Find("UIBars").transform).GetComponent<UISprite>();
        
    }

    public void DelayedStart()
    {
        Mothership =
            Instantiate(mothershipPrefab, transform.position, transform.rotation, Owner.playerController.transform)
                .GetComponent<Mothership>();
        Mothership.Owner.playerController = Owner.playerController;
        Mothership.Owner.playerNumber = Owner.playerNumber;
        Mothership.Planet = Planet;
    }

    private void Update()
    {
        if (LineRendererArrow != null)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = transform.position.z;
            LineRendererArrow.SetPosition(1, point);
            LineRendererArrow.SetPosition(0, Mothership.Trans.position);
        }
    }

    private void OnDestroy()
    {
        if (AssignedUICircle != null)
        {
            Destroy(AssignedUICircle.gameObject);
        }
    }

    public void AssignToBase(Planet newBas)
    {
        if (Mothership.Mode == Mothership.MothershipMode.MovingNewBase)
        {
            return;
        }
        Planet = newBas;
        transform.parent = Planet.Trans;
        Vector3 newPos = Planet.Trans.position;
        newPos.z += 0.1f;
        transform.position = newPos;
        AssignedUICircle.transform.position = transform.position;
        Mothership.Mode = Mothership.MothershipMode.MovingNewBase;
        Mothership.Planet = Planet;
    }
    
    public void MakeArrow()
    {
        if (Mothership.Mode == Mothership.MothershipMode.MovingNewBase)
        {
            return;
        }
        LineRendererArrow = gameObject.AddComponent<LineRenderer>();
        LineRendererArrow.SetPosition(0, transform.position);
        LineRendererArrow.SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        LineRendererArrow.material = (Material)Resources.Load("Arrow");
//        lineArrow.material.SetColor("_TintColor", GameController.Instance.playerColors[mothership.owner.playerNumber]);
        LineRendererArrow.widthMultiplier = 3f;
    }
    
}
