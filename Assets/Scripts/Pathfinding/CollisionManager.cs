using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICollidable
{
    Node GetNode();
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

    List<Node> nodeList;
    List<GameObject> unitObjects = new List<GameObject>();

    public void AddAllNodes(List<Node> list)
    {
        nodeList = new List<Node>(list);
    }

    public void AddUnit(GameObject unit)
    {
        unitObjects.Add(unit);
    }

    public void RemoveUnit(GameObject unit)
    {
        unitObjects.Remove(unit);
    }

	public void Update()
	{
        //CheckNodesAndUnitPoints();
        CheckCollisionGrid();
	}

    void CheckCollisionGrid()
    {
        
    }

    void CheckNodesAndUnitPoints()
    {
        foreach (Node node in nodeList)
        {
            foreach (GameObject unit in unitObjects)
            {
                if (node.rect.Contains(unit.transform.position))
                {
                    if (!node.walkable && (node.prisoner != unit))
                    {
                        if (node.prisoner.GetComponent<Drone>().mode != Drone.Mode.Moving)
                        {
                            unit.GetComponent<Unit>().hasCollided = true;
                        }
                        if (unit.GetComponent<Drone>().mode == Drone.Mode.Idle)
                        {
                            unit.GetComponent<Unit>().hasCollided = true;
                        }
                    }
                    else
                    {
                        //bool result = ;
                        if (unit.GetComponent<Unit>().hasNode)
                        {
                            unit.GetComponent<Unit>().node.ReleaseObject();
                        }
                        node.ImprisonObject(unit);
                    }
                }
    //            else
				//{
    //                if (node.prisoner == unit)
    //                {
    //                    node.walkable = true;
    //                    node.ReleaseObject();
    //                }
				//} 
            }
        }
    }
}