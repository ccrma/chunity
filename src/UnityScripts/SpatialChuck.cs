using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


[RequireComponent(typeof(AudioSource))]
public class SpatialChuck : MonoBehaviour {

	private bool running = false;
	private System.UInt32 myChuckId = System.UInt32.MaxValue;
	private int myNumChannels = 2;
	private int myBufferLength;
	private float[] myOutBuffer;
	private AudioSource mySource;
	private bool isMuted;

	// Use this for initialization
	void Awake() {
		int numBuffers;
		AudioSettings.GetDSPBufferSize( out myBufferLength, out numBuffers );

		myOutBuffer = new float[myBufferLength * myNumChannels];
		myChuckId = Chuck.Manager.InitializeFilter();

		mySource = GetComponent<AudioSource>();
		mySource.spatialize = true;
		// must run spatializer after effects (includes this chuck effect)
		// for chuck to be spatialized
		mySource.spatializePostEffects = true;
		mySource.spatialBlend = 1.0f;

		running = true;

		Debug.Log( "INITTED A CHUCK YAY " + myChuckId.ToString() );
	}

	void Update()
	{
		isMuted = mySource.mute;
	}

	// Update is called once per frame
	void OnAudioFilterRead(float[] data, int channels)
	{
		if( !running || isMuted )
		{
			return;
		}

		if( channels != myNumChannels )
		{
			// sadness -- num channels has changed so we must reconstruct myOutBuffer
			myNumChannels = channels;
			myOutBuffer = new float[myBufferLength * myNumChannels];
		}

		// first, run callback with data as input and myOutBuffer as output
		Chuck.Manager.ManualAudioCallback( myChuckId, data, myOutBuffer, Convert.ToUInt32( channels ) );

		// then, copy myOutBuffer back into data
		Array.Copy( myOutBuffer, data, data.Length );
	}

	public bool RunCode( string code )
	{
		return Chuck.Manager.RunCode( myChuckId, code );
	}

	public bool SetInt( string variableName, System.Int64 value )
	{
		return Chuck.Manager.SetInt( myChuckId, variableName, value );
	}

	public bool GetInt( string variableName, Action< System.Int64 > callback )
	{
		return Chuck.Manager.GetInt( myChuckId, variableName, callback );
	}

	public bool SetFloat( string variableName, double value )
	{
		return Chuck.Manager.SetFloat( myChuckId, variableName, value );
	}

	public bool GetFloat( string variableName, Action< double > callback )
	{
		return Chuck.Manager.GetFloat( myChuckId, variableName, callback );
	}

	public bool SignalEvent( string variableName )
	{
		return Chuck.Manager.SignalEvent( myChuckId, variableName );
	}

	public bool BroadcastEvent( string variableName )
	{
		return Chuck.Manager.SignalEvent( myChuckId, variableName );
	}
}
