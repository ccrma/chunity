using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetIntExample : MonoBehaviour {

	// my chuck
	private ChuckSubInstance myChuck;

	// store the callback
	private Chuck.IntCallback myGetTheIntCallback;

	// did we get a new value?
	private bool haveNewValue = false;
	// what was new value?
	private System.Int64 theNewValue = 0;


	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();
		myGetTheIntCallback = myChuck.CreateGetIntCallback( GetTheInt );

		myChuck.RunCode(@"
			external int myExternalInt;
			while( true )
			{
				Math.random2( 300, 1000 ) => myExternalInt;
				10::ms => now;
			}
		");

		myChuck.GetInt( "myExternalInt", myGetTheIntCallback );
	}
	
	// Update is called once per frame
	void Update () {
		if( haveNewValue )
		{
			// reset
			haveNewValue = false;
			// do something with the value
			// TODO: debug why this is always 0
			Debug.Log( "the new value is: " + theNewValue.ToString() );

			// call callback again
			myChuck.GetInt( "myExternalInt", myGetTheIntCallback );
		}
	}

	void GetTheInt( System.Int64 result )
	{
		haveNewValue = true;
		theNewValue = result;
	}
}
