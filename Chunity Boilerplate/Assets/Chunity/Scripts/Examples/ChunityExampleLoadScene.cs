using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChunityExampleLoadScene : MonoBehaviour 
{
	public string nextScene;

	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			SceneManager.LoadScene( nextScene );
		}
	}
}
