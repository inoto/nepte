using UnityEngine;

public class CardContainer: MonoBehaviour
{
	public int MaxSlots = 4;
	public int ActiveSlots = 4;
	public GameObject[] Slots;
	public GameObject[] Cards;
	
	private void Awake()
	{
		Slots = new GameObject[MaxSlots];
		Cards = new GameObject[MaxSlots];
	}

}