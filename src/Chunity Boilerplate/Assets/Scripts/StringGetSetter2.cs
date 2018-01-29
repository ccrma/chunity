using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringGetSetter2 : MonoBehaviour {

	private ChuckSubInstance myChuck;
	private ChuckStringSyncer mySyncer;

	private bool haveSetOnce = false;

	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();

		myChuck.RunCode( @"
			external string filename;
			external Event playTheFile;

			playTheFile => now;

			SndBuf buf => dac;
			filename => buf.read;
			
			""I GOT YOUR FILE"" @=> filename;

			buf.length() => now;
		" );

		mySyncer = gameObject.AddComponent<ChuckStringSyncer>();
		mySyncer.SyncString( myChuck, "filename" );
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( "space" ) )
		{
			if( !haveSetOnce )
			{
				mySyncer.SetNewValue( "special:dope" );
				haveSetOnce = true;
			}
			myChuck.BroadcastEvent( "playTheFile" );
			Debug.Log( "Hey, the synced string is " + mySyncer.GetCurrentValue() );
		}	
	}

}
