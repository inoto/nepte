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
        nodeRadius = Grid.Instance.nodeRadius;
        groupSizeX = 10;
        groupSizeY = 10;
        groupSize = groupSizeX * groupSizeY;
        nodeDiameter = nodeRadius * 2;
        FillWithNodes();
        CollisionManager.Instance.AddGroup(this);
    }

    public void FillWithNodes()
    {
		float nodeRadius = Grid.Instance.nodeRadius;
		float nodeDiameter = Grid.Instance.nodeDiameter;

        Vector2 point = rect.min;
        point.x += nodeRadius;
        point.y += nodeRadius;

        for (int x = 0; x < groupSizeX; x++)
        {
            for (int y = 0; y < groupSizeY; y++)
            {
                nodes.Add(Grid.Instance.NodeFromWorldPoint(point));
                point.y += nodeDiameter;
            }
            point.y -= Vector2.one.x*10;
            point.x += nodeDiameter;
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
        if (movedUnits.Count > 0)
            Debug.Log("moved units: " + movedUnits.Count);
        movedUnits.Clear();
		foreach (GameObject unit in units)
		{
			if (!rect.Contains(unit.transform.position))
			{
				CollisionManager.Instance.AddUnit(unit);
                movedUnits.Add(unit);
			}
            else
            {
				foreach (Node node in nodes)
				{
					if (node.rect.Contains(unit.transform.position))
					{
						
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
