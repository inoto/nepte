﻿using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
    public Rect rect;
    public float size;
    public GameObject prisoner;
	public int movementPenalty;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;
	
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;
        size = Grid.Instance.nodeRadius * 2;
        rect = new Rect(worldPosition.x-size/2, worldPosition.y-size/2, size, size);
	}

    public void ImprisonObject(GameObject obj)
    {
        walkable = false;
        prisoner = obj;
        obj.GetComponent<Unit>().node = this;
        obj.GetComponent<Unit>().hasNode = true;
    }

    public void ReleaseObject()
    {
        walkable = true;
        prisoner.GetComponent<Unit>().node = null;
        prisoner.GetComponent<Unit>().hasNode = false;
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
