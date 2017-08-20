using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionType
{
	Drone,
	Base,
	Radar,
	Weapon
}

public interface ICollidable
{
	int InstanceId { get; }
	Vector2 Point { get; set; }
	float Radius { get; }
    float RadiusHard { get; }
	CollisionType Type { get; }
	bool Active { get; }
	Drone drone { get; }
	GameObject GameObject { get; }
}

public class CollisionManager : MonoBehaviour
{
    private static CollisionManager _instance;

    public static CollisionManager Instance { get { return _instance; } }

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
    }

	public bool showGizmos;
	public int objectsInTree = 0;

	//public QuadTreeNode qtree;
	public QuadTreeEternal qtree;

    List<CollisionCircle> units = new List<CollisionCircle>();
    List<CollisionCircle> bases = new List<CollisionCircle>();
    List<CollisionCircle> radars = new List<CollisionCircle>();
    List<CollisionCircle> weapons = new List<CollisionCircle>();

    List<CollisionCircle> returnedUnits = new List<CollisionCircle>();
    List<CollisionCircle> returnedBases = new List<CollisionCircle>();
	List<CollisionCircle> returnedRadars = new List<CollisionCircle>();
	List<CollisionCircle> returnedWeapons = new List<CollisionCircle>();

	private void Start()
	{
        Rect rect = new Rect();
		rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
		rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

		//qtree = new QuadTreeNode(rect);
		//qtree.Split();
		qtree = new QuadTreeEternal(rect);
	}

    //public void AddUnit(CollisionCircle unit)
    //{
    //    units.Add(unit);
    //}

	public void AddCollidable(ICollidable obj)
	{
		qtree.Insert(obj);
	}

	public void RemoveCollidable(ICollidable obj)
	{
		qtree.Remove(obj);
	}

	private void Update()
	{
		qtree.Update();

		foreach (CollisionRecord cr in QuadTreeEternal.crList)
		{
			if (cr.CollidedObject.Type == CollisionType.Drone && cr.OtherCollidedObject.Type == CollisionType.Drone)
			{
				cr.CollidedObject.Point += (cr.CollidedObject.Point - cr.OtherCollidedObject.Point) * 0.02f;
				cr.OtherCollidedObject.Point += (cr.OtherCollidedObject.Point - cr.CollidedObject.Point) * 0.02f;
			}
			if (cr.CollidedObject.Type == CollisionType.Radar && cr.OtherCollidedObject.Type == CollisionType.Drone)
			{
				if (cr.CollidedObject.drone.owner != cr.OtherCollidedObject.drone.owner)
					if (cr.CollidedObject.drone.target == null)
						cr.CollidedObject.drone.EnterCombatMode(cr.OtherCollidedObject.drone);
			}
			if (cr.CollidedObject.Type == CollisionType.Drone && cr.OtherCollidedObject.Type == CollisionType.Radar)
			{
				if (cr.CollidedObject.drone.owner != cr.OtherCollidedObject.drone.owner)
					if (cr.OtherCollidedObject.drone.target == null)
						cr.OtherCollidedObject.drone.EnterCombatMode(cr.CollidedObject.drone);
			}
			//
			//if (cr.OtherCollidedObject != null)
			//cr.OtherCollidedObject.unit.trans.position = (Vector2)cr.OtherCollidedObject.unit.trans.position + cr.VectorsSubtraction * 0.03f;
			//cr.CollidedObject.GetGameobject().GetComponent<Weapon>().ReleaseLaserMissile(cr.OtherCollidedObject.GetPoint());
		}
		QuadTreeEternal.crList.Clear();
	}

	/*private void Update()
	{
		qtree.Clear();
        //Debug.Log("total units: " + units.Count);
		for (int i = 0; i < units.Count; i++)
		{
			qtree.Insert(units[i]);
		}
		//for (int i = 0; i < radars.Count; i++)
		//{
		//	qtree.Insert(radars[i]);
		//}

		for (int i = 0; i < units.Count; i++)
		{
            returnedUnits.Clear();
			qtree.Retrieve(returnedUnits, units[i]);
			for (int k = 0; k < returnedUnits.Count; k++)
			{
				if (returnedUnits[k] != units[i])
				{
                    float dist = (units[i].point - returnedUnits[k].point).sqrMagnitude;
					CheckCollisionsUnits(units[i], returnedUnits[k], dist);
					//CollisionAttackOrResponse(units[i], returnedUnits[k], dist);
				}
			}
            //returnedRadars.Clear();
            //qtree.Retrieve(returnedRadars, radars[i]);
			//for (int k = 0; k < Bases.Count; k++)
			//{
			//	float dist = Vector2.Distance(units[i].GetPoint(), Bases[k].transform.position);
			//	//CollisionAttackOrResponse(units[i], Bases[k], dist);
			//}
		}
	}*/

	//Debug view for QuadTreeNode
	void OnDrawGizmos()
	{
        if (showGizmos)
        {
            if (qtree != null)
                qtree.DrawDebug();
        }
	}
}