﻿using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
    public Rect rect;
    public float size;
    public GameObject prisoner;
	public int movementPenalty;
	public Node[] neigbours = new Node[8];

	public int[] distance = new int[GameController.Instance.players];
	public bool[] suitable = new bool[GameController.Instance.players];

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;
	
	public Node(Rect newRect, float newSize, int _gridX, int _gridY)
    {
        walkable = true;
        rect = newRect;
        worldPosition = new Vector2(rect.center.x, rect.center.y);
		gridX = _gridX;
		gridY = _gridY;
        size = newSize;
		for (int i = 0; i < distance.Length; i++)
		{
			distance[i] = 0;
			suitable[i] = false;
		}
		neigbours = Grid.Instance.GetNeighbours(this).ToArray();
        //rect = new Rect(worldPosition.x-size/2, worldPosition.y-size/2, size, size);
	}

    public void ImprisonObject(GameObject obj)
    {
        walkable = false;
        prisoner = obj;
        obj.GetComponent<Base>().node.Add(this);
    }

    public void ReleaseObject()
    {
        walkable = true;
        prisoner.GetComponent<Base>().node.Remove(this);
        prisoner = null;
    }

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}

}
