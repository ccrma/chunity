using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advancer2 : MonoBehaviour {

	ChuckSubInstance myChuck;
	ChuckFloatSyncer myAdvancerSyncer;
	ChuckEventListener myAdvancerListener;

	public Transform myCube;

	float myPos;

	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();
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

		myAdvancerSyncer = gameObject.AddComponent<ChuckFloatSyncer>();
		myAdvancerSyncer.SyncFloat( myChuck, "pos" );

		myAdvancerListener = gameObject.AddComponent<ChuckEventListener>();
		myAdvancerListener.ListenForEvent( myChuck, "notifier", RotateMyCube );

	}
	
	// Update is called once per frame
	void Update () {
		float newTime = Mathf.Clamp( Input.mousePosition.x, 250, 1000 ) / 1000.0f;

		myChuck.SetFloat( "timeStep",  newTime );
		myPos = myAdvancerSyncer.GetCurrentValue();

		transform.position = new Vector3( myPos % 4, 0, 0 );
	}

	void RotateMyCube()
	{
		myCube.Rotate( new Vector3( 5, 10, 15 ) );
	}
}
