using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
#else
using CK_INT = System.Int64;
using CK_UINT = System.UInt64;
#endif
using CK_FLOAT = System.Double;

public class ChuckFloatSyncer : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: SyncFloat
    // desc: start checking chuck for floatToSync so that
    //       later, you can get it whenever you want
    // ----------------------------------------------------
    public void SyncFloat( ChuckSubInstance chuck, string floatToSync )
    {
        // cancel existing if it's happening
        StopSyncing();

        // start up again
        myChuck = chuck;
        myFloatName = floatToSync;
        #if !UNITY_WEBGL
        AllocateCallback();
        #endif
    }




    // ----------------------------------------------------
    // name: GetCurrentValue
    // desc: returns the most recently fetched value for
    //       your synced float
    // ----------------------------------------------------
    public float GetCurrentValue()
    {
        return myFloatValue;
    }




    // ----------------------------------------------------
    // name: SetNewValue
    // desc: sends a new value to chuck for your synced float
    // ----------------------------------------------------
    public void SetNewValue( float newValue )
    {
        // set
        if( myChuck != null && myFloatName != "" )
        {
            myChuck.SetFloat( myFloatName, (CK_FLOAT) newValue );
        }

        // pre-set my storage too
        myFloatValue = newValue;
    }



    // ----------------------------------------------------
    // name: StopSyncing
    // desc: stops checking chuck for your float
    // ----------------------------------------------------
    public void StopSyncing()
    {
        myChuck = null;
        myFloatName = "";
        #if !UNITY_WEBGL
        ReturnCallback();
        #endif
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    #if !UNITY_WEBGL
    Chuck.FloatCallback myFloatCallback;

    #if UNITY_IOS && !UNITY_EDITOR
    // This version of the class can only be used with a 
    // fixed number of this component, but its callbacks
    // are static, which means it can be used on iOS.
    const int numCallbacks = 8;

    int myCallbackNumber = -1;
    private static HashSet<int> availableIndices;
    private static Dictionary<int, ChuckFloatSyncer> activeCallbacks;
    #endif // UNITY_IOS

    private void Awake()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        if( availableIndices == null )
        {
            availableIndices = new HashSet<int>();
            for( int i = 0; i < numCallbacks; i++ )
            {
                availableIndices.Add( i );
            }
        }
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<int, ChuckFloatSyncer>();
        }
        #endif
    }


    private void AllocateCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        // iOS allocation
        if( availableIndices.Count == 0 )
        {
            throw new Exception( "Ran out of callbacks in ChuckEventListener" );
        }
        myCallbackNumber = availableIndices.First();
        availableIndices.Remove( myCallbackNumber );
        activeCallbacks[ myCallbackNumber ] = this;
        myFloatCallback = GetMyCallback( myCallbackNumber );
        #else
        // regular allocation
        myFloatCallback = MyCallback;
        #endif
    }

    private void ReturnCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        if( activeCallbacks.Remove( myCallbackNumber ) )
        {
            availableIndices.Add( myCallbackNumber );
        }
        myCallbackNumber = -1;
        #endif
        // always set my callback to null
        myFloatCallback = null;
    }


    #endif // !UNITY_WEBGL
    string myFloatName = "";

    private void Update()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myFloatName != "" )
        {
            myChuck.GetFloat( myFloatName, gameObject.name, "MyCallback" );
        }
        #else
        if( myChuck != null && myFloatCallback != null && myFloatName != "" )
        {
            myChuck.GetFloat( myFloatName, myFloatCallback );
        }
        #endif
    }

    private float myFloatValue = 0;
    private void MyCallback( CK_FLOAT newValue )
    {
        myFloatValue = (float) newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }


    #if UNITY_IOS && !UNITY_EDITOR
    private static Chuck.FloatCallback GetMyCallback( int myNumber )
    {
        switch( myNumber )
        {
            case 0: return Callback0;
            case 1: return Callback1;
            case 2: return Callback2;
            case 3: return Callback3;
            case 4: return Callback4;
            case 5: return Callback5;
            case 6: return Callback6;
            case 7: return Callback7;
            default: return null;
        }
    }
    
    // dumb repetitive code to get around the fact that
    // we have to use static callbacks for iOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback0( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 0 ) )
        {
            activeCallbacks[0].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback1( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 1 ) )
        {
            activeCallbacks[1].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback2( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 2 ) )
        {
            activeCallbacks[2].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback3( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 3 ) )
        {
            activeCallbacks[3].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback4( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 4 ) )
        {
            activeCallbacks[4].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback5( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 5 ) )
        {
            activeCallbacks[5].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback6( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 6 ) )
        {
            activeCallbacks[6].myFloatValue = (float) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private static void Callback7( CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( 7 ) )
        {
            activeCallbacks[7].myFloatValue = (float) newValue;
        }
    }

    

    #endif // UNITY_IOS
}
