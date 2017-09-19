using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Capture : MonoBehaviour
{
	public bool isCapturing = false;
	
	private Transform trans;
	public Body body;
	
	public float[] counter;
	private float counterStep = 0.02f;
	public int[] capturerCount;
	public int leadIndex = -1;
	
	public UI2DSprite assignedCircleTimer;
	
	public GameObject circleTimerPrefab;

	private void Awake()
	{
		trans = GetComponent<Transform>();
		body = GetComponent<Body>();
	}

	private void Start()
	{
		counter = new float[GameController.Instance.players];
		capturerCount = new int[GameController.Instance.players];
	}

	public void Tick(int side)
	{
		//Debug.Log("tick side: " + side);
		if (body.owner.playerNumber != -1)
			return;
		if (!isCapturing)
		{
			StartCoroutine(StartCapturing());
		}
		capturerCount[side]++;
	}
	
	public void UnTick(int side)
	{
		if (body.owner.playerNumber != -1)
			return;
		capturerCount[side]--;
	}

	public void AddCapturerByPlayer(int player)
	{
		//Debug.Log("tick side: " + side);
		if (!isCapturing)
		{
			StartCoroutine(StartCapturing());
		}
		capturerCount[player]++;
	}
	
	public void RemoveCapturerByPlayer(int player)
	{
		capturerCount[player]--;
	}

	IEnumerator StartCapturing()
	{
		isCapturing = true;
		AddCircleTimer();
		while (isCapturing)
		{
			// in every loop we're trying to find player which has more capturers than others
			// if multiple players have same maximum value then no one should to win
			int maxValue = capturerCount.Max();
			int newCounter = 0;
			foreach (var value in capturerCount)
			{
				if (value == maxValue)
					newCounter++;
			}
			if (newCounter <= 1)
			{
				int player = Array.IndexOf(capturerCount, maxValue);

				if (leadIndex == -1)
					SetLead(player);

				if (leadIndex == player)
				{
					assignedCircleTimer.fillAmount += 0.02f * capturerCount[player];
					counter[player] += 0.02f * capturerCount[player];
					if (assignedCircleTimer.fillAmount >= 1)
					{
						isCapturing = false;
						GetComponent<Base>().SetOwner(player,
							GameController.Instance.playerControllerObject[player].GetComponent<PlayerController>());
						for (int i = 0; i < counter.Length; i++)
						{
							counter[i] = 0;
							capturerCount[i] = 0;
						}
						Destroy(assignedCircleTimer.gameObject);
						leadIndex = -1;
					}
				}
				else
				{
					assignedCircleTimer.fillAmount -= 0.02f * capturerCount[player];
					counter[leadIndex] -= 0.02f * capturerCount[player];
					if (assignedCircleTimer.fillAmount <= 0)
						SetLead(player);
				}
			}
			yield return new WaitForSeconds(0.2f);
			if (body.collision.collidedCount <= 0)
			{
				isCapturing = false;
				Destroy(assignedCircleTimer.gameObject);
				Clean();
			}
		}
	}

	void SetLead(int player)
	{
		leadIndex = player;
		assignedCircleTimer.color = GameController.Instance.playerColors[player];
	}

	void Clean()
	{
		leadIndex = -1;
		for (int i = 0; i < counter.Length; i++)
		{
			counter[i] = 0;
			capturerCount[i] = 0;
		}
	}

	void AddCircleTimer()
	{
		GameObject assignedCircleTimerObject = Instantiate(circleTimerPrefab, trans.position, trans.rotation);
		assignedCircleTimerObject.transform.SetParent(GameObject.Find("Timers").transform);
		assignedCircleTimerObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
		assignedCircleTimer = assignedCircleTimerObject.GetComponent<UI2DSprite>();
		assignedCircleTimer.SetAnchor(gameObject);
		//assignedCircleTimer.material.SetColor("_Color", body.owner.color);
		//assignedCircleTimer.material.SetColor("_TintColor", body.owner.color);
	}
}