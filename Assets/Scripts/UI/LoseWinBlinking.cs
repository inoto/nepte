using System.Collections;
using UnityEngine;

public class LoseWinBlinking : MonoBehaviour
{
    private bool isBlinked = false;

    private UILabel label;

	// Use this for initialization
	void Start ()
    {
        label = GetComponent<UILabel>();
        StartCoroutine(Blink());
	}
	
	IEnumerator Blink()
	{
		while (true)
		{
			
            if (!isBlinked)
			{
                label.depth += 1;
                isBlinked = true;
			}
			else
			{
				label.depth -= 1;
                isBlinked = false;
			}
            yield return new WaitForSeconds(0.1f);
		}
	}
}
