using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advancer : MonoBehaviour {

	ChuckSubInstance myChuck;
	Chuck.FloatCallback myGetPosCallback;
	Chuck.VoidCallback myFirstVoidCallback;
	Chuck.VoidCallback mySecondVoidCallback;

	int notifyCount;

	float myPos;

	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();
		myGetPosCallback = Chuck.CreateGetFloatCallback( GetPosCallback );
		myFirstVoidCallback = Chuck.CreateVoidCallback( BeNotified1 );
		mySecondVoidCallback = Chuck.CreateVoidCallback( BeNotified2 );

		myPos = 0;

		myChuck.RunCode( @"
			1 => external float timeStep;
			external float pos;
			external Event notifier;

			fun void updatePos() {{
				timeStep::second => dur currentTimeStep;
				currentTimeStep / 1000 => dur deltaTime;
				now => time startTime;
				
				pos => float originalPos;
								
				while( now < startTime + currentTimeStep )
				{{
					deltaTime / currentTimeStep +=> pos;
					deltaTime => now;
				}}
			}}
			

			fun void playNote() {{
				SinOsc foo => dac;
				0.2::second => now;
				foo =< dac;
			}}

			while( true )
			{{
				spork ~ playNote();
				spork ~ updatePos();
				notifier.broadcast();
				timeStep::second => now;
			}}
		");

		myChuck.StartListeningForChuckEvent( "notifier", myFirstVoidCallback );
		myChuck.StartListeningForChuckEvent( "notifier", mySecondVoidCallback );
//		myChuck.ListenForChuckEventOnce( "notifier", myFirstVoidCallback );
//		myChuck.ListenForChuckEventOnce( "notifier", mySecondVoidCallback );

	}
	
	// Update is called once per frame
	void Update () {
		float newTime = Mathf.Clamp( Input.mousePosition.x, 250, 1000 ) / 1000.0f;

		myChuck.SetFloat( "timeStep",  newTime );
		myChuck.GetFloat( "pos", myGetPosCallback );

		transform.position = new Vector3( myPos % 4, 0, 0 );

		if( notifyCount > 5 )
		{
			myChuck.StopListeningForChuckEvent( "notifier", myFirstVoidCallback );
		}
	}

	void GetPosCallback( System.Double pos )
	{
		myPos = (float) pos;
	}

	void BeNotified1()
	{
		Debug.Log("I was notified~~");
		notifyCount++;
	}

	void BeNotified2()
	{
		Debug.Log("I was notified TWOOOO");
	}
}
