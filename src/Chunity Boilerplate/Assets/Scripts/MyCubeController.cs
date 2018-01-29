using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCubeController : MonoBehaviour {

	ChuckSubInstance myChuck;

	// Use this for initialization
	void Start () {
		myChuck = GetComponent<ChuckSubInstance>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
		{
			// rotate my cube's transform
			transform.Rotate(new Vector3(0, 15, 5));
			// play a chuck script
			myChuck.RunCode(@"
				SndBuf buffy => dac;
				""special:dope"" => buffy.read;
				buffy.length() => now;		
	
			");
		}
	}
}
