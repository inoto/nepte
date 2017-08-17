using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollidableUnit
{
    Drone GetDrone();
	Drone.Mode GetMode();
	Vector2 GetPoint();
	float GetRadiusSoft();
    float GetRadiusHard();
	GameObject GetGameobject();
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

	public QuadTreeNode qtree;

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

		qtree = new QuadTreeNode(rect);
        //qtree.Split();
	}

    public void AddUnit(CollisionCircle unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(CollisionCircle unit)
    {
        units.Remove(unit);
    }

	private void Update()
	{
		qtree.Clear();
        //Debug.Log("total units: " + units.Count);
		for (int i = 0; i < units.Count; i++)
		{
            
			qtree.Insert(units[i]);
		}

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
			//for (int k = 0; k < Bases.Count; k++)
			//{
			//	float dist = Vector2.Distance(units[i].GetPoint(), Bases[k].transform.position);
			//	//CollisionAttackOrResponse(units[i], Bases[k], dist);
			//}
		}
	}

	void CheckCollisionsUnits(CollisionCircle unit, CollisionCircle enemy, float dist)
	{
        float radiusesHard = unit.radiusHard + enemy.radiusHard;
        float radiuses = unit.radius + enemy.radius;
        if (dist < radiusesHard * radiusesHard)
		{
			
		}
        else if (dist < radiuses * radiuses)
		{
            if (unit.unit.droneComponent.owner != enemy.unit.droneComponent.owner)
                unit.unit.weaponComponent.ReleaseLaserMissile(enemy.point);
		}
	}

	//void CollisionAttackOrResponse(ICollidableUnit unit, ICollidableUnit enemy, float dist)
	//{
	//	if (dist <= unit.ResponseRadus + enemy.HardRadius && unit.side != enemy.Side)
	//	{
	//		if (dist <= unit.attackRadius + enemy.HardRadius)
	//		{
				
	//			unit.Attack(enemy);
	//		}
	//		else if (unit.State != UnitState.ResponseTrigger && unit.State != UnitState.Attack)
	//		{
				
	//			unit.TriggerOnEnemy(enemy);
	//		}
	//	}
	//}

    public bool RectIntersectsWithCircle(Rect rect, float halfW, float halfH, CollisionCircle circle)
	{
        float closestX = Mathf.Clamp(circle.point.x, rect.x, rect.width);
        float closestY = Mathf.Clamp(circle.point.y, rect.height, rect.y);

		// Calculate the distance between the circle's center and this closest point
		float distanceX = circle.point.x - closestX;
		float distanceY = circle.point.y - closestY;

		// If the distance is less than the circle's radius, an intersection occurs
		float distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);
        return distanceSquared < (circle.radius * circle.radius);
	}

	//Debug view for QuadTreeNode
	void OnDrawGizmos()
	{
		if (showGizmos)
			if (qtree != null)
				qtree.DrawDebug();
	}
}