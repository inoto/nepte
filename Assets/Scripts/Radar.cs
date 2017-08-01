using UnityEngine;

public class Radar : MonoBehaviour
{
    public int owner;

	[Header("Cache")]
	private Drone droneParent;
    private CircleCollider2D radarCollider;
    private IOwnable triggeredDrone;


	// Use this for initialization
	void Start ()
    {
        droneParent = transform.parent.GetComponent<Drone>();
        radarCollider = gameObject.GetComponent<CircleCollider2D>();
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("unit"))
        {
            triggeredDrone = other.gameObject.GetComponent<IOwnable>();

            if (triggeredDrone.GetGameObject() != null
                && other.gameObject != droneParent.gameObject
                && triggeredDrone.GetOwner() != owner)
            {
                droneParent.EnterCombatMode(other.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredDrone = null;
    }

	//public int GetOwner()
	//{
	//	return owner;
	//}

	//public void SetOwner(int newOwner)
	//{
	//	owner = newOwner;
	//}

	//public GameObject GetGameObject()
	//{
	//	return gameObject;
	//}
}
