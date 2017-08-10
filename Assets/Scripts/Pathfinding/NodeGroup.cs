using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGroup
{
	public Rect rect;

    public List<Node> nodes = new List<Node>();
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> movedUnits = new List<GameObject>();

    public int groupSizeX, groupSizeY;
    public int groupSize;

    public float nodeRadius;
    public float nodeDiameter;
   

    public NodeGroup(Rect newRect)
	{
		rect = newRect;
        nodeRadius = 0.05f;
        groupSizeX = 10;
        groupSizeY = 10;
        groupSize = groupSizeX * groupSizeY;
        nodeDiameter = nodeRadius * 2;
        FillWithNodes();
        CollisionManager.Instance.AddGroup(this);
    }

    public void FillWithNodes()
    {
        Vector2 point = rect.min;
        point.x += 0.05f;
        point.y += 0.05f;

        for (int x = 0; x < groupSizeX; x++)
        {
            for (int y = 0; y < groupSizeY; y++)
            {
                nodes.Add(Grid.Instance.NodeFromWorldPoint(point));
                point.y += 0.1f;
            }
            point.y -= Vector2.one.x;
            point.x += 0.1f;
        }
    }

	void AddAllNodes(List<Node> list)
	{
		//nodes = new List<Node>(list);
	}

	void AddUnit(GameObject unit)
	{
		units.Add(unit);
	}

	public void CheckCollisions()
	{
		foreach (Node node in nodes)
		{
            movedUnits.Clear();
			foreach (GameObject unit in units)
			{
                if (!rect.Contains(unit.transform.position))
                {
                    CollisionManager.Instance.AddUnit(unit);
                    movedUnits.Remove(unit);
                }
                else
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
                }
			}
            foreach (GameObject unit in movedUnits)
            {
                units.Remove(unit);
            }
		}
	}
}
