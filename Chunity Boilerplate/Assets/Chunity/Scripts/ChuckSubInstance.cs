using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
#elif UNITY_ANDROID
using CK_INT = System.IntPtr;
using CK_UINT = System.UIntPtr;
#else
using CK_INT = System.Int64;
using CK_UINT = System.UInt64;
#endif
using CK_FLOAT = System.Double;


[RequireComponent( typeof( AudioSource ) )]
public class ChuckSubInstance : MonoBehaviour
{

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
    // name: RunFile
    // desc: add a new ChucK program to this VM, from
    //       filename. 
    //       (fromStreamingAssets == true -->
    //       prepend the location of the StreamingAssets
    //       folder to the filename)
    // ----------------------------------------------------
    public bool RunFile( string filename, bool fromStreamingAssets = true )
    {
        return chuckMainInstance.RunFileWithReplacementDac(
            filename, myOutputUgen, fromStreamingAssets );
    }





    // ----------------------------------------------------
    // name: RunFile
    // desc: add a new ChucK program to this VM, from
    //       filename, with colonSeparatedArgs.
    //       (fromStreamingAssets == true -->
    //       prepend the location of the StreamingAssets
    //       folder to the filename)
    // ----------------------------------------------------
    public bool RunFile( string filename, string colonSeparatedArgs,
        bool fromStreamingAssets = true )
    {
        return chuckMainInstance.RunFileWithReplacementDac(
            filename, colonSeparatedArgs, myOutputUgen, fromStreamingAssets );
    }




    // ----------------------------------------------------
    // name: SetInt
    // desc: set the value of global int variableName
    // ----------------------------------------------------
    public bool SetInt( string variableName, CK_INT value )
    {
        return chuckMainInstance.SetInt( variableName, value );
    }




    // ----------------------------------------------------
    // name: CreateGetIntCallback
    // desc: construct the callback necessary for GetInt
    // ----------------------------------------------------
    public Chuck.IntCallback CreateGetIntCallback( Action<CK_INT> callbackFunction )
    {
        return Chuck.CreateGetIntCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetIntCallback
    // desc: construct the callback necessary for GetInt
    // ----------------------------------------------------
    public Chuck.NamedIntCallback CreateGetIntCallback( Action<string, CK_INT> callbackFunction )
    {
        return Chuck.CreateNamedGetIntCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetIntCallback
    // desc: construct the callback necessary for GetInt
    // ----------------------------------------------------
    public Chuck.IntCallbackWithID CreateGetIntCallback( Action<CK_INT, CK_INT> callbackFunction )
    {
        return Chuck.CreateIDGetIntCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: GetInt
    // desc: eventually call the callback with the value 
    //       of global int variableName
    // ----------------------------------------------------
    public bool GetInt( string variableName, Chuck.IntCallback callback )
    {
        return chuckMainInstance.GetInt( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetInt
    // desc: eventually call the callback with the 
    //       name of the variable and the value 
    //       of global int variableName
    // ----------------------------------------------------
    public bool GetInt( string variableName, Chuck.NamedIntCallback callback )
    {
        return chuckMainInstance.GetInt( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetInt
    // desc: eventually call the callback with the 
    //       name of the variable and the value 
    //       of global int variableName
    // ----------------------------------------------------
    public bool GetInt( string variableName, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetInt( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetFloat
    // desc: set the value of global float variableName
    // ----------------------------------------------------
    public bool SetFloat( string variableName, CK_FLOAT value )
    {
        return chuckMainInstance.SetFloat( variableName, value );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatCallback
    // desc: construct the callback necessary for GetFloat
    // ----------------------------------------------------
    public Chuck.FloatCallback CreateGetFloatCallback( Action<CK_FLOAT> callbackFunction )
    {
        return Chuck.CreateGetFloatCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatCallback
    // desc: construct the callback necessary for GetFloat
    // ----------------------------------------------------
    public Chuck.NamedFloatCallback CreateGetFloatCallback( Action<string, CK_FLOAT> callbackFunction )
    {
        return Chuck.CreateNamedGetFloatCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatCallback
    // desc: construct the callback necessary for GetFloat
    // ----------------------------------------------------
    public Chuck.FloatCallbackWithID CreateGetFloatCallback( Action<CK_INT, CK_FLOAT> callbackFunction )
    {
        return Chuck.CreateIDGetFloatCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: GetFloat
    // desc: eventually call the callback with the value 
    //       of global float variableName
    // ----------------------------------------------------
    public bool GetFloat( string variableName, Chuck.FloatCallback callback )
    {
        return chuckMainInstance.GetFloat( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloat
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global float variableName
    // ----------------------------------------------------
    public bool GetFloat( string variableName, Chuck.NamedFloatCallback callback )
    {
        return chuckMainInstance.GetFloat( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloat
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global float variableName
    // ----------------------------------------------------
    public bool GetFloat( string variableName, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetFloat( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetString
    // desc: set the value of global string variableName
    // ----------------------------------------------------
    public bool SetString( string variableName, System.String value )
    {
        return chuckMainInstance.SetString( variableName, value );
    }




    // ----------------------------------------------------
    // name: CreateGetStringCallback
    // desc: construct the callback necessary for GetString
    // ----------------------------------------------------
    public Chuck.StringCallback CreateGetStringCallback( Action<System.String> callbackFunction )
    {
        return Chuck.CreateGetStringCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetStringCallback
    // desc: construct the callback necessary for GetString
    // ----------------------------------------------------
    public Chuck.NamedStringCallback CreateGetStringCallback( Action<System.String, System.String> callbackFunction )
    {
        return Chuck.CreateNamedGetStringCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetStringCallback
    // desc: construct the callback necessary for GetString
    // ----------------------------------------------------
    public Chuck.StringCallbackWithID CreateGetStringCallback( Action<CK_INT, System.String> callbackFunction )
    {
        return Chuck.CreateIDGetStringCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: GetString
    // desc: eventually call the callback with the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.StringCallback callback )
    {
        return chuckMainInstance.GetString( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetString
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.NamedStringCallback callback )
    {
        return chuckMainInstance.GetString( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetString
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.StringCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetString( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SignalEvent
    // desc: call .signal() on global Event variableName
    //       (awake the next listener)
    // ----------------------------------------------------
    public bool SignalEvent( string variableName )
    {
        return chuckMainInstance.SignalEvent( variableName );
    }




    // ----------------------------------------------------
    // name: BroadcastEvent
    // desc: call .broadcast() on global Event variableName
    //       (awake all listeners)
    // ----------------------------------------------------
    public bool BroadcastEvent( string variableName )
    {
        return chuckMainInstance.BroadcastEvent( variableName );
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
    // name: CreateVoidCallback
    // desc: create the callback necessary for waiting on
    //       chuck events
    // ----------------------------------------------------
    public Chuck.NamedVoidCallback CreateVoidCallback( Action<string> callbackFunction )
    {
        return Chuck.CreateNamedVoidCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateVoidCallback
    // desc: create the callback necessary for waiting on
    //       chuck events
    // ----------------------------------------------------
    public Chuck.VoidCallbackWithID CreateVoidCallback( Action<CK_INT> callbackFunction )
    {
        return Chuck.CreateIDVoidCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
    {
        return chuckMainInstance.ListenForChuckEventOnce( variableName, callback );
    }




    // ----------------------------------------------------
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.NamedVoidCallback callback )
    {
        return chuckMainInstance.ListenForChuckEventOnce( variableName, callback );
    }




    // ----------------------------------------------------
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.ListenForChuckEventOnce( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return chuckMainInstance.StartListeningForChuckEvent( variableName, callback );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.NamedVoidCallback callback )
    {
        return chuckMainInstance.StartListeningForChuckEvent( variableName, callback );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.StartListeningForChuckEvent( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return chuckMainInstance.StopListeningForChuckEvent( variableName, callback );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.NamedVoidCallback callback )
    {
        return chuckMainInstance.StopListeningForChuckEvent( variableName, callback );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.StopListeningForChuckEvent( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: CreateGetIntArrayCallback
    // desc: create a callback for getting an int array
    // ----------------------------------------------------
    public Chuck.IntArrayCallback CreateGetIntArrayCallback( Action<CK_INT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateGetIntArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetIntArrayCallback
    // desc: create a callback for getting an int array with its name
    // ----------------------------------------------------
    public Chuck.NamedIntArrayCallback CreateGetIntArrayCallback( Action<string, CK_INT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateNamedGetIntArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetIntArrayCallback
    // desc: create a callback for getting an int array with its name
    // ----------------------------------------------------
    public Chuck.IntArrayCallbackWithID CreateGetIntArrayCallback( Action<CK_INT, CK_INT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateIDGetIntArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: SetIntArray
    // desc: set the value of global int variableName[]
    // ----------------------------------------------------
    public bool SetIntArray( string variableName, CK_INT[] values )
    {
        return chuckMainInstance.SetIntArray( variableName, values );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.IntArrayCallback callback )
    {
        return chuckMainInstance.GetIntArray( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the name and value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.NamedIntArrayCallback callback )
    {
        return chuckMainInstance.GetIntArray( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the name and value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.IntArrayCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetIntArray( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetIntArrayValue
    // desc: set the value of global int variableName[index]
    // ----------------------------------------------------
    public bool SetIntArrayValue( string variableName, uint index, CK_INT value )
    {
        return chuckMainInstance.SetIntArrayValue( variableName, index, value );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.IntCallback callback )
    {
        return chuckMainInstance.GetIntArrayValue( variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the name and value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.NamedIntCallback callback )
    {
        return chuckMainInstance.GetIntArrayValue( variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the name and value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetIntArrayValue( variableName, index, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetAssociativeIntArrayValue
    // desc: set the value of global int variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeIntArrayValue( string variableName, string key, CK_INT value )
    {
        return chuckMainInstance.SetAssociativeIntArrayValue( variableName, key, value );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.IntCallback callback )
    {
        return chuckMainInstance.GetAssociativeIntArrayValue( variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the name and value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.NamedIntCallback callback )
    {
        return chuckMainInstance.GetAssociativeIntArrayValue( variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the name and value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetAssociativeIntArrayValue( variableName, key, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatArrayCallback
    // desc: create a callback for getting a float array
    // ----------------------------------------------------
    public Chuck.FloatArrayCallback CreateGetFloatArrayCallback( Action<CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateGetFloatArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatArrayCallback
    // desc: create a callback for getting a float array
    // ----------------------------------------------------
    public Chuck.NamedFloatArrayCallback CreateGetFloatArrayCallback( Action<string, CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateNamedGetFloatArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatArrayCallback
    // desc: create a callback for getting a float array
    // ----------------------------------------------------
    public Chuck.FloatArrayCallbackWithID CreateGetFloatArrayCallback( Action<CK_INT, CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return Chuck.CreateIDGetFloatArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: SetFloatArray
    // desc: set the value of global float variableName[]
    // ----------------------------------------------------
    public bool SetFloatArray( string variableName, CK_FLOAT[] values )
    {
        return chuckMainInstance.SetFloatArray( variableName, values );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.FloatArrayCallback callback )
    {
        return chuckMainInstance.GetFloatArray( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the name and value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.NamedFloatArrayCallback callback )
    {
        return chuckMainInstance.GetFloatArray( variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the name and value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.FloatArrayCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetFloatArray( variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetFloatArrayValue
    // desc: set the value of global float variableName[index]
    // ----------------------------------------------------
    public bool SetFloatArrayValue( string variableName, uint index, CK_FLOAT value )
    {
        return chuckMainInstance.SetFloatArrayValue( variableName, index, value );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.FloatCallback callback )
    {
        return chuckMainInstance.GetFloatArrayValue( variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the name and value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.NamedFloatCallback callback )
    {
        return chuckMainInstance.GetFloatArrayValue( variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the name and value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetFloatArrayValue( variableName, index, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetAssociativeFloatArrayValue
    // desc: set the value of global float variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeFloatArrayValue( string variableName, string key, CK_FLOAT value )
    {
        return chuckMainInstance.SetAssociativeFloatArrayValue( variableName, key, value );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.FloatCallback callback )
    {
        return chuckMainInstance.GetAssociativeFloatArrayValue( variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the name and value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.NamedFloatCallback callback )
    {
        return chuckMainInstance.GetAssociativeFloatArrayValue( variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the name and value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return chuckMainInstance.GetAssociativeFloatArrayValue( variableName, key, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetRunning
    // desc: whether the SubInstance is outputting sound
    // ----------------------------------------------------
    public void SetRunning( bool r )
    {
        running = r;
    }




    #if UNITY_WEBGL
    // method calls specific to WebGL
    public bool GetInt( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetInt( variableName, gameObjectWithCallback, callback );
    }

    public bool GetFloat( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetFloat( variableName, gameObjectWithCallback, callback );
    }

    public bool GetString( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetString( variableName, gameObjectWithCallback, callback );
    }

    public bool ListenForChuckEventOnce( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.ListenForChuckEventOnce( variableName, gameObjectWithCallback, callback );
    }

    public bool StartListeningForChuckEvent( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.StartListeningForChuckEvent( variableName, gameObjectWithCallback, callback );
    }

    public bool StopListeningForChuckEvent( string variableName, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.StopListeningForChuckEvent( variableName, gameObjectWithCallback, callback );
    }

    public bool GetIntArrayValue( string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetIntArrayValue( variableName, index, gameObjectWithCallback, callback );
    }

    public bool GetAssociativeIntArrayValue( string variableName, string key, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetAssociativeIntArrayValue( variableName, key, gameObjectWithCallback, callback );
    }

    public bool GetFloatArrayValue( string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetFloatArrayValue( variableName, index, gameObjectWithCallback, callback );
    }

    public bool GetAssociativeFloatArrayValue( string variableName, string key, string gameObjectWithCallback, string callback )
    {
        return chuckMainInstance.GetAssociativeFloatArrayValue( variableName, key, gameObjectWithCallback, callback );
    }
#endif






    // =========== INTERNAL MECHANICS ========== //

    #if UNITY_WEBGL
    private System.UInt32 myID;
    private static System.UInt32 nextID = 1;
    #endif

    private string myOutputUgen;

    private bool running = false;
    private int myNumChannels = 2;
    private int myBufferLength;
    private float[] myMonoBuffer;
    private AudioSource mySource;
    private bool isMuted;
    private bool prevSpatialize;

    private AudioClip spatialClip;

    private long numSamplesSeen = 0;

    // Use this for initialization
    void Awake()
    {
        // if I don't have a ChuckMainInstance at Awake, see if I can 
        // find one in TheChuck
        if( chuckMainInstance == null )
        {
            chuckMainInstance = TheChuck.instance;
        }

        // unity getting stuff
        int numBuffers;
        AudioSettings.GetDSPBufferSize( out myBufferLength, out numBuffers );

        myMonoBuffer = new float[myBufferLength];

        // setup group for reliable ordering
        mySource = GetComponent<AudioSource>();
        #if UNITY_WEBGL
        #else
        mySource.outputAudioMixerGroup = Chuck.FindAudioMixerGroup( "ChuckSubInstanceDestination" );
        // other settings
        mySource.loop = true;
        mySource.playOnAwake = true;
        // medium priority
        mySource.priority = 128;

        spatialClip = (AudioClip) Resources.Load( "Audio/1" );
        mySource.clip = spatialClip;
        mySource.Play();
        #endif


        // setup chuck
        myOutputUgen = chuckMainInstance.GetUniqueVariableName( "__dac__" );

        #if UNITY_WEBGL
        myID = nextID++;
        System.UInt32 mainID = chuckMainInstance.GetID();
        initSubChuckInstance( mainID, myID, myOutputUgen );
        initSpatializer( myID, mySource.minDistance, mySource.maxDistance );
        #else
        // replacement dac is initted and constructed here!
        // so it shouldn't have to be anywhere else.
        chuckMainInstance.RunCode( string.Format( @"
			global Gain {0} => blackhole;
			true => {0}.buffered;
		", myOutputUgen ) );
        #endif

        // opposite to have first UpdateSpatialize() take effect
        prevSpatialize = !spatialize;
        UpdateSpatialize();

        running = true;

    }

    #if UNITY_WEBGL
    [DllImport( "__Internal" )]
    private static extern bool initSubChuckInstance( System.UInt32 chuckID, System.UInt32 subChuckID, System.String dacName );
    [DllImport( "__Internal" )]
    private static extern bool initSpatializer( System.UInt32 subChuckID, float minDistance, float maxDistance );
    [DllImport( "__Internal" )]
    private static extern bool setSubChuckSpatializationParameters( System.UInt32 subChuckID, 
        System.UInt32 doSpatialization, float minDistance, float maxDistance, float rolloffFactor );
    [DllImport( "__Internal" )]
    private static extern bool setSubChuckTransform( System.UInt32 subChuckID, 
        float posX, float posY, float posZ,
        float forwardX, float forwardY, float forwardZ 
    );
    [DllImport( "__Internal" )]
    private static extern bool muteSubChuckInstance( System.UInt32 subChuckID );
    [DllImport( "__Internal" )]
    private static extern bool unMuteSubChuckInstance( System.UInt32 subChuckID );

    void OnDisable()
    {
        muteSubChuckInstance( myID );
    }

    void OnEnable()
    {
        unMuteSubChuckInstance( myID );
    }
    #endif

    void Update()
    {
        isMuted = mySource.mute;
        UpdateSpatialize();

        #if UNITY_WEBGL
        if( transform.hasChanged )
        {
            Vector3 pos = transform.position;
            Vector3 forward = transform.forward;
            setSubChuckTransform( myID, pos.x, pos.y, pos.z, forward.x, forward.y, forward.z );
        }
        #endif
    }

    void UpdateSpatialize()
    {
        if( prevSpatialize == spatialize )
        {
            return;
        }

        #if UNITY_WEBGL
        setSubChuckSpatializationParameters( myID, spatialize ? (uint) 1 : (uint) 0, mySource.minDistance, mySource.maxDistance, 1 );
        #else
        if( spatialize )
        {
            mySource.spatialBlend = 1.0f;
        }
        else
        {
            mySource.spatialBlend = 0.0f;
        }
        #endif
        prevSpatialize = spatialize;
    }

    #if UNITY_WEBGL
    #else
    void OnAudioFilterRead( float[] data, int channels )
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
            myNumChannels = channels;
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
                data[i * channels + j] *= myMonoBuffer[i];
            }
        }
    }
    #endif


    public string GetUniqueVariableName()
    {
        return chuckMainInstance.GetUniqueVariableName();
    }

    public string GetUniqueVariableName( string prefix )
    {
        return chuckMainInstance.GetUniqueVariableName( prefix );
    }

}
