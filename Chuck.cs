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
			// create a chuck in c++, then connect it to the unity callback
			if( (!initChuckInstance( _nextValidID )) || 
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

	public bool RunCode( string name, string code )
	{
		if( ids.ContainsKey( name ) )
		{
			return runChuckCode( ids[name], code );
		}
		else
		{
			Debug.Log( name + " has not been initialized as a ChucK instance" );
		}
		return false;
	}

	public bool SetInt( string chuckName, string variableName, System.Int64 value )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return setChuckInt( ids[chuckName], variableName, value );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool GetInt( string chuckName, string variableName, Action< System.Int64 > callback )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			// save a copy of the delegate so it doesn't get garbage collected!
			// TODO: what to do when two requests quickly in a row???
			string internalKey = chuckName + "$" + variableName;
			intCallbacks[internalKey] = new MyIntCallback( callback );
			// register the callback with ChucK
			return getChuckInt( ids[chuckName], variableName, intCallbacks[internalKey] );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool SetFloat( string chuckName, string variableName, double value )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return setChuckFloat( ids[chuckName], variableName, value );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool GetFloat( string chuckName, string variableName, Action< double > callback )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			// save a copy of the delegate so it doesn't get garbage collected!
			// TODO: what to do when two requests quickly in a row???
			string internalKey = chuckName + "$" + variableName;
			floatCallbacks[internalKey] = new MyFloatCallback( callback );
			// register the callback with ChucK
			return getChuckFloat( ids[chuckName], variableName, floatCallbacks[internalKey] );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool SignalEvent( string chuckName, string variableName )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return signalChuckEvent( ids[chuckName], variableName );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}
	}

	public bool BroadcastEvent( string chuckName, string variableName )
	{
		if( ids.ContainsKey( chuckName ) )
		{
			return broadcastChuckEvent( ids[chuckName], variableName );
		}
		else
		{
			Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
			return false;
		}	
	}

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void MyLogCallback( System.String str );

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void MyIntCallback( System.Int64 i );

	[UnmanagedFunctionPointer (CallingConvention.Cdecl)]
	public delegate void MyFloatCallback( double f );

	private MyLogCallback chout_delegate;
	private MyLogCallback cherr_delegate;
	private MyLogCallback stdout_delegate;
	private MyLogCallback stderr_delegate;
	private Dictionary< string, MyIntCallback > intCallbacks;
	private Dictionary< string, MyFloatCallback > floatCallbacks;

	const string PLUGIN_NAME = "AudioPluginChuck";

	[DllImport (PLUGIN_NAME)]
	private static extern void cleanRegisteredChucks();

	[DllImport (PLUGIN_NAME)]
	private static extern bool initChuckInstance( System.UInt32 chuckID );

	[DllImport (PLUGIN_NAME)]
	private static extern bool runChuckCode( System.UInt32 chuckID, System.String code );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChuckInt( System.UInt32 chuckID, System.String name, System.Int64 val );

	[DllImport (PLUGIN_NAME)]
	private static extern bool getChuckInt( System.UInt32 chuckID, System.String name, MyIntCallback callback );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChuckFloat( System.UInt32 chuckID, System.String name, double val );

	[DllImport (PLUGIN_NAME)]
	private static extern bool getChuckFloat( System.UInt32 chuckID, System.String name, MyFloatCallback callback );

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
		intCallbacks = new Dictionary< string, MyIntCallback >();
		floatCallbacks = new Dictionary< string, MyFloatCallback >();

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
