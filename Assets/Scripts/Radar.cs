using UnityEngine;

public class Radar : MonoBehaviour
{

	[Header("Cache")]
	private Drone droneComponent;
    private IOwnable triggeredDrone;

	// Use this for initialization
	void Start ()
    {
        droneComponent = GetComponent<Drone>();
	}

    private void OnEnable()
    {
        droneComponent = GetComponent<Drone>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (droneComponent.HasNoEnemy)
        {
            if (other.gameObject.CompareTag("drone") || other.gameObject.CompareTag("base"))
            {
                triggeredDrone = other.gameObject.GetComponent<IOwnable>();

                if (other.gameObject != droneComponent.gameObject
                    && triggeredDrone.GetOwner() != droneComponent.owner)
                {
                    droneComponent.EnterCombatMode(other.gameObject);
                    triggeredDrone.AddAttacker(droneComponent.gameObject);
                }
            }
        }
        else
        {
            if (droneComponent.enemy.gameObject.CompareTag("base") && other.gameObject.CompareTag("drone"))
			{
				triggeredDrone = other.gameObject.GetComponent<IOwnable>();

				if (other.gameObject != droneComponent.gameObject
					&& triggeredDrone.GetOwner() != droneComponent.owner)
				{
					droneComponent.EnterCombatMode(other.gameObject);
                    droneComponent.attackers.Add(triggeredDrone.GetGameObject());
                    triggeredDrone.AddAttacker(droneComponent.gameObject);
				}
			}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggeredDrone = null;
    }

}
