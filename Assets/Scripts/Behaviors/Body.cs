using System.Collections;
using UnityEngine;

public class Body : MonoBehaviour
{
    public bool ShowRadius = false;
	
	public float Radius = 1;
	public float RadiusHard = 0.5f;

	[Header("Components")]
    public Transform Trans;
    public Owner Owner;
    public Mover Mover;
    public CollisionCircle Collision;

	private void Awake()
	{
		Trans = GetComponent<Transform>();
        Owner = GetComponent<Owner>();
        Mover = GetComponent<Mover>();
	}

    public void OnDrawGizmos()
	{
		if (ShowRadius)
		{
			Color newColorAgain = Color.green;
			newColorAgain.a = 0.8f;
			Gizmos.color = newColorAgain;
			Gizmos.DrawWireSphere(Trans.position, Radius);
		}
	}

}
