using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyADCUser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<ChuckSubInstance>().RunCode(@"
			adc => PitShift p => dac;
			while( true )
			{
				Math.random2f(0.5, 2.0) => p.shift;
				1::second => now;
			}
		");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
