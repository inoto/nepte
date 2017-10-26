using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Not used anymore",false)]
public class NodeGroup
{
	private Rect rect;

	private readonly List<Node> nodes = new List<Node>();
	private readonly List<GameObject> units = new List<GameObject>();
    public List<GameObject> MovedUnits = new List<GameObject>();

	private readonly int groupSizeX;
	private readonly int groupSizeY;
	private int groupSize;

	public readonly float NodeRadius;
	public float NodeDiameter;
   

    public NodeGroup(Rect newRect)
	{
		rect = newRect;
        NodeRadius = Grid.Instance.NodeRadius;
        groupSizeX = 10;
        groupSizeY = 10;
        groupSize = groupSizeX * groupSizeY;
        NodeDiameter = NodeRadius * 2;
        FillWithNodes();
        //CollisionManager.Instance.AddGroup(this);
    }

	private void FillWithNodes()
    {
		float nodeRadius = Grid.Instance.NodeRadius;
		float nodeDiameter = Grid.Instance.NodeDiameter;

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

	private void AddUnit(GameObject unit)
	{
		units.Add(unit);
	}

	//public void CheckCollisions()
	//{
 //       if (movedUnits.Count > 0)
 //           Debug.Log("moved units: " + movedUnits.Count);
 //       movedUnits.Clear();
	//	foreach (GameObject unit in units)
	//	{
	//		if (!rect.Contains(unit.transform.position))
	//		{
	//			CollisionManager.Instance.AddUnit(unit);
 //               movedUnits.Add(unit);
	//		}
 //           else
 //           {
	//			foreach (Node node in nodes)
	//			{
	//				if (node.rect.Contains(unit.transform.position))
	//				{
						
	//				}
	//			}
 //           }

	//	}
	//	foreach (GameObject unit in movedUnits)
	//	{
	//		units.Remove(unit);
	//	}
	//}
}
