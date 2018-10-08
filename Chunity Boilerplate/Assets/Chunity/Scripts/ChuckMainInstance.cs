using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;



[RequireComponent( typeof( AudioSource ) )]
public class ChuckMainInstance : MonoBehaviour
{



    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: microphoneIdentifier
    // desc: ChucK will search all your mic devices
    //       for one containing this substring.
    //       If left blank, will use the default device
    // ----------------------------------------------------
    [Tooltip( "A substring to search for in your microphone devices list." )]
    public string microphoneIdentifier = "";




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
    public bool SetInt( string variableName, System.Int64 value )
    {
        return Chuck.Manager.SetInt( myChuckId, variableName, value );
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
        return Chuck.Manager.GetInt( myChuckId, variableName, callback );
    }




    // ----------------------------------------------------
    // name: SetFloat
    // desc: set the value of global float variableName
    // ----------------------------------------------------
    public bool SetFloat( string variableName, double value )
    {
        return Chuck.Manager.SetFloat( myChuckId, variableName, value );
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
        return Chuck.Manager.GetFloat( myChuckId, variableName, callback );
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
    // name: GetString
    // desc: eventually call the callback with the value 
    //       of global string variableName
    // ----------------------------------------------------
    public bool GetString( string variableName, Chuck.StringCallback callback )
    {
        return Chuck.Manager.GetString( myChuckId, variableName, callback );
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
    // name: ListenForChuckEventOnce
    // desc: call the callback only the next time that
    //       global Event variableName signals it
    // ----------------------------------------------------
    public bool ListenForChuckEventOnce( string variableName, Chuck.VoidCallback callback )
    {
        return Chuck.Manager.ListenForChuckEventOnce( myChuckId, variableName, callback );
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
    // name: StopListeningForChuckEvent
    // desc: cancel the callback registered to 
    //       global Event variableName
    // ----------------------------------------------------
    public bool StopListeningForChuckEvent( string variableName, Chuck.VoidCallback callback )
    {
        return Chuck.Manager.StopListeningForChuckEvent( myChuckId, variableName, callback );
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
    // name: SetIntArrayValue
    // desc: set the value of global int variableName[index]
    // ----------------------------------------------------
    public bool SetIntArrayValue( string variableName, uint index, long value )
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
    // name: SetAssociativeIntArrayValue
    // desc: set the value of global int variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeIntArrayValue( string variableName, string key, long value )
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
    // name: SetFloatArrayValue
    // desc: set the value of global float variableName[index]
    // ----------------------------------------------------
    public bool SetFloatArrayValue( string variableName, uint index, double value )
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
    // name: SetAssociativeFloatArrayValue
    // desc: set the value of global float variableName[key]
    // ----------------------------------------------------
    public bool SetAssociativeFloatArrayValue( string variableName, string key, double value )
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
        SetupMic();

        // has init
        hasInit = true;
    }

    public bool HasInit()
    {
        return hasInit;
    }

    private void SetupMic()
    {
        // default device
        myMicDevice = "";
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
