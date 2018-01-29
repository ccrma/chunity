using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChuckFloatSyncer : MonoBehaviour {


	// ================= PUBLIC FACING ================== //


	// ----------------------------------------------------
	// name: SyncFloat
	// desc: start checking chuck for floatToSync so that
	//       later, you can get it whenever you want
	// ----------------------------------------------------
	public void SyncFloat( ChuckSubInstance chuck, string floatToSync )
	{
		// cancel existing if it's happening
		StopSyncing();

		// start up again
		myChuck = chuck;
		myFloatName = floatToSync;
		myFloatCallback = Chuck.CreateGetFloatCallback( MyCallback );
	}




	// ----------------------------------------------------
	// name: GetCurrentValue
	// desc: returns the most recently fetched value for
	//       your synced float
	// ----------------------------------------------------
	public float GetCurrentValue()
	{
		return myFloatValue;
	}




	// ----------------------------------------------------
	// name: SetNewValue
	// desc: sends a new value to chuck for your synced float
	// ----------------------------------------------------
	public void SetNewValue( float newValue )
	{
		// set
		if( myChuck != null && myFloatName != "" )
		{
			myChuck.SetFloat( myFloatName, (double) newValue ); 
		}

		// pre-set my storage too
		myFloatValue = newValue;
	}



	// ----------------------------------------------------
	// name: StopSyncing
	// desc: stops checking chuck for your float
	// ----------------------------------------------------
	public void StopSyncing()
	{
		myChuck = null;
		myFloatName = "";
	}




	// =========== INTERNAL MECHANICS ========== //

	ChuckSubInstance myChuck = null;
	Chuck.FloatCallback myFloatCallback;
	string myFloatName = "";

	private void Update()
	{
		if( myChuck != null && myFloatCallback != null && myFloatName != "" )
		{
			myChuck.GetFloat( myFloatName, myFloatCallback );
		}
	}
		
	private float myFloatValue = 0;
	private void MyCallback( double newValue )
	{
		myFloatValue = (float) newValue;
	}

	void OnDestroy()
	{
		StopSyncing();
	}
}
