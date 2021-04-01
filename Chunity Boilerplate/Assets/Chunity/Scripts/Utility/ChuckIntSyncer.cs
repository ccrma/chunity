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

public class ChuckIntSyncer : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: SyncInt
    // desc: start checking chuck for intToSync so that
    //       later, you can get it whenever you want
    // ----------------------------------------------------
    public void SyncInt( ChuckSubInstance chuck, string intToSync )
    {
        // cancel existing if it's happening
        StopSyncing();

        // start up again
        myChuck = chuck;
        myIntName = intToSync;
        #if !UNITY_WEBGL
        AllocateCallback();
        #endif
    }




    // ----------------------------------------------------
    // name: GetCurrentValue
    // desc: returns the most recently fetched value for
    //       your synced int
    // ----------------------------------------------------
    public int GetCurrentValue()
    {
        return myIntValue;
    }




    // ----------------------------------------------------
    // name: SetNewValue
    // desc: sends a new value to chuck for your synced int
    // ----------------------------------------------------
    public void SetNewValue( int newValue )
    {
        // set
        if( myChuck != null && myIntName != "" )
        {
            myChuck.SetInt( myIntName, (CK_INT) newValue );
        }

        // pre-set my storage too
        myIntValue = newValue;
    }



    // ----------------------------------------------------
    // name: StopSyncing
    // desc: stops checking chuck for your int
    // ----------------------------------------------------
    public void StopSyncing()
    {
        myChuck = null;
        myIntName = "";

        #if !UNITY_WEBGL
        ReturnCallback();
        #endif
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    #if !UNITY_WEBGL
    Chuck.IntCallback myIntCallback;

    #if UNITY_IOS && !UNITY_EDITOR
    // This version of the class can only be used with a 
    // fixed number of this component, but its callbacks
    // are static, which means it can be used on iOS.
    const int numCallbacks = 8;

    int myCallbackNumber = -1;
    private static HashSet<int> availableIndices;
    private static Dictionary<int, ChuckIntSyncer> activeCallbacks;
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
            activeCallbacks = new Dictionary<int, ChuckIntSyncer>();
        }
        #endif
    }


    private void AllocateCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        // iOS allocation
        if( availableIndices.Count == 0 )
        {
            throw new Exception( "Ran out of callbacks in ChuckIntSyncer" );
        }
        myCallbackNumber = availableIndices.First();
        availableIndices.Remove( myCallbackNumber );
        activeCallbacks[ myCallbackNumber ] = this;
        myIntCallback = GetMyCallback( myCallbackNumber );
        #else
        // regular allocation
        myIntCallback = MyCallback;
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
        myIntCallback = null;
    }

    #endif // !UNITY_WEBGL

    string myIntName = "";

    private void Update()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myIntName != "" )
        {
            myChuck.GetInt( myIntName, gameObject.name, "MyCallback" );
        }
        #else
        if( myChuck != null && myIntCallback != null && myIntName != "" )
        {
            myChuck.GetInt( myIntName, myIntCallback );
        }
        #endif
    }

    private int myIntValue = 0;
    private void MyCallback( CK_INT newValue )
    {
        myIntValue = (int) newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }


    #if UNITY_IOS && !UNITY_EDITOR
    private static Chuck.IntCallback GetMyCallback( int myNumber )
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
    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback0( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 0 ) )
        {
            activeCallbacks[0].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback1( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 1 ) )
        {
            activeCallbacks[1].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback2( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 2 ) )
        {
            activeCallbacks[2].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback3( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 3 ) )
        {
            activeCallbacks[3].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback4( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 4 ) )
        {
            activeCallbacks[4].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback5( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 5 ) )
        {
            activeCallbacks[5].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback6( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 6 ) )
        {
            activeCallbacks[6].myIntValue = (int) newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private static void Callback7( CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( 7 ) )
        {
            activeCallbacks[7].myIntValue = (int) newValue;
        }
    }

    

    #endif // UNITY_IOS
}
