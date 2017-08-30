using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public QuadTreeEternal qtree;

    List<ICollidable> units = new List<ICollidable>();
    List<ICollidable> bases = new List<ICollidable>();
    List<ICollidable> radars = new List<ICollidable>();
    List<ICollidable> weapons = new List<ICollidable>();

    List<ICollidable> returnedUnits = new List<ICollidable>();
    List<ICollidable> returnedBases = new List<ICollidable>();
	List<ICollidable> returnedRadars = new List<ICollidable>();
	List<ICollidable> returnedWeapons = new List<ICollidable>();

    public List<ICollidable> objects = new List<ICollidable>();

	private void Start()
	{
        Rect rect = new Rect();
		rect.min = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.min;
		rect.max = GameObject.Find("Battleground").GetComponent<MeshRenderer>().bounds.max;

		qtree = new QuadTreeEternal(rect);
	}

	public void AddCollidable(ICollidable obj)
	{
        qtree.Insert(obj);
	}

	//public void RemoveCollidable(ICollidable obj)
	//{
 //       //Debug.Log(obj.Type + " removed");
	//	switch (obj.collisionType)
	//	{
	//		case CollisionType.Drone:
	//			units.Remove(obj);
	//			break;
	//		case CollisionType.Base:
	//			bases.Remove(obj);
	//			break;
	//		case CollisionType.Radar:
	//			radars.Remove(obj);
	//			break;
	//		case CollisionType.Weapon:
	//			weapons.Remove(obj);
	//			break;
	//	}

	//}

    // update for QuadTreeEternal
    private void Update()
    {
        qtree.Update();

   // 	foreach (CollisionRecord cr in QuadTreeEternal.crList)
   // 	{
   //         if (cr.collided1.collisionType == CollisionType.Body && cr.collided2.collisionType == CollisionType.Body)
   // 		{
   //             // союзника добавить в cohasion & align
   //             if (cr.collided1.Owner == cr.collided2.Owner)
   //             {
   //                 cr.collided1.
   //             }
			//	// любого добавить в separation
			//	if (mover == unit)
	  //              continue;
		 //       float dist = (mover.trans.position - unit.trans.position).magnitude;
		 //       if (dist > 0 && dist < desiredSeparation)
		 //       {
		 //           Vector2 diff = (mover.trans.position - unit.trans.position).normalized;
		 //           diff /= dist;
		 //           sum += diff;
		 //           countSeparation++;
		 //       }
			//}
    	//	if (cr.CollidedObject.Type == CollisionType.Radar && cr.OtherCollidedObject.Type == CollisionType.Drone)
    	//	{
    	//		if (cr.CollidedObject.drone.owner != cr.OtherCollidedObject.drone.owner)
    	//			if (cr.CollidedObject.drone.target == null)
    	//				cr.CollidedObject.drone.EnterCombatMode(cr.OtherCollidedObject.drone);
    	//	}
    	//	if (cr.CollidedObject.Type == CollisionType.Drone && cr.OtherCollidedObject.Type == CollisionType.Radar)
    	//	{
    	//		if (cr.CollidedObject.drone.owner != cr.OtherCollidedObject.drone.owner)
    	//			if (cr.OtherCollidedObject.drone.target == null)
    	//				cr.OtherCollidedObject.drone.EnterCombatMode(cr.CollidedObject.drone);
    	//	}
    	//	//
    	//	//if (cr.OtherCollidedObject != null)
    	//	//cr.OtherCollidedObject.unit.trans.position = (Vector2)cr.OtherCollidedObject.unit.trans.position + cr.VectorsSubtraction * 0.03f;
    	//	//cr.CollidedObject.GetGameobject().GetComponent<Weapon>().ReleaseLaserMissile(cr.OtherCollidedObject.GetPoint());
    	//}
    	//QuadTreeEternal.crList.Clear();
    }

    //private void Update()
    //{
    //      unitsInTree = units.Count;
    //      basesInTree = bases.Count;
    //      radarsInTree = radars.Count;
    //      weaponsInTree = weapons.Count;
    //qtree.Clear();

    //      AddCollidablesToTree();

    //      // units
    //      for (int i = 0; i < units.Count; i++)
    //      {
    //          returnedUnits.Clear();
    //          qtree.Retrieve(returnedUnits, units[i]);
    //          for (int u = 0; u < returnedUnits.Count; u++)
    //          {
    //              if (returnedUnits[u] != units[i])
    //              {
    //                  float dist = (units[i].Point - returnedUnits[u].Point).sqrMagnitude;
    //                  //CheckCollisionsUnits(units[i], returnedUnits[u], dist);
    //                  //CheckCollisionsUnitRadar(units[i], returnedUnits[u], dist);
    //                  //CheckCollisionsUnitWeapon(units[i], returnedUnits[u], dist);
    //              }
    //          }
    //      }
    //for (int i = 0; i < radars.Count; i++)
    //{
    //	returnedUnits.Clear();
    //	qtree.Retrieve(returnedUnits, radars[i]);
    //	for (int u = 0; u < returnedUnits.Count; u++)
    //	{
    //		if (returnedUnits[u] != radars[i])
    //		{
    //			float dist = (returnedUnits[u].Point - radars[i].Point).sqrMagnitude;
    //			//CheckCollisionsUnitRadar(returnedUnits[u], radars[i], dist);
    //			//CheckCollisionsUnitWeapon(returnedUnits[u], radars[i], dist);
    //		}
    //	}
    //}
    //for (int i = 0; i < weapons.Count; i++)
    //{
    //	returnedUnits.Clear();
    //	qtree.Retrieve(returnedUnits, weapons[i]);
    //	for (int u = 0; u < returnedUnits.Count; u++)
    //	{
    //		if (returnedUnits[u] != weapons[i])
    //		{
    //			float dist = (returnedUnits[u].Point - weapons[i].Point).sqrMagnitude;
    //			//CheckCollisionsUnitRadar(returnedUnits[u], weapons[i], dist);
    //			//CheckCollisionsUnitWeapon(returnedUnits[u], weapons[i], dist);
    //		}
    //	}
    //}
    //for (int i = 0; i < bases.Count; i++)
    //{
    //	returnedUnits.Clear();
    //	qtree.Retrieve(returnedUnits, bases[i]);
    //	for (int u = 0; u < returnedUnits.Count; u++)
    //	{
    //		if (returnedUnits[u] != bases[i])
    //		{
    //			float dist = (bases[i].Point - returnedUnits[u].Point).sqrMagnitude;
    //			//CheckCollisionsBaseRadar(bases[i], returnedUnits[u], dist);
    //			//CheckCollisionsBaseWeapon(bases[i], returnedUnits[u], dist);
    //		}
    //	}
    //}

    //         for (int r = 0; r < radars.Count; r++)
    //         {
    //             if (returnedUnits[rr] != units[i] && returnedUnits[rr].drone != units[i].drone)
    //             {
    //                 float dist = (units[i].Point - returnedUnits[rr].Point).sqrMagnitude;
    //                 CheckCollisionsUnitRadar(units[i], returnedUnits[rr], dist);
    //             }
    //		//for (int b = 0; b < bases.Count; b++)
    //		//{
    //		//	if (returnedRadars[rr] != bases[i])
    //  //          {
    //  //              float dist = (bases[i].Point - returnedRadars[rr].Point).sqrMagnitude;
    //  //              CheckCollisionsUnitRadar(bases[i], returnedRadars[rr], dist);
    //  //          }
    //		//}
    //         }
    //         for (int w = 0; w < weapons.Count; w++)
    //{
    //	for (int rw = 0; rw < returnedWeapons.Count; rw++)
    //	{
    //		if (returnedWeapons[rw] != units[i] && returnedWeapons[rw].drone != units[i].drone)
    //		{
    //			float dist = (units[i].Point - returnedWeapons[rw].Point).sqrMagnitude;
    //			CheckCollisionsUnitWeapon(units[i], returnedWeapons[rw], dist);
    //		}
    //	}
    //}
    //for (int b = 0; b < bases.Count; b++)
    //{
    //	returnedBases.Clear();
    //	qtree.Retrieve(returnedBases, bases[b]);
    //	for (int rb = 0; rb < returnedBases.Count; rb++)
    //	{
    //		if (returnedBases[rb] != units[i] && returnedBases[rb].drone != units[i].drone)
    //		{
    //			float dist = (units[i].Point - returnedBases[rb].Point).sqrMagnitude;
    //			CheckCollisionsUnitBase(units[i], returnedBases[rb], dist);
    //		}
    //	}
    //}

    // bases
    //for (int i = 0; i < bases.Count; i++)
    //{
    //	returnedBases.Clear();
    //          qtree.Retrieve(returnedBases, bases[i]);
    //	for (int r = 0; r < radars.Count; r++)
    //	{
    //		returnedRadars.Clear();
    //		qtree.Retrieve(returnedRadars, radars[r]);
    //		for (int rr = 0; rr < returnedRadars.Count; rr++)
    //		{
    //			if (returnedRadars[rr] != bases[i])
    //			{
    //				float dist = (bases[i].Point - returnedRadars[rr].Point).sqrMagnitude;
    //				CheckCollisionsBaseRadar(bases[i], returnedRadars[rr], dist);
    //			}
    //		}
    //	}
    //	for (int w = 0; w < weapons.Count; w++)
    //	{
    //		returnedWeapons.Clear();
    //		qtree.Retrieve(returnedWeapons, weapons[w]);
    //		for (int rw = 0; rw < returnedWeapons.Count; rw++)
    //		{
    //			if (returnedWeapons[rw] != bases[i])
    //			{
    //				float dist = (bases[i].Point - returnedWeapons[rw].Point).sqrMagnitude;
    //				CheckCollisionsBaseWeapon(bases[i], returnedWeapons[rw], dist);
    //			}
    //		}
    //	}
    //}
//}
    /*
    void CheckCollisionsUnits(ICollidable unit1, ICollidable unit2, float dist)
	{
        if (unit1.collisionType == CollisionType.Drone && unit2.collisionType == CollisionType.Drone)
        {
            float radiusesHard = unit1.RadiusHard + unit2.RadiusHard;
            float radiuses = unit1.Radius + unit2.Radius;
			if (dist < radiusesHard * radiusesHard)
			{
				unit1.drone.trans.position += (unit1.drone.trans.position - unit2.drone.trans.position) * 0.03f;
				unit2.drone.trans.position += (unit2.drone.trans.position - unit1.drone.trans.position) * 0.03f;
            }
            if (dist < radiuses * radiuses)
            {

				unit1.drone.trans.position += (unit1.drone.trans.position - unit2.drone.trans.position) * 0.02f;
				unit2.drone.trans.position += (unit2.drone.trans.position - unit1.drone.trans.position) * 0.02f;
            }
        }
	}
	void CheckCollisionsUnitRadar(ICollidable unit1, ICollidable unit2, float dist)
	{
		if (unit1 == null || unit2 == null)
			return;
		if (unit1.collisionType == CollisionType.Drone && unit2.collisionType == CollisionType.Radar)
		{
            if (unit1.drone.owner != unit2.drone.owner)
            {
                // if drone has no target
                if (unit2.drone.target == null)
                {
                    float radiuses = unit1.Radius + unit2.Radius;
                    if (dist < radiuses * radiuses)
                    {
                        unit2.drone.EnterCombatMode(unit1.drone);
                    }
                }
            }
		}
	}
	void CheckCollisionsUnitWeapon(ICollidable unit1, ICollidable unit2, float dist)
	{
        if (unit1 == null || unit2 == null)
            return;
		if (unit1.collisionType == CollisionType.Drone && unit2.collisionType == CollisionType.Weapon)
		{
            if (unit2.drone.target == unit1 && unit2.drone.mode != Drone.Mode.Attacking)
			{
				float radiuses = unit1.Radius + unit2.Radius;
				if (dist < radiuses * radiuses)
				{
					unit2.drone.EnterAttackingMode();
				}
			}
		}
	}
	void CheckCollisionsUnitBase(ICollidable unit1, ICollidable unit2, float dist)
	{
		if (unit1 == null || unit2 == null)
			return;
		if (unit1.collisionType == CollisionType.Drone && unit2.collisionType == CollisionType.Base)
		{
			float radiuses = unit1.Radius + unit2.Radius;
			if (dist < radiuses * radiuses)
			{
                Vector3 vec = unit1.drone.trans.position - unit2.bas.trans.position;
                vec.z = 0;
				unit1.drone.trans.position += vec * 0.02f;
			}
		}
	}
	void CheckCollisionsBaseRadar(ICollidable unit1, ICollidable unit2, float dist)
	{
		if (unit1 == null || unit2 == null)
			return;
		if (unit1.collisionType == CollisionType.Base && unit2.collisionType == CollisionType.Radar)
		{
			if (unit1.bas.owner != unit2.drone.owner && unit2.drone.target == null)
			{
				float radiuses = unit1.Radius + unit2.Radius;
				if (dist < radiuses * radiuses)
				{
                    
                    unit2.drone.EnterCombatMode(unit1.bas);
				}
			}
		}
	}
	void CheckCollisionsBaseWeapon(ICollidable unit1, ICollidable unit2, float dist)
	{
		if (unit1 == null || unit2 == null)
		    return;
		if (unit1.collisionType == CollisionType.Base && unit2.collisionType == CollisionType.Weapon)
		{
            if (unit1.bas.owner != unit2.drone.owner && unit2.drone.target == unit1 && unit2.drone.mode != Drone.Mode.Attacking)
			{
				float radiuses = unit1.Radius + unit2.Radius;
				if (dist < radiuses * radiuses)
				{
					unit2.drone.EnterAttackingMode();
				}
			}
		}
	}
    */

  //  void AddCollidablesToTree()
  //  {
		//for (int i = 0; i < units.Count; i++)
		//{
		//	qtree.Insert(units[i]);
		//}
		//for (int i = 0; i < radars.Count; i++)
		//{
		//	qtree.Insert(radars[i]);
		//}
		//for (int i = 0; i < weapons.Count; i++)
		//{
		//	qtree.Insert(weapons[i]);
		//}
		//for (int i = 0; i < bases.Count; i++)
		//{
		//	qtree.Insert(bases[i]);
		//}
		
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