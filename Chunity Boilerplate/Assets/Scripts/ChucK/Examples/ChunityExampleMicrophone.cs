using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleMicrophone : MonoBehaviour
{
	// In the same way that dac is the output
	// ("digital-analog converter"), 
	// adc is the input in ChucK 
	// ("analog-digital converter").
	// This script uses the microphone
	// set up in ChuckMainInstance / TheChuck
	// and pitch shifts it before playing it
	// out the output. (Beware feedback!)

	void Start()
	{
		GetComponent<ChuckSubInstance>().RunCode( @"
			adc => PitShift p => dac;
			while( true )
			{
				Math.random2f(0.5, 2.0) => p.shift;
				1::second => now;
			}
		" );
	}
}
