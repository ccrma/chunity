using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExamplePersistAcrossSceneChange : MonoBehaviour 
{
	// Use this for initialization
	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
			SndBuf buf => dac;
			""special:dope"" => buf.read;
			true => buf.loop;
			2 => buf.rate;
			while( true ) { 1::second => now; }
		" );
	}
}
