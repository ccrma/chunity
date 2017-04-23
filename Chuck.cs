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

	public void RunCode( string name, string code )
	{
		if( ids.ContainsKey( name ) )
		{
			runChuckCode( ids[name], code );
		}
		else
		{
			Debug.Log( name + " has not been registered as a ChucK instance" );
		}
	}

	const string PLUGIN_NAME = "AudioPluginDemo";

	[DllImport (PLUGIN_NAME)]
	private static extern void cleanRegisteredChucks();

	[DllImport (PLUGIN_NAME)]
	private static extern bool runChuckCode( System.UInt32 chuckID, System.String code );


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
	}
}