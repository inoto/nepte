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

	public void AddBase(CollisionCircle bas)
	{
		bases.Add(bas);
	}

	public void AddRadar(CollisionCircle radar)
	{
		radars.Add(radar);
	}

	public void AddWeapon(CollisionCircle weapon)
	{
		weapons.Add(weapon);
	}

    public void RemoveUnit(CollisionCircle unit)
    {
        units.Remove(unit);
    }

	public void RemoveBase(CollisionCircle bas)
	{
		bases.Remove(bas);
	}

	public void RemoveRadar(CollisionCircle radar)
	{
		radars.Remove(radar);
	}

	public void RemoveWeapon(CollisionCircle weapon)
	{
		weapons.Remove(weapon);
	}

	private void Update()
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
	}


	void CheckCollisionsUnits(CollisionCircle unit1, CollisionCircle unit2, float dist)
	{
        float radiusesHard = unit1.radiusHard + unit2.radiusHard;
        float radiuses = unit1.radius + unit2.radius;
        //if (dist < radiusesHard * radiusesHard)
        //{

        //}
        if (dist < radiuses * radiuses)
        {
            unit1.unit.trans.position += (unit1.unit.trans.position - unit2.unit.trans.position) * 0.03f;
            unit2.unit.trans.position += (unit2.unit.trans.position - unit1.unit.trans.position) * 0.03f;
            //if (unit.unit.droneComponent.owner != enemy.unit.droneComponent.owner)
            //unit.unit.weaponComponent.ReleaseLaserMissile(enemy.point);
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