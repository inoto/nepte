using UnityEngine;

public class Battleground : MonoBehaviour
{
    public Vector3 size;

    public float width, height;

	// Use this for initialization
	void Start ()
    {
        size = GetComponent<SpriteRenderer>().bounds.size;
	}

}
