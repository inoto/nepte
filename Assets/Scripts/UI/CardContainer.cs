using UnityEngine;

public class CardContainer: MonoBehaviour
{
	public int maxSlots = 4;
	public int activeSlots = 4;
	public GameObject[] slots;
	public GameObject[] cards;
	
	private void Awake()
	{
		slots = new GameObject[maxSlots];
		cards = new GameObject[maxSlots];
	}

	private void Start()
	{
		
	}
}