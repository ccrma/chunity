using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

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
public class ChuckMainInstance : MonoBehaviour
{



    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: useMicrophone
    // desc: If false, ChucK will not set up the microphone
    //       for the adc variable.
    // ----------------------------------------------------
    [Tooltip( "Whether ChucK should attempt to search for and set up a microphone." )]
    public bool useMicrophone = true;
    
    
    
    
    // ----------------------------------------------------
    // name: microphoneIdentifier
    // desc: ChucK will search all your mic devices
    //       for one containing this substring.
    //       If left blank, will use the default device
    // ----------------------------------------------------
    [Tooltip( "A substring to search for in your microphone devices list." )]
    public string microphoneIdentifier = "";




    // ----------------------------------------------------
    // name: persistToNextScene
    // desc: this ChucK will not be deleted upon a 
    //       scene load if this bool is true.
    //       If left false, will be delete as usual.
    // ----------------------------------------------------
    [Tooltip( "Whether to keep this ChuckMainInstance when the next scene loads." )]
    private bool persistToNextScene = false;




    // ----------------------------------------------------
    // name: clearChuckOnSceneLoad
    // desc: if this ChucK is not fully deleted on a 
    //       scene load, and this bool is true, then
    //       its VM will be cleared / reset.
    //       Otherwise, the VM will continue running
    //       in the next scene.
    // ----------------------------------------------------
    [Tooltip( "If this ChuckMainInstance is kept when the next scene loads, this controls whether its VM is cleared (reset)." )]
    private bool clearChuckOnSceneLoad = false;




    // ----------------------------------------------------
    // name: RunCode
    // desc: add a new ChucK program to this VM
    // ----------------------------------------------------
    public bool RunCode( string code )
    {
        return Chuck.Manager.RunCode( myChuckId, code );
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
        return Chuck.Manager.RunFile( myChuckId, filename, fromStreamingAssets );
    }





    // ----------------------------------------------------
    // name: RunFile
    // desc: add a new ChucK program to this VM, from
    //       filename, with colonSeparatedArgs.
    //       (fromStreamingAssets == true -->
    //       prepend the location of the StreamingAssets
    //       folder to the filename)
    // ----------------------------------------------------
    public bool RunFile( string filename, string colonSeparatedArgs, bool fromStreamingAssets = true )
    {
        return Chuck.Manager.RunFile( myChuckId, filename, colonSeparatedArgs, fromStreamingAssets );
    }




