using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextPage : MonoBehaviour 
{
	public void GoToNextPage() 
	{
		if (SceneManager.GetActiveScene ().name == "Diary1")
			SceneManager.LoadScene ("MyTest");
	}

	public void GoToNextPage(string sceneName) 
	{
		SceneManager.LoadScene (sceneName);
	}
		
}
