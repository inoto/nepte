using UnityEngine;

public class Radar : MonoBehaviour
{

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

    private void OnEnable()
    {
        droneParent = transform.parent.GetComponent<Drone>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (droneParent.HasNoEnemy)
        {
            if (other.gameObject.CompareTag("drone") || other.gameObject.CompareTag("base"))
            {
                triggeredDrone = other.gameObject.GetComponent<IOwnable>();

                if (other.gameObject != droneParent.gameObject
                    && triggeredDrone.GetOwner() != droneParent.owner)
                {
                    droneParent.EnterCombatMode(other.gameObject);
                    triggeredDrone.AddAttacker(droneParent.gameObject);
                }
            }
        }
        else
        {
            if (droneParent.enemy.gameObject.CompareTag("base") && other.gameObject.CompareTag("drone"))
			{
				triggeredDrone = other.gameObject.GetComponent<IOwnable>();

				if (other.gameObject != droneParent.gameObject
					&& triggeredDrone.GetOwner() != droneParent.owner)
				{
					droneParent.EnterCombatMode(other.gameObject);
                    droneParent.attackers.Add(triggeredDrone.GetGameObject());
                    triggeredDrone.AddAttacker(droneParent.gameObject);
				}
			}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredDrone = null;
    }

}
