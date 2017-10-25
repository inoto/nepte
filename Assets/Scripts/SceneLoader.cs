using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader: MonoBehaviour
{
	private bool loadScene = false;

	[SerializeField] private int scene;
	[SerializeField] private TextMesh loadingText;
	
	private void Update()
	{

		if (loadScene == false)
		{
			loadScene = true;
			loadingText.text = "Loading...";
			StartCoroutine(LoadNewScene());

		}
		else
		{
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
		}

	}
	
	private IEnumerator LoadNewScene()
	{
		yield return new WaitForSeconds(3);
		AsyncOperation async = SceneManager.LoadSceneAsync(scene);
		while (!async.isDone)
		{
			yield return null;
		}

	}
}
