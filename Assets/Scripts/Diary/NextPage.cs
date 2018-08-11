using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextPage : MonoBehaviour 
{
	public void GoToNextPage() 
	{
		if (SceneManager.GetActiveScene ().name == "Diary1")
			SceneManager.LoadScene ("Diary2");
		else if (SceneManager.GetActiveScene ().name == "Diary2")
			SceneManager.LoadScene ("MyTest");
	}
}