    // ----------------------------------------------------
    // name: SetInt
    // desc: set the value of global int variableName
    // ----------------------------------------------------
    public bool SetInt( string variableName, CK_INT value )
    {
        return Chuck.Manager.SetInt( myChuckId, variableName, value );
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
        return Chuck.Manager.GetInt( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetInt
    // desc: eventually call the callback with the name
    //       of the variable and the value 
    //       of global int variableName
    // ----------------------------------------------------
    public bool GetInt( string variableName, Chuck.NamedIntCallback callback )
    {
        return Chuck.Manager.GetInt( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetInt
    // desc: eventually call the callback with the name
    //       of the variable and the value 
    //       of global int variableName
    // ----------------------------------------------------
    public bool GetInt( string variableName, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetInt( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetFloat
    // desc: set the value of global float variableName
    // ----------------------------------------------------
    public bool SetFloat( string variableName, CK_FLOAT value )
    {
        return Chuck.Manager.SetFloat( myChuckId, variableName, value );
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
        return Chuck.Manager.GetFloat( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloat
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global float variableName
    // ----------------------------------------------------
    public bool GetFloat( string variableName, Chuck.NamedFloatCallback callback )
    {
        return Chuck.Manager.GetFloat( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloat
    // desc: eventually call the callback with the
    //       name of the variable and the value 
    //       of global float variableName
    // ----------------------------------------------------
    public bool GetFloat( string variableName, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetFloat( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetString
    // desc: set the value of global string variableName
    // ----------------------------------------------------
    public bool SetString( string variableName, System.String value )
    {
        return Chuck.Manager.SetString( myChuckId, variableName, value );
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
        return Chuck.Manager.GetString( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetString
    // desc: eventually call the callback with the 
    //       name of the variable and the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.NamedStringCallback callback )
    {
        return Chuck.Manager.GetString( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetString
    // desc: eventually call the callback with the 
    //       name of the variable and the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.StringCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetString( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SignalEvent
    // desc: call .signal() on global Event variableName
    //       (awake the next listener)
    // ----------------------------------------------------
    public bool SignalEvent( string variableName )
    {
        return Chuck.Manager.SignalEvent( myChuckId, variableName );
    }




    // ----------------------------------------------------
    // name: BroadcastEvent
    // desc: call .broadcast() on global Event variableName
    //       (awake all listeners)
    // ----------------------------------------------------
    public bool BroadcastEvent( string variableName )
    {
        return Chuck.Manager.BroadcastEvent( myChuckId, variableName );
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
        return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.NamedVoidCallback callback )
    {
        return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return Chuck.Manager.StartListeningForChuckEvent( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.NamedVoidCallback callback )
    {
        return Chuck.Manager.StartListeningForChuckEvent( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: StartListeningForChuckEvent
    // desc: call the callback every time that 
    //       global Event variableName signals it
    //       (until cancelled)
    // ----------------------------------------------------
    public bool StartListeningForChuckEvent( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.StartListeningForChuckEvent( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.NamedVoidCallback callback )
    {
        return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, callback, callbackID );
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
        return Chuck.Manager.SetIntArray( myChuckId, variableName, values );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.IntArrayCallback callback )
    {
        return Chuck.Manager.GetIntArray( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the name and value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.NamedIntArrayCallback callback )
    {
        return Chuck.Manager.GetIntArray( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArray
    // desc: get the name and value of global int variableName[]
    // ----------------------------------------------------
    public bool GetIntArray( string variableName, Chuck.IntArrayCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetIntArray( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetIntArrayValue
    // desc: set the value of global int variableName[index]
    // ----------------------------------------------------
    public bool SetIntArrayValue( string variableName, uint index, CK_INT value )
    {
        return Chuck.Manager.SetIntArrayValue( myChuckId, variableName, index, value );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.IntCallback callback )
    {
        return Chuck.Manager.GetIntArrayValue( myChuckId, variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the name and value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.NamedIntCallback callback )
    {
        return Chuck.Manager.GetIntArrayValue( myChuckId, variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetIntArrayValue
    // desc: get the name and value of global int variableName[index]
    // ----------------------------------------------------
    public bool GetIntArrayValue( string variableName, uint index, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetIntArrayValue( myChuckId, variableName, index, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetAssociativeIntArrayValue
    // desc: set the value of global int variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeIntArrayValue( string variableName, string key, CK_INT value )
    {
        return Chuck.Manager.SetAssociativeIntArrayValue( myChuckId, variableName, key, value );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.IntCallback callback )
    {
        return Chuck.Manager.GetAssociativeIntArrayValue( myChuckId, variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the name and value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.NamedIntCallback callback )
    {
        return Chuck.Manager.GetAssociativeIntArrayValue( myChuckId, variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeIntArrayValue
    // desc: get the name and value of global int variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeIntArrayValue( string variableName, string key, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetAssociativeIntArrayValue( myChuckId, variableName, key, callback, callbackID );
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
        return Chuck.Manager.SetFloatArray( myChuckId, variableName, values );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.FloatArrayCallback callback )
    {
        return Chuck.Manager.GetFloatArray( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the name and value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.NamedFloatArrayCallback callback )
    {
        return Chuck.Manager.GetFloatArray( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArray
    // desc: get the name and value of global float variableName[]
    // ----------------------------------------------------
    public bool GetFloatArray( string variableName, Chuck.FloatArrayCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetFloatArray( myChuckId, variableName, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetFloatArrayValue
    // desc: set the value of global float variableName[index]
    // ----------------------------------------------------
    public bool SetFloatArrayValue( string variableName, uint index, CK_FLOAT value )
    {
        return Chuck.Manager.SetFloatArrayValue( myChuckId, variableName, index, value );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.FloatCallback callback )
    {
        return Chuck.Manager.GetFloatArrayValue( myChuckId, variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the name and value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.NamedFloatCallback callback )
    {
        return Chuck.Manager.GetFloatArrayValue( myChuckId, variableName, index, callback );
    }




    // ----------------------------------------------------
    // name: GetFloatArrayValue
    // desc: get the name and value of global float variableName[index]
    // ----------------------------------------------------
    public bool GetFloatArrayValue( string variableName, uint index, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetFloatArrayValue( myChuckId, variableName, index, callback, callbackID );
    }




    // ----------------------------------------------------
    // name: SetAssociativeFloatArrayValue
    // desc: set the value of global float variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeFloatArrayValue( string variableName, string key, CK_FLOAT value )
    {
        return Chuck.Manager.SetAssociativeFloatArrayValue( myChuckId, variableName, key, value );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.FloatCallback callback )
    {
        return Chuck.Manager.GetAssociativeFloatArrayValue( myChuckId, variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the name and value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.NamedFloatCallback callback )
    {
        return Chuck.Manager.GetAssociativeFloatArrayValue( myChuckId, variableName, key, callback );
    }




    // ----------------------------------------------------
    // name: GetAssociativeFloatArrayValue
    // desc: get the name and value of global float variableName[key]
    // ----------------------------------------------------
    public bool GetAssociativeFloatArrayValue( string variableName, string key, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return Chuck.Manager.GetAssociativeFloatArrayValue( myChuckId, variableName, key, callback, callbackID );
    }




#if UNITY_WEBGL
    // method calls specific to WebGL
    public bool GetInt( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetInt( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool GetFloat( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetFloat( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool GetString( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetString( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool ListenForChuckEventOnce( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool StartListeningForChuckEvent( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.StartListeningForChuckEvent( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool StopListeningForChuckEvent( string variableName, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, gameObjectWithCallback, callback );
    }

    public bool GetIntArrayValue( string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetIntArrayValue( myChuckId, variableName, index, gameObjectWithCallback, callback );
    }

    public bool GetAssociativeIntArrayValue( string variableName, string key, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetAssociativeIntArrayValue( myChuckId, variableName, key, gameObjectWithCallback, callback );
    }

    public bool GetFloatArrayValue( string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetFloatArrayValue( myChuckId, variableName, index, gameObjectWithCallback, callback );
    }

    public bool GetAssociativeFloatArrayValue( string variableName, string key, string gameObjectWithCallback, string callback )
    {
        return Chuck.Manager.GetAssociativeFloatArrayValue( myChuckId, variableName, key, gameObjectWithCallback, callback );
    }
#endif





    // =========== INTERNAL MECHANICS ========== //

    private AudioSource mySource;
    private AudioClip micClip;
    private string myMicDevice;

    private System.UInt32 myChuckId = System.UInt32.MaxValue;
    private int currentVar = 0;
    private long numSamplesSeen = 0;

    private float[] myOutBuffer;
    private int myNumChannels = 2;
    private int myBufferLength;

    private bool hasInit = false;



    void Awake()
    {
        // create a chuck
        myChuckId = Chuck.Manager.InitializeFilter();

        // initialize my buffer
        int numBuffers;
        AudioSettings.GetDSPBufferSize( out myBufferLength, out numBuffers );
        myOutBuffer = new float[myBufferLength * myNumChannels];

        // setup group for reliable ordering
        mySource = GetComponent<AudioSource>();
        mySource.outputAudioMixerGroup = Chuck.FindAudioMixerGroup( "ChuckMainInstanceDestination" );

        // setup mic
        if( useMicrophone )
        {
            SetupMic();
        }

        #if UNITY_WEBGL
        // setup listener
        SetUpListener();
        #endif

        // has init
        hasInit = true;
    }

    public System.UInt32 GetID()
    {
        return myChuckId;
    }

    public bool HasInit()
    {
        return hasInit;
    }

    private void SetupMic()
    {
        // default device
        myMicDevice = "";
        #if UNITY_WEBGL
        // TODO
        // pass; could setup later if I wanted from chuckscript
        #else
        // try to find one that matches identifier
        if( microphoneIdentifier != "" )
        {
            foreach( string device in Microphone.devices )
            {
                if( device.Contains( microphoneIdentifier ) )
                {
                    myMicDevice = device;
                }
            }
        }

        // make a clip that loops recording when it reaches the end, is 10 seconds long, and uses the project sample rate
        micClip = Microphone.Start( myMicDevice, true, 10, AudioSettings.GetConfiguration().sampleRate );

        mySource.clip = micClip;
        // also loop the audio source
        mySource.loop = true;
        // high priority!
        mySource.priority = 0;
        // wait for mic to start
        while( !( Microphone.GetPosition( myMicDevice ) > 0 ) ) { };
        // play audio source!
        mySource.Play();
        #endif
    }

    #if UNITY_WEBGL
    private void SetUpListener()
    {
        AudioListener theListener = FindObjectOfType< AudioListener >();
        ChuckListenerPosition listener = theListener.GetComponent< ChuckListenerPosition >();
        if( listener == null )
        {
            // it doesn't exist --> we need to create it
            theListener.gameObject.AddComponent< ChuckListenerPosition >();
        }
    }
    #endif

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

    #if UNITY_WEBGL
    #else
    void OnAudioFilterRead( float[] data, int channels )
    {
        // check whether channels is correct
        if( channels != myNumChannels )
        {
            // sadness -- num channels has changed so we must reconstruct myOutBuffer
            myNumChannels = channels;
            myOutBuffer = new float[myBufferLength * myNumChannels];
        }

        // advance the MasterInstance to the now we need
        if( Chuck.Manager.ManualAudioCallback( myChuckId, data, myOutBuffer, Convert.ToUInt32( channels ) ) )
        {
            numSamplesSeen += data.Length / channels;
        }
        else
        {
            // be silent when chuck fails
            Array.Clear( myOutBuffer, 0, myOutBuffer.Length );
        }

        // copy output back to data, which is now output
        Array.Copy( myOutBuffer, data, data.Length );
    }
    #endif

    // unused for the moment
    private bool Advance( long now, float[] input, float[] output, uint channels )
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

    public bool GetUGenSamples( string variableName, float[] buffer, int numSamples )
    {
        return Chuck.Manager.GetUGenSamples( myChuckId, variableName, buffer, numSamples );
    }

    public bool RunCodeWithReplacementDac( string code, string replacementDac )
    {
        return Chuck.Manager.RunCodeWithReplacementDac( myChuckId, code, replacementDac );
    }

    public bool RunFileWithReplacementDac( string filename, string replacementDac, bool fromStreamingAssets )
    {
        return Chuck.Manager.RunFileWithReplacementDac( myChuckId, filename, replacementDac, fromStreamingAssets );
    }

    public bool RunFileWithReplacementDac( string filename, string args, string replacementDac, bool fromStreamingAssets )
    {
        return Chuck.Manager.RunFileWithReplacementDac( myChuckId, filename, args, replacementDac, fromStreamingAssets );
    }

    private void OnDestroy()
    {
        Chuck.Manager.CleanupFilter( myChuckId );
    }
}
