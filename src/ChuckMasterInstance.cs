using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


// TODO: require audio component, have this thing have a mic input?
// will require refactoring of Advance()
public class ChuckMasterInstance : MonoBehaviour {

	private System.UInt32 myChuckId = System.UInt32.MaxValue;
	private int currentVar = 0;
	private long numSamplesSeen = 0;

	// Use this for initialization
	void Awake()
	{
		// create a chuck
		myChuckId = Chuck.Manager.InitializeFilter();
	}

	public string GetUniqueVariableName()
	{
		currentVar++;
		return "v" + currentVar.ToString();
	}

	public string GetUniqueVariableName( string prefix )
	{
		currentVar++;
		return prefix + currentVar.ToString();
	}

	public bool Advance( long now, float[] input, float[] output, uint channels ) 
	{
		// check if we've already advanced far enough
		if( now <= numSamplesSeen )
		{
			return true;
		}

		if( Chuck.Manager.ManualAudioCallback( myChuckId, input, output, channels ) )
		{
			numSamplesSeen += output.Length / channels;
			return true;
		}

		// couldn't call audio callback
		return false;
	}

	public bool RunCode( string code )
	{
		return Chuck.Manager.RunCode( myChuckId, code );
	}

	public bool SetInt( string variableName, System.Int64 value )
	{
		return Chuck.Manager.SetInt( myChuckId, variableName, value );
	}

	public Chuck.IntCallback CreateGetIntCallback( Action< System.Int64 > callbackFunction )
	{
		return Chuck.CreateGetIntCallback( callbackFunction );
	}

	public bool GetInt( string variableName, Chuck.IntCallback callback )
	{
		return Chuck.Manager.GetInt( myChuckId, variableName, callback );
	}

	public bool SetFloat( string variableName, double value )
	{
		return Chuck.Manager.SetFloat( myChuckId, variableName, value );
	}

	public Chuck.FloatCallback CreateGetFloatCallback( Action< double > callbackFunction )
	{
		return Chuck.CreateGetFloatCallback( callbackFunction );
	}

	public bool GetFloat( string variableName, Chuck.FloatCallback callback )
	{
		return Chuck.Manager.GetFloat( myChuckId, variableName, callback );
	}

	public bool SetString( string variableName, System.String value )
	{
		return Chuck.Manager.SetString( myChuckId, variableName, value );
	}

	public Chuck.StringCallback CreateGetStringCallback( Action< System.String > callbackFunction )
	{
		return Chuck.CreateGetStringCallback( callbackFunction );
	}

	public bool GetString( string variableName, Chuck.StringCallback callback )
	{
		return Chuck.Manager.GetString( myChuckId, variableName, callback );
	}

	public Chuck.VoidCallback CreateVoidCallback( Action callbackFunction )
	{
		return Chuck.CreateVoidCallback( callbackFunction );
	}

	public bool SignalEvent( string variableName )
	{
		return Chuck.Manager.SignalEvent( myChuckId, variableName );
	}

	public bool BroadcastEvent( string variableName )
	{
		return Chuck.Manager.SignalEvent( myChuckId, variableName );
	}

	public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
	{
		return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, callback );
	}

	public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return Chuck.Manager.StartListeningForChuckEvent( myChuckId, variableName, callback );
	}

	public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
	{
		return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, callback );
	}

	public bool GetUGenSamples( string variableName, float[] buffer, int numSamples )
	{
		return Chuck.Manager.GetUGenSamples( myChuckId, variableName, buffer, numSamples );
	}

    private void OnDestroy()
    {
        Chuck.Manager.CleanupFilter( myChuckId );
    }
}
