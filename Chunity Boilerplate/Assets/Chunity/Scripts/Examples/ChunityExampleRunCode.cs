using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleRunCode : MonoBehaviour
{
	// This example runs a ChucK script every time
	// the space bar is pressed.

	ChuckSubInstance myChuck;

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_IOS
		for (int i = 0; i < Input.touchCount; ++i)
		{
			if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
#else
		if( Input.GetKeyDown( "space" ) )
		{
#endif
			// rotate my cube's transform
			transform.Rotate( new Vector3( 0, 15, 5 ) );

			// play a chuck script
			myChuck.RunCode( @"
				SndBuf buffy => dac;
				""special:dope"" => buffy.read;
				buffy.length() => now;		
	
			" );
		}
#if UNITY_IOS
		}
#endif

	}
}
