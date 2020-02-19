using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{       
	public static void ReloadScene()
	{
		SceneManager.LoadScene("03Map00");
	}
}