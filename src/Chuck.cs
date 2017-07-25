using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using System.Runtime.InteropServices;

public class Chuck
{

	public static Chuck Manager
	{
		get
		{
			if( __sharedInstance == null )
				__sharedInstance = new Chuck();

			return __sharedInstance;
		}
	}

	public void Initialize( AudioMixer mixer, string name )
	{
		// only initialize if haven't initialized yet
		if( !ids.ContainsKey( name ) )
		{
			System.UInt32 sampleRate = Convert.ToUInt32( AudioSettings.outputSampleRate );

			// create a chuck in c++, then connect it to the unity callback
			if( (!initChuckInstance( _nextValidID, sampleRate )) || 
				(!mixer.SetFloat( name, _nextValidID * 1.0f )) )
			{
				// note: when things go poorly, mixer.SetFloat 
				// never *actually* returns false and so this error message will not be seen.
				// instead, will see "Assertion failed on expression: 'res == FMOD_OK'
				Debug.Log( "ChucK ID C++ storage failed for " + name );
				return;
			}

			// store association in c-sharp
			ids.Add( name, _nextValidID );
			Debug.Log( "ChucK instance " + name + " has been initialized!" );
			_nextValidID++;
		}
		else
		{
			Debug.Log( "ChucK instance " + name + " has already been initialized." );
		}
	}

	public System.UInt32 InitializeFilter()
	{
		System.UInt32 id = _nextValidID;
		System.UInt32 sampleRate = Convert.ToUInt32( AudioSettings.outputSampleRate );

		if( !initChuckInstance( Convert.ToUInt32( id ), sampleRate ) )
		{
			Debug.Log( "Chuck C++ initialization failed for filter" );
			return System.UInt32.MaxValue;
		}

		_nextValidID++;

		return id;
	}

    public bool CleanupFilter( System.UInt32 id )
    {
        return cleanupChuckInstance( id );
    }

	public bool ManualAudioCallback( System.UInt32 chuckID, float[] inBuffer, float[] outBuffer, System.UInt32 channels )
	{
		System.UInt32 numFrames = Convert.ToUInt32( inBuffer.Length / channels );
		return chuckManualAudioCallback( chuckID, inBuffer, outBuffer, numFrames, channels, channels );
	}

	public bool RunCode( string name, string code )
	{
		if( ids.ContainsKey( name ) )
		{
			return RunCode( ids[name], code );
		}
		else
		{
			Debug.Log( name + " has not been initialized as a ChucK instance" );
		}
		return false;
	}

	public bool RunCode( System.UInt32 chuckId, string code )
	{
		return runChuckCode( chuckId, code );
	}

	public bool SetInt( string chuckName, string variableName, System.Int64 value )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return SetInt( ids[chuckName], variableName, value );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool SetInt( System.UInt32 chuckId, string variableName, System.Int64 value )
	{
		return setChuckInt( chuckId, variableName, value );
	}

	public IntCallback GetInt( string chuckName, string variableName, Action< System.Int64 > callback )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return GetInt( ids[chuckName], variableName, callback );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return null;
		}
	}

	public IntCallback GetInt( System.UInt32 chuckId, string variableName, Action< System.Int64 > callback )
	{
		// save a copy of the delegate so it doesn't get garbage collected!
		string internalKey = chuckId.ToString() + "$" + variableName;
		intCallbacks[internalKey] = new IntCallback( callback );
		// register the callback with ChucK
		if( !getChuckInt( chuckId, variableName, intCallbacks[internalKey] ) )
		{
			return null;
		}
		return intCallbacks[internalKey];
	}

	public bool SetFloat( string chuckName, string variableName, double value )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return SetFloat( ids[chuckName], variableName, value );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool SetFloat( System.UInt32 chuckId, string variableName, double value )
	{
		return setChuckFloat( chuckId, variableName, value );
	}

	public FloatCallback GetFloat( string chuckName, string variableName, Action< double > callback )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return GetFloat( ids[chuckName], variableName, callback );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return null;
		}
	}

	public FloatCallback GetFloat( System.UInt32 chuckId, string variableName, Action< double > callback )
	{
		// save a copy of the delegate so it doesn't get garbage collected!
		string internalKey = chuckId.ToString() + "$" + variableName;
		floatCallbacks[internalKey] = new FloatCallback( callback );
		// register the callback with ChucK
		if( !getChuckFloat( chuckId, variableName, floatCallbacks[internalKey] ) )
		{
			return null;
		}
		return floatCallbacks[internalKey];
	}

	public bool SignalEvent( string chuckName, string variableName )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return SignalEvent( ids[chuckName], variableName );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool SignalEvent( System.UInt32 chuckId, string variableName )
	{
		return signalChuckEvent( chuckId, variableName );
	}

	public bool BroadcastEvent( string chuckName, string variableName )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return BroadcastEvent( ids[chuckName], variableName );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}	
	}

	public bool BroadcastEvent( System.UInt32 chuckId, string variableName )
	{
		return broadcastChuckEvent( chuckId, variableName );
	}

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void MyLogCallback( System.String str );

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void IntCallback( System.Int64 i );

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void FloatCallback( double f );

	private MyLogCallback chout_delegate;
	private MyLogCallback cherr_delegate;
	private MyLogCallback stdout_delegate;
	private MyLogCallback stderr_delegate;
	private Dictionary< string, IntCallback > intCallbacks;
	private Dictionary< string, FloatCallback > floatCallbacks;

	const string PLUGIN_NAME = "AudioPluginChuck";

	[DllImport (PLUGIN_NAME)]
	private static extern void cleanRegisteredChucks();

	[DllImport (PLUGIN_NAME)]
	private static extern bool initChuckInstance( System.UInt32 chuckID, System.UInt32 sampleRate );

    [DllImport (PLUGIN_NAME)]
	private static extern bool cleanupChuckInstance( System.UInt32 chuckID );

	[DllImport (PLUGIN_NAME)]
	private static extern bool chuckManualAudioCallback( System.UInt32 chuckID, float[] inBuffer, float[] outBuffer,
		System.UInt32 numFrames, System.UInt32 inChannels, System.UInt32 outChannels );

	[DllImport (PLUGIN_NAME)]
	private static extern bool runChuckCode( System.UInt32 chuckID, System.String code );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChuckInt( System.UInt32 chuckID, System.String name, System.Int64 val );

	[DllImport (PLUGIN_NAME)]
	private static extern bool getChuckInt( System.UInt32 chuckID, System.String name, IntCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChuckFloat( System.UInt32 chuckID, System.String name, double val );

	[DllImport (PLUGIN_NAME)]
	private static extern bool getChuckFloat( System.UInt32 chuckID, System.String name, FloatCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool signalChuckEvent( System.UInt32 chuckID, System.String name );

	[DllImport (PLUGIN_NAME)]
	private static extern bool broadcastChuckEvent( System.UInt32 chuckID, System.String name );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChoutCallback( MyLogCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setCherrCallback( MyLogCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setStdoutCallback( MyLogCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setStderrCallback( MyLogCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setDataDir( System.String dir );


	private static Chuck __sharedInstance;
	private System.UInt32 _nextValidID;
	private Dictionary< string, System.UInt32 > ids;

	private Chuck()
	{
		// Store the location of data files
		setDataDir( Application.streamingAssetsPath );

		// Important in the editor, where native static arrays won't be cleaned up when entering / exiting play mode
		cleanRegisteredChucks();

		// First id is 0
		_nextValidID = 0;

		// Store exposed parameter names -> ids
		ids = new Dictionary< string, System.UInt32 >();

		// Store external ints -> callbacks to avoid garbage collection of callbacks
		intCallbacks = new Dictionary< string, IntCallback >();
		floatCallbacks = new Dictionary< string, FloatCallback >();

		// Create and store callbacks
		chout_delegate = new MyLogCallback( ChoutCallback );
		cherr_delegate = new MyLogCallback( CherrCallback );
		stdout_delegate = new MyLogCallback( StdoutCallback );
		stderr_delegate = new MyLogCallback( StderrCallback );

		// Store pointers to callbacks inside ChucK's inner workings
		setChoutCallback( chout_delegate );
		setCherrCallback( cherr_delegate );
		setStdoutCallback( stdout_delegate );
		setStderrCallback( stderr_delegate );
	}

	public void Quit()
	{
		Debug.Log( "ChucK quitting now" );
		cleanRegisteredChucks();
	}

	static void ChoutCallback( System.String str )
	{
		Debug.Log( "[chout]: " + str );
	}

	static void CherrCallback( System.String str )
	{
		Debug.LogError( "[cherr]: " + str );
	}

	static void StdoutCallback( System.String str )
	{
		Debug.Log( str );
	}

	static void StderrCallback( System.String str )
	{
		Debug.LogError( str );
	}
}
