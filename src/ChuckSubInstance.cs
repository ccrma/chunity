using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class ChuckSubInstance : MonoBehaviour {

	// ================= PUBLIC FACING ================== //


	// ----------------------------------------------------
	// name: chuckMainInstance
	// desc: ChuckSubInstance relies on a ChuckMainInstance
	//       that it shares with all other ChuckSubInstances
	//       relying on that ChuckMainInstance
	// ----------------------------------------------------
	[Tooltip( "The ChuckMainInstance this sub-instance relies on." )]
	public ChuckMainInstance chuckMainInstance;




	// ----------------------------------------------------
	// name: spatialize
	// desc: whether to spatialize this ChuckSubInstance
	// ----------------------------------------------------
	[Tooltip( "Whether to spatialize this ChuckSubInstance" )]
	public bool spatialize = false;




	// ----------------------------------------------------
	// name: RunCode
	// desc: add a new ChucK program to this VM
	// ----------------------------------------------------
	public bool RunCode( string code )
	{
		return chuckMainInstance.RunCodeWithReplacementDac(
			code, myOutputUgen );
	}




	// ----------------------------------------------------
	// name: SetInt
	// desc: set the value of external int variableName
	// ----------------------------------------------------
	public bool SetInt( string variableName, System.Int64 value )
	{
		return chuckMainInstance.SetInt( variableName, value );
	}




	// ----------------------------------------------------
	// name: CreateGetIntCallback
	// desc: construct the callback necessary for GetInt
	// ----------------------------------------------------
	public Chuck.IntCallback CreateGetIntCallback( Action< System.Int64 > callbackFunction )
	{
		return Chuck.CreateGetIntCallback( callbackFunction );
	}




	// ----------------------------------------------------
	// name: GetInt
	// desc: eventually call the callback with the value 
	//       of external int variableName
	// ----------------------------------------------------
	public bool GetInt( string variableName, Chuck.IntCallback callback )
	{
		return chuckMainInstance.GetInt( variableName, callback );
	}




	// ----------------------------------------------------
	// name: SetFloat
	// desc: set the value of external float variableName
	// ----------------------------------------------------
	public bool SetFloat( string variableName, double value )
	{
		return chuckMainInstance.SetFloat( variableName, value );
	}




	// ----------------------------------------------------
	// name: CreateGetFloatCallback
	// desc: construct the callback necessary for GetFloat
	// ----------------------------------------------------
	public Chuck.FloatCallback CreateGetFloatCallback( Action< double > callbackFunction )
	{
		return Chuck.CreateGetFloatCallback( callbackFunction );
	}




	// ----------------------------------------------------
	// name: GetFloat
	// desc: eventually call the callback with the value 
	//       of external float variableName
	// ----------------------------------------------------
	public bool GetFloat( string variableName, Chuck.FloatCallback callback )
	{
		return chuckMainInstance.GetFloat( variableName, callback );
	}




	// ----------------------------------------------------
	// name: SetString
	// desc: set the value of external string variableName
	// ----------------------------------------------------
	public bool SetString( string variableName, System.String value )
	{
		return chuckMainInstance.SetString( variableName, value );
	}




	// ----------------------------------------------------
	// name: CreateGetStringCallback
	// desc: construct the callback necessary for GetString
	// ----------------------------------------------------
	public Chuck.StringCallback CreateGetStringCallback( Action< System.String > callbackFunction )
	{
		return Chuck.CreateGetStringCallback( callbackFunction );
	}




	// ----------------------------------------------------
	// name: GetString
	// desc: eventually call the callback with the value 
	//       of external string variableName
	// ----------------------------------------------------
	public bool GetString( string variableName, Chuck.StringCallback callback )
	{
		return chuckMainInstance.GetString( variableName, callback );
	}




	// ----------------------------------------------------
	// name: SignalEvent
	// desc: call .signal() on external Event variableName
	//       (awake the next listener)
	// ----------------------------------------------------
	public bool SignalEvent( string variableName )
	{
		return chuckMainInstance.SignalEvent( variableName );
	}




	// ----------------------------------------------------
	// name: BroadcastEvent
	// desc: call .broadcast() on external Event variableName
	//       (awake all listeners)
	// ----------------------------------------------------
	public bool BroadcastEvent( string variableName )
	{
		return chuckMainInstance.SignalEvent( variableName );
	}




	// ----------------------------------------------------
	// name: CreateVoidCallback
	// desc: create the callback necessary for waiting on
	//       chuck events
	// ----------------------------------------------------
	public Chuck.VoidCallback CreateVoidCallback( Action callbackFunction )
	{
		return Chuck.CreateVoidCallback( callbackFunction );
	}




	// ----------------------------------------------------
	// name: ListenForChuckEventOnce
	// desc: call the callback only the next time that
	//       external Event variableName signals it
	// ----------------------------------------------------
	public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMainInstance.ListenForChuckEventOnce( variableName, callback );
	}




	// ----------------------------------------------------
	// name: StartListeningForChuckEvent
	// desc: call the callback every time that 
	//       external Event variableName signals it
	//       (until canceled)
	// ----------------------------------------------------
	public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMainInstance.StartListeningForChuckEvent( variableName, callback );
	}




	// ----------------------------------------------------
	// name: StopListeningForChuckEvent
	// desc: cancel the callback registered to 
	//       external Event variableName
	// ----------------------------------------------------
	public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMainInstance.StopListeningForChuckEvent( variableName, callback );
	}




    // ----------------------------------------------------
	// name: SetRunning
	// desc: whether the SubInstance is outputting sound
	// ----------------------------------------------------
	public void SetRunning( bool r )
	{
		running = r;
	}






	// =========== INTERNAL MECHANICS ========== //


	private string myOutputUgen;

	private bool running = false;
	private int myNumChannels = 2;
	private int myBufferLength;
	private float[] myOutBuffer;
	private float[] myMonoBuffer;
	private AudioSource mySource;
	private bool isMuted;
	private bool prevSpatialize;

	private AudioClip spatialClip;

	private long numSamplesSeen = 0;

	// Use this for initialization
	void Awake() {
        // unity getting stuff
		int numBuffers;
		AudioSettings.GetDSPBufferSize( out myBufferLength, out numBuffers );

		myOutBuffer = new float[myBufferLength * myNumChannels];
		myMonoBuffer = new float[myBufferLength];

		// setup group for reliable ordering
        mySource = GetComponent<AudioSource>();
        mySource.outputAudioMixerGroup = Chuck.FindAudioMixerGroup( "ChuckSubInstanceDestination" );
        // other settings
		mySource.loop = true;
		mySource.playOnAwake = true;
		// medium priority
		mySource.priority = 128;

		spatialClip = (AudioClip) Resources.Load("1");
		mySource.clip = spatialClip;
		mySource.Play();

		// opposite to have first UpdateSpatialize() take effect
		prevSpatialize = !spatialize;
		UpdateSpatialize();


		// setup chuck
		myOutputUgen = chuckMainInstance.GetUniqueVariableName( "__dac__" );
		// replacement dac is initted and constructed here!
		// so it shouldn't have to be anywhere else.
		chuckMainInstance.RunCode( string.Format( @"
			external Gain {0} => blackhole;
			true => {0}.buffered;
		", myOutputUgen ) );

		running = true;

	}

	void Update()
	{
		isMuted = mySource.mute;
		UpdateSpatialize();
	}

	void UpdateSpatialize()
	{
		if( prevSpatialize == spatialize )
		{
			return;
		}

		if( spatialize )
		{
			mySource.spatialBlend = 1.0f;
		}
		else
		{
			mySource.spatialBlend = 0.0f;
		}
		prevSpatialize = spatialize;
	}

	// Update is called once per frame
	void OnAudioFilterRead(float[] data, int channels)
	{
		if( !chuckMainInstance.HasInit() )
		{
			// my chuck is not ready. be silent.
			Array.Clear( data, 0, data.Length );
			return;
		}

		// check whether channels is correct
		if( channels != myNumChannels )
		{
			// sadness -- num channels has changed so we must reconstruct myOutBuffer
			myNumChannels = channels;
			myOutBuffer = new float[myBufferLength * myNumChannels];
		}

		// update num samples seen
		int numFrames = data.Length / channels;
		numSamplesSeen += numFrames;

		if( !running || isMuted )
		{
			// I am not ready. be silent.
			Array.Clear( data, 0, data.Length );
			return;
		}

		// get ugen output
		chuckMainInstance.GetUGenSamples( myOutputUgen, myMonoBuffer, numFrames );

		// always multiply output by input
		// if spatializing, input is some lowered level
		// if not spatializing, input is 1 and multiplication is a nop
		for( int i = 0; i < numFrames; i++ )
		{
			for( int j = 0; j < channels; j++ )
			{
				data[i*channels + j] *= myMonoBuffer[i];
			}
		}
	}


    public string GetUniqueVariableName()
    {
        return chuckMainInstance.GetUniqueVariableName();
    }

}
