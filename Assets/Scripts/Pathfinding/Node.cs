using UnityEngine;

[System.Serializable]
public class Node : IHeapItem<Node>
{
	public bool IsWalkable;
	public Vector2 WorldPosition;
	public readonly int GridX;
	public readonly int GridY;
    [System.NonSerialized] private readonly Rect Rect;
    [System.NonSerialized] private float size;
	private GameObject prisoner;
    [System.NonSerialized]
	public Node[] Neigbours = new Node[8];

    [System.NonSerialized]
	public readonly int[] Distance = new int[GameManager.Instance.Players];
    public bool IsNeigboursFilled = false;
    [System.NonSerialized]
    public readonly bool[] Visited = new bool[GameManager.Instance.Players];
    [System.NonSerialized]
    public readonly Vector2[] FlowVector = new Vector2[GameManager.Instance.Players];

	public int GCost;
	public int HCost;
	public Node Parent;
	private int heapIndex;
	
    public Node(Rect newRect, Vector2 pos, int gridX, int gridY)
    {
        IsWalkable = true;
        Rect = newRect;
        WorldPosition = pos;
		GridX = gridX;
		GridY = gridY;
	    size = Rect.size.x;
		for (int i = 0; i < Distance.Length; i++)
		{
			Distance[i] = 0;
            Visited[i] = false;
		}
		//neigbours = Grid.Instance.GetNeighbours(this);
        //rect = new Rect(worldPosition.x-size/2, worldPosition.y-size/2, size, size);
	}

    public void ImprisonObject(GameObject obj)
    {
        IsWalkable = false;
        prisoner = obj;
        //obj.GetComponent<Drone>().move.node.Add(this);
    }

    public void ReleaseObject()
    {
        IsWalkable = true;
        //prisoner.GetComponent<Drone>().node.Remove(this);
        prisoner = null;
    }

	private int FCost {
		get {
			return GCost + HCost;
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
		int compare = FCost.CompareTo(nodeToCompare.FCost);
		if (compare == 0) {
			compare = HCost.CompareTo(nodeToCompare.HCost);
		}
		return -compare;
	}

}
