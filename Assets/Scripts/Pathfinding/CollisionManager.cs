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

    public int queueCount = 0;

    List<NodeGroup> groups = new List<NodeGroup>();
    List<GameObject> objects = new List<GameObject>();
    List<GameObject> movedObjects = new List<GameObject>();

    public void AddGroup(NodeGroup ng)
    {
        groups.Add(ng);
    }

    public void AddUnit(GameObject unit)
    {
        objects.Add(unit);
        queueCount++;
    }

    public void RemoveUnit(GameObject unit)
    {
        //objects.
    }

	public void Update()
	{
        //CheckUnitsInQueue();
        //CheckCollisionGrid();
	}

    void CheckCollisionGrid()
    {
        
    }

    void CheckUnitsInQueue()
    {
        foreach (NodeGroup ng in groups)
        {
            if (objects != null)
            {
                movedObjects.Clear();
                foreach (GameObject unit in objects)
                {
                    if (ng.rect.Contains(unit.transform.position))
                    {
                        ng.units.Add(unit);
                        movedObjects.Add(unit);
                        queueCount--;
                    }
                }
                foreach (GameObject unit in movedObjects)
                {
                    objects.Remove(unit);
                }
            }
            ng.CheckCollisions();
        }
    }
}