using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
	[Header("Card")]
	public float CooldownSeconds = 1f;
	public bool InCooldown = false;
	protected UISprite CooldownTimer;
	
	protected UISprite Sprite;

	public virtual void Drag()
	{
		Sprite = GetComponent<UISprite>();
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
		if (!InCooldown)
		{
			GameObject cooldownTimerObject = (GameObject) Instantiate(Resources.Load("CooldownTimer"), transform.position,
				transform.rotation, transform);
			CooldownTimer = cooldownTimerObject.GetComponent<UISprite>();
			CooldownTimer.SetAnchor(transform);
			GetComponent<UIButton>().enabled = false;
		}
		if (GameManager.Instance.IsGame)
		{
			this.StartCoroutine(CooldownTimerCoroutine());
		}
	}

	private IEnumerator CooldownTimerCoroutine()
	{
		InCooldown = true;
		float waitTime = 1 / CooldownSeconds;
		while (InCooldown)
		{
			CooldownTimer.fillAmount -= waitTime/60;
			if (CooldownTimer.fillAmount <= 0)
			{
				InCooldown = false;
				GetComponent<UIButton>().enabled = true;
				Destroy(CooldownTimer.gameObject);
			}
			yield return new WaitForSeconds(waitTime/60);
		}
	}

	private void OnDisable()
	{
		if (InCooldown)
		{
			StopAllCoroutines();
		}
	}

	private void OnEnable()
	{
		if (InCooldown)
		{
			StartCooldownTimer();
		}
	}
}
