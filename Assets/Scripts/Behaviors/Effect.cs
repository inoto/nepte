using System.Collections;
using UnityEngine;

public class Effect: MonoBehaviour
{
	[Header("Effect")]
	public bool forProperty = false;
	public float duration;

	protected virtual void Apply()
	{
		//Debug.Log("effect started");
	}

	protected virtual void Reset()
	{
		//Debug.Log("effect ended");
	}

	public virtual void Activate(float newDuration)
	{
		duration = newDuration;
		StartCoroutine(StartEffect());
	}

	IEnumerator StartEffect()
	{
		Apply();
		yield return new WaitForSeconds(duration);
		Reset();
		DestroyObject(this);
	}
}