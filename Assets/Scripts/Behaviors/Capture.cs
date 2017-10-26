using System;
using System.Collections;
using UnityEngine;

[Obsolete("Not used anymore",true)]
public class Capture : MonoBehaviour
{
	public bool isCapturing = false;
	
	private Transform trans;
	public Owner owner;
	public Planet bas;
	
	private float counterStep = 0.01f;
	public int[] capturerCount;
	public int leadIndex = -1;
	
	public UI2DSprite assignedCircleTimer;
	
	public GameObject circleTimerPrefab;

	private void Awake()
	{
		trans = GetComponent<Transform>();
		owner = GetComponent<Owner>();
		bas = GetComponent<Planet>();
//		body = GetComponent<Body>();
	}

	private void Start()
	{
		//counter = new float[GameController.Instance.players];
		capturerCount = new int[GameManager.Instance.Players];
	}

	public void Tick(int side)
	{
		//Debug.Log("tick side: " + side);
		if (owner.PlayerNumber != -1)
			return;
		if (!isCapturing)
		{
			StartCoroutine(StartCapturing());
		}
		capturerCount[side]++;
	}
	
	public void UnTick(int side)
	{
		if (owner.PlayerNumber != -1)
			return;
		capturerCount[side]--;
	}

	public void AddCapturerByPlayer(int player)
	{
		//Debug.Log("tick side: " + side);
		capturerCount[player]++;
		if (!isCapturing)
		{
//			Debug.Log("start capture");
			StartCoroutine(StartCapturing());
		}
		
	}
	
	public void RemoveCapturerByPlayer(int player)
	{
		if (capturerCount[player] > 0)
			capturerCount[player]--;
	}

	IEnumerator StartCapturing()
	{
//		Debug.Log("start capture coroutine");
		isCapturing = true;
		AddCircularTimer();
		while (isCapturing)
		{
			// in every loop we're trying to find player which has more capturers than others
			// if multiple players have same maximum value then no one should be lead
			if (leadIndex == -1)
			{
				leadIndex = FindLead();

				if (leadIndex > -1)
				{
					//Debug.Log("new lead: " + leadIndex);
					assignedCircleTimer.color = GameManager.Instance.PlayerColors[leadIndex];
				}
			}
			else
			{
				int opponentCapturers = CountLeadOpponents();
				if (opponentCapturers > 0)
				{
					//Debug.Log("lead opponents: " + opponentCapturers);
					assignedCircleTimer.fillAmount = assignedCircleTimer.fillAmount + counterStep * capturerCount[leadIndex] -
					                                 counterStep * opponentCapturers;
					//counter[leadIndex] = counter[leadIndex] + counterStep * capturerCount[leadIndex] - counterStep * opponentCapturers;
				}
				else
				{
					assignedCircleTimer.fillAmount += counterStep * capturerCount[leadIndex];
					//counter[leadIndex] += counterStep * capturerCount[leadIndex];
				}
				
				if (assignedCircleTimer.fillAmount >= 1)
				{
					isCapturing = false;
					GetComponent<Planet>().SetOwner(leadIndex,
						GameManager.Instance.PlayerController[leadIndex]);
					//GetComponent<Base>().PutNearDronesInside();
					Clean();
					Destroy(assignedCircleTimer.gameObject);
				}
				if (assignedCircleTimer.fillAmount <= 0)
				{
					leadIndex = -1;
					Clean();
				}
				
				if (bas.Collision.CollidedCount <= 0)
				{
					assignedCircleTimer.fillAmount -= counterStep * 4;
					//counter[leadIndex] -= counterStep * 4;
					if (assignedCircleTimer.fillAmount <= 0)
					{
						isCapturing = false;
						Destroy(assignedCircleTimer.gameObject);
						Clean();
					}
				}
			}
			yield return new WaitForSeconds(0.4f);
		}
	}

	int CountLeadOpponents()
	{
		int count = 0;
		for (int i = 0; i < capturerCount.Length; i++)
		{
			if (i == leadIndex)
				continue;
			if (capturerCount[i] > 0)
				count += capturerCount[i];
		}
		return count;
	}

	int FindLead()
	{
		int lead = -1;
		int max = 0;
		int enters = 0;
		for (int i = 0; i < capturerCount.Length; i++)
		{
			if (capturerCount[i] > max)
			{
				lead = i;
			}
			if (capturerCount[i] == max && lead > -1)
				enters++;
		}
		if (enters > 1)
			return -1;
		else
			return lead;
	}

	void Clean()
	{
		leadIndex = -1;
		for (int i = 0; i < capturerCount.Length; i++)
		{
			//counter[i] = 0;
			capturerCount[i] = 0;
		}
	}

	public void Reset()
	{
		if (assignedCircleTimer != null)
			Destroy(assignedCircleTimer);
		StopAllCoroutines();
		isCapturing = false;
		Clean();
	}

	void AddCircularTimer()
	{
		GameObject assignedCircleTimerObject = Instantiate(circleTimerPrefab, trans.position, trans.rotation);
		assignedCircleTimerObject.transform.SetParent(GameObject.Find("Timers").transform);
		assignedCircleTimerObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
		assignedCircleTimer = assignedCircleTimerObject.GetComponent<UI2DSprite>();
//		assignedCircleTimer.SetAnchor(gameObject);
		//assignedCircleTimer.material.SetColor("_Color", body.owner.color);
		//assignedCircleTimer.material.SetColor("_TintColor", body.owner.color);
	}
}