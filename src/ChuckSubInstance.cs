using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class ChuckSubInstance : MonoBehaviour {

	public ChuckMasterInstance chuckMasterInstance;

	private string myOutputUgen;

	public bool spatialize = false;

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

		mySource = GetComponent<AudioSource>();
		mySource.loop = true;
		mySource.playOnAwake = true;

		spatialClip = (AudioClip) Resources.Load("1");
		mySource.clip = spatialClip;
		mySource.Play();

		// opposite to have first UpdateSpatialize() take effect
		prevSpatialize = !spatialize;
		UpdateSpatialize();


		// setup chuck
		myOutputUgen = chuckMasterInstance.GetUniqueVariableName( "dac" );
		chuckMasterInstance.RunCode( string.Format( @"
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
//			mySource.clip = spatialClip;
//			mySource.Play();
		}
		else
		{
			mySource.spatialBlend = 0.0f;
//			mySource.clip = spatialClip;
//			mySource.Play();
		}
		prevSpatialize = spatialize;
	}

	public string GetSubDac()
	{
		return string.Format( "external Gain {0}", myOutputUgen );
	}
	
	// Update is called once per frame
	void OnAudioFilterRead(float[] data, int channels)
	{
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

		// advance the MasterInstance to the now we need
		chuckMasterInstance.Advance( numSamplesSeen, data, myOutBuffer, Convert.ToUInt32( channels ) );

		if( !running || isMuted )
		{
			return;
		}

		// get ugen output
		chuckMasterInstance.GetUGenSamples( myOutputUgen, myMonoBuffer, numFrames );

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

	public bool RunCode( string code )
	{
		return chuckMasterInstance.RunCode( code );
	}

	public bool SetInt( string variableName, System.Int64 value )
	{
		return chuckMasterInstance.SetInt( variableName, value );
	}

	public Chuck.IntCallback CreateGetIntCallback( Action< System.Int64 > callbackFunction )
	{
		return Chuck.CreateGetIntCallback( callbackFunction );
	}

	public bool GetInt( string variableName, Chuck.IntCallback callback )
	{
		return chuckMasterInstance.GetInt( variableName, callback );
	}

	public bool SetFloat( string variableName, double value )
	{
		return chuckMasterInstance.SetFloat( variableName, value );
	}

	public Chuck.FloatCallback CreateGetFloatCallback( Action< double > callbackFunction )
	{
		return Chuck.CreateGetFloatCallback( callbackFunction );
	}

	public bool GetFloat( string variableName, Chuck.FloatCallback callback )
	{
		return chuckMasterInstance.GetFloat( variableName, callback );
	}

	public bool SetString( string variableName, System.String value )
	{
		return chuckMasterInstance.SetString( variableName, value );
	}

	public Chuck.StringCallback CreateGetStringCallback( Action< System.String > callbackFunction )
	{
		return Chuck.CreateGetStringCallback( callbackFunction );
	}

	public bool GetString( string variableName, Chuck.StringCallback callback )
	{
		return chuckMasterInstance.GetString( variableName, callback );
	}

	public Chuck.VoidCallback CreateVoidCallback( Action callbackFunction )
	{
		return Chuck.CreateVoidCallback( callbackFunction );
	}

	public bool SignalEvent( string variableName )
	{
		return chuckMasterInstance.SignalEvent( variableName );
	}

	public bool BroadcastEvent( string variableName )
	{
		return chuckMasterInstance.SignalEvent( variableName );
	}

	public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMasterInstance.ListenForChuckEventOnce( variableName, callback );
	}

	public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMasterInstance.StartListeningForChuckEvent( variableName, callback );
	}

	public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return chuckMasterInstance.StopListeningForChuckEvent( variableName, callback );
	}
}
