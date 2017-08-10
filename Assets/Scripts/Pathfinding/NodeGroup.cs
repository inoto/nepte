using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGroup
{
	public Rect rect;

    Node[,] nodes;
	List<GameObject> units;

    public int groupSizeX, groupSizeY;
    public int groupSize;

    Vector3 penVector;

	public NodeGroup(Rect newRect, CollisionGrid newParent)
	{
		rect = newRect;
		//parent = newParent;
        groupSizeX = Mathf.RoundToInt(rect.width / 0.1f);
		groupSizeY = Mathf.RoundToInt(rect.height / 0.1f);
        groupSize = groupSizeX * groupSizeY;
	}

	public List<Node> GetNodesAsList()
	{
		List<Node> list = new List<Node>();
		for (int x = 0; x < groupSizeX; x++)
		{
			for (int y = 0; y < groupSizeY; y++)
			{
				list.Add(nodes[x, y]);
			}
		}
		return list;
	}

    void CreateGroup()
    {
		nodes = new Node[groupSizeX, groupSizeY];

		for (int x = 0; x < groupSizeX; x++)
		{
			for (int y = 0; y < groupSizeY; y++)
			{
				bool walkable = !(Physics.CheckSphere(penVector, nodeRadius, unwalkableMask));

				int movementPenalty = 0;


				Ray ray = new Ray(penVector + Vector3.up * 50, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 100, walkableMask))
				{
					walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
				}

				if (!walkable)
				{
					movementPenalty += obstacleProximityPenalty;
				}


				nodes[x, y] = new Node(walkable, penVector, x, y, movementPenalty);

				penVector.y += nodeDiameter;
			}
			penVector.y = battlegroundRenderer.bounds.min.y + 0.05f;
			penVector.x += nodeDiameter;
		}
    }

	void AddAllNodes(List<Node> list)
	{
		nodes = new List<Node>(list);
	}

	void AddUnit(GameObject unit)
	{
		units.Add(unit);
	}

	public void CheckCollisions()
	{
		foreach (Node node in nodes)
		{
			foreach (GameObject unit in units)
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
	}
}
