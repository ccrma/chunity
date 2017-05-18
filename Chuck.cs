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
			// store association in c++
			if( !mixer.SetFloat(name, _nextValidID * 1.0f) )
			{
				// note: when things go poorly, mixer.SetFloat 
				// never *actually* returns false and so this error message will not be seen.
				// instead, will see "Assertion failed on expression: 'res == FMOD_OK'
				Debug.Log( "ChucK ID C++ storage failed for " + name );
				return;
			}

			// store association in c-sharp
			ids.Add( name, _nextValidID );
			Debug.Log( "ChucK ID " + name + " stored with id " + _nextValidID.ToString() );
			_nextValidID++;
		}
		else
		{
			Debug.Log( "ChucK instance " + name + " has already been initialized" );
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
			Debug.Log( name + " has not been registered as a ChucK instance" );
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
			Debug.Log( chuckName + " has not been registered as a ChucK instance" );
			return false;
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MyLogCallback( System.String str );

	MyLogCallback chout_delegate;
	MyLogCallback cherr_delegate;

	const string PLUGIN_NAME = "AudioPluginChuck";

	[DllImport (PLUGIN_NAME)]
	private static extern void cleanRegisteredChucks();

	[DllImport (PLUGIN_NAME)]
	private static extern bool runChuckCode( System.UInt32 chuckID, System.String code );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChuckInt( System.UInt32 chuckID, System.String name, System.Int64 val );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setChoutCallback( IntPtr fp );

	[DllImport (PLUGIN_NAME)]
	private static extern bool setCherrCallback( IntPtr fp );



	private static Chuck __sharedInstance;
	private System.UInt32 _nextValidID;
	private Dictionary< string, System.UInt32 > ids;

	private Chuck()
	{
		// Important in the editor, where native static arrays won't be cleaned up when entering / exiting play mode
		cleanRegisteredChucks();

		// First id is 0
		_nextValidID = 0;

		// Store exposed parameter names -> ids
		ids = new Dictionary< string, System.UInt32 >();

		// Create callbacks (see http://hojjatjafary.blogspot.ca/2013/01/c-plugin-debug-log.html )
		chout_delegate = new MyLogCallback( ChoutCallback );
		cherr_delegate = new MyLogCallback( CherrCallback );

		// Convert callback_delegate into a function pointer that can be
		// used in unmanaged code.
		IntPtr intptr_chout_delegate = Marshal.GetFunctionPointerForDelegate(chout_delegate);
		IntPtr intptr_cherr_delegate = Marshal.GetFunctionPointerForDelegate(cherr_delegate);

		setChoutCallback( intptr_chout_delegate );
		setCherrCallback( intptr_cherr_delegate );
	}

	public void Quit()
	{
		Debug.Log("ChucK quitting now");
		cleanRegisteredChucks();
	}

	static void ChoutCallback( System.String str )
	{
		Debug.Log( "[chout] " + str );
	}

	static void CherrCallback( System.String str )
	{
		Debug.LogError( "[cherr] " + str );
	}
}