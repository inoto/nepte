using System.Collections;
using UnityEngine;

public class Effect: MonoBehaviour
{
	[Header("Effect")]
	public bool IsForProperty = false;
	public float Duration;

	protected virtual void Apply()
	{
//		Debug.Log("effect started");
	}

	protected virtual void Reset()
	{
//		Debug.Log("effect ended");
	}

	public virtual void Activate(float newDuration)
	{
		Duration = newDuration;
		StartCoroutine(StartEffect());
	}

	private IEnumerator StartEffect()
	{
		Apply();
		yield return new WaitForSeconds(Duration);
		Reset();
		DestroyObject(this);
	}
}