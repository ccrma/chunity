using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public bool SetInt( string variableName, System.Int64 value )
    {
        return chuckMainInstance.SetInt( variableName, value );
    }




    // ----------------------------------------------------
    // name: CreateGetIntCallback
    // desc: construct the callback necessary for GetInt
    // ----------------------------------------------------
    public Chuck.IntCallback CreateGetIntCallback( Action<System.Int64> callbackFunction )
    {
        return Chuck.CreateGetIntCallback( callbackFunction );
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
    // name: SetFloat
    // desc: set the value of global float variableName
    // ----------------------------------------------------
    public bool SetFloat( string variableName, double value )
    {
        return chuckMainInstance.SetFloat( variableName, value );
    }




    // ----------------------------------------------------
    // name: CreateGetFloatCallback
    // desc: construct the callback necessary for GetFloat
    // ----------------------------------------------------
    public Chuck.FloatCallback CreateGetFloatCallback( Action<double> callbackFunction )
    {
        return Chuck.CreateGetFloatCallback( callbackFunction );
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
    // name: GetString
    // desc: eventually call the callback with the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.StringCallback callback )
    {
        return chuckMainInstance.GetString( variableName, callback );
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
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
    {
        return chuckMainInstance.ListenForChuckEventOnce( variableName, callback );
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
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return chuckMainInstance.StopListeningForChuckEvent( variableName, callback );
    }




    // ----------------------------------------------------
    // name: CreateGetIntArrayCallback
    // desc: create a callback for getting an int array
    // ----------------------------------------------------
    public Chuck.IntArrayCallback CreateGetIntArrayCallback( Action<long[], ulong> callbackFunction )
    {
        return Chuck.CreateGetIntArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: SetIntArray
    // desc: set the value of global int variableName[]
    // ----------------------------------------------------
    public bool SetIntArray( string variableName, long[] values )
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
    // name: SetIntArrayValue
    // desc: set the value of global int variableName[index]
    // ----------------------------------------------------
    public bool SetIntArrayValue( string variableName, uint index, long value )
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
    // name: SetAssociativeIntArrayValue
    // desc: set the value of global int variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeIntArrayValue( string variableName, string key, long value )
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
    // name: CreateGetFloatArrayCallback
    // desc: create a callback for getting a float array
    // ----------------------------------------------------
    public Chuck.FloatArrayCallback CreateGetFloatArrayCallback( Action<double[], ulong> callbackFunction )
    {
        return Chuck.CreateGetFloatArrayCallback( callbackFunction );
    }




    // ----------------------------------------------------
    // name: SetFloatArray
    // desc: set the value of global float variableName[]
    // ----------------------------------------------------
    public bool SetFloatArray( string variableName, double[] values )
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
    // name: SetFloatArrayValue
    // desc: set the value of global float variableName[index]
    // ----------------------------------------------------
    public bool SetFloatArrayValue( string variableName, uint index, double value )
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
    // name: SetAssociativeFloatArrayValue
    // desc: set the value of global float variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeFloatArrayValue( string variableName, string key, double value )
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
        mySource.outputAudioMixerGroup = Chuck.FindAudioMixerGroup( "ChuckSubInstanceDestination" );
        // other settings
        mySource.loop = true;
        mySource.playOnAwake = true;
        // medium priority
        mySource.priority = 128;

        spatialClip = (AudioClip) Resources.Load( "1" );
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
			global Gain {0} => blackhole;
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


    public string GetUniqueVariableName()
    {
        return chuckMainInstance.GetUniqueVariableName();
    }

    public string GetUniqueVariableName( string prefix )
    {
        return chuckMainInstance.GetUniqueVariableName( prefix );
    }

}
