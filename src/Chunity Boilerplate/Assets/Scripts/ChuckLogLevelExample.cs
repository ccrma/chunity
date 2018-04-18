using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuckLogLevelExample : MonoBehaviour {

	private ChuckSubInstance myChuck;

	// Use this for initialization
	void Start()
	{
		myChuck = GetComponent<ChuckSubInstance>();	
		// for no logs: you will probably use this setting 99% of the time.

		Debug.Log( "no chuck logs:" );
		Chuck.SetLogLevel( Chuck.LogLevel.None );
		RunAChuckProgram();


		// for warnings
		/*
		Debug.Log( "warning chuck logs:" );
		Chuck.SetLogLevel( Chuck.LogLevel.Warning );
		RunAChuckProgram();
		*/

		// all possible logs.
		/*
		Debug.Log( "'crazy' chuck logs:" );
		Chuck.SetLogLevel( Chuck.LogLevel.Crazy );
		RunAChuckProgram(); 
		*/

		// see other levels between the above in Chuck.cs
	}

	void RunAChuckProgram()
	{
		myChuck.RunCode( "SinOsc foo => dac; 1::second => now;" );
	}
}
