using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringGetSetter : MonoBehaviour {

	private ChuckSubInstance myChuck;
	private Chuck.StringCallback myCallback;

	private string gottenString;
	private bool haveSetOnce = false;

	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();
		myCallback = Chuck.CreateGetStringCallback( GetTheString );

		myChuck.RunCode( @"
			external string filename;
			external Event playTheFile;

			playTheFile => now;

			SndBuf buf => dac;
			filename => buf.read;
			
			""I GOT YOUR FILE"" @=> filename;

			buf.length() => now;
		" );
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( "space" ) )
		{
			if( !haveSetOnce )
			{
				myChuck.SetString( "filename", "special:dope" );
				haveSetOnce = true;
			}
			myChuck.BroadcastEvent( "playTheFile" );
			myChuck.GetString( "filename", myCallback );
		}	
	}


	void GetTheString( string s )
	{
		gottenString = s;
		Debug.Log( "Hey I fetched a string! It's " + s );
	}
}
