using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
	[Header("Card")]
	public float cooldownSeconds = 1f;
	public bool inCooldown = false;
	protected UISprite cooldownTimer;
	
	protected UISprite sprite;

	public virtual void Drag()
	{
		sprite = GetComponent<UISprite>();
//		if (sprite != null)
//		{
//			sprite.depth += 10;
//		}
	}

	public virtual bool Activate(Vector2 position)
	{
		return true;
	}

	public void StartCooldownTimer()
	{
		if (!inCooldown)
		{
			GameObject cooldownTimerObject = (GameObject) Instantiate(Resources.Load("CooldownTimer"), transform.position,
				transform.rotation, transform);
			cooldownTimer = cooldownTimerObject.GetComponent<UISprite>();
			cooldownTimer.SetAnchor(transform);
			GetComponent<UIButton>().enabled = false;
		}
		StartCoroutine(CooldownTimer());
	}

	IEnumerator CooldownTimer()
	{
		inCooldown = true;
		float waitTime = 1 / cooldownSeconds;
		while (inCooldown)
		{
			cooldownTimer.fillAmount -= waitTime/60;
			if (cooldownTimer.fillAmount <= 0)
			{
				inCooldown = false;
				GetComponent<UIButton>().enabled = true;
				Destroy(cooldownTimer.gameObject);
			}
			yield return new WaitForSeconds(waitTime/60);
		}
	}

	private void OnDisable()
	{
		if (inCooldown)
			StopAllCoroutines();
	}

	private void OnEnable()
	{
		if (inCooldown)
			StartCooldownTimer();
	}
}
