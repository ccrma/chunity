using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleRunFromFile : MonoBehaviour
{
	// You may not always wish to type your 
	// ChucK scripts inline in Unity C# code.
	// You can also run scripts from file.

	// Place your scripts in the StreamingAssets
	// folder to run them. You can optionally
	// pass arguments to the scripts, just as
	// in the command line version of ChucK.

	ChuckSubInstance myChuck;

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();
		// play a file located in streaming assets
		myChuck.RunFile( "ExampleChuckScript.ck" );
	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetKeyDown( "space" ) )
		{
			// note that the .ck extension is optional, 
			// just like with the command line version of chuck
			// also note: arguments are passed in a colon-separated string
			myChuck.RunFile( "ExampleChuckScript", "600:0.5" );
		}
	}
}
