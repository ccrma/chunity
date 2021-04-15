using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ChuckStringSyncer : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: SyncString
    // desc: start checking chuck for stringToSync so that
    //       later, you can get it whenever you want
    // ----------------------------------------------------
    public void SyncString( ChuckSubInstance chuck, string stringToSync )
    {
        // cancel existing if it's happening
        StopSyncing();

        // start up again
        myChuck = chuck;
        myStringName = stringToSync;
        #if !UNITY_WEBGL
        AllocateCallback();
        #endif
    }




    // ----------------------------------------------------
    // name: GetCurrentValue
    // desc: returns the most recently fetched value for
    //       your synced string
    // ----------------------------------------------------
    public string GetCurrentValue()
    {
        return myStringValue;
    }




    // ----------------------------------------------------
    // name: SetNewValue
    // desc: sends a new value to chuck for your synced string
    // ----------------------------------------------------
    public void SetNewValue( string newValue )
    {
        // set
        if( myChuck != null && myStringName != "" )
        {
            myChuck.SetString( myStringName, newValue );
        }

        // pre-set my storage too
        myStringValue = newValue;
    }



    // ----------------------------------------------------
    // name: StopSyncing
    // desc: stops checking chuck for your string
    // ----------------------------------------------------
    public void StopSyncing()
    {
        myChuck = null;
        myStringName = "";

        #if !UNITY_WEBGL
        ReturnCallback();
        #endif
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    #if !UNITY_WEBGL
    Chuck.StringCallback myStringCallback;

    #if UNITY_IOS && !UNITY_EDITOR
    // This version of the class can only be used with a 
    // fixed number of this component, but its callbacks
    // are static, which means it can be used on iOS.
    const int numCallbacks = 8;

    int myCallbackNumber = -1;
    private static HashSet<int> availableIndices;
    private static Dictionary<int, ChuckStringSyncer> activeCallbacks;
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
            activeCallbacks = new Dictionary<int, ChuckStringSyncer>();
        }
        #endif
    }


    private void AllocateCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        // iOS allocation
        if( availableIndices.Count == 0 )
        {
            throw new Exception( "Ran out of callbacks in ChuckStringSyncer" );
        }
        myCallbackNumber = availableIndices.First();
        availableIndices.Remove( myCallbackNumber );
        activeCallbacks[ myCallbackNumber ] = this;
        myStringCallback = GetMyCallback( myCallbackNumber );
        #else
        // regular allocation
        myStringCallback = MyCallback;
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
        myStringCallback = null;
    }

    #endif // !UNITY_WEBGL


    string myStringName = "";

    private void Update()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myStringName != "" )
        {
            myChuck.GetString( myStringName, gameObject.name, "MyCallback" );
        }
        #else
        if( myChuck != null && myStringCallback != null && myStringName != "" )
        {
            myChuck.GetString( myStringName, myStringCallback );
        }
        #endif
    }

    private string myStringValue = "";
    private void MyCallback( string newValue )
    {
        myStringValue = newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }

    #if UNITY_IOS && !UNITY_EDITOR
    private static Chuck.StringCallback GetMyCallback( int myNumber )
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
    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback0( string newValue )
    {
        if( activeCallbacks.ContainsKey( 0 ) )
        {
            activeCallbacks[0].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback1( string newValue )
    {
        if( activeCallbacks.ContainsKey( 1 ) )
        {
            activeCallbacks[1].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback2( string newValue )
    {
        if( activeCallbacks.ContainsKey( 2 ) )
        {
            activeCallbacks[2].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback3( string newValue )
    {
        if( activeCallbacks.ContainsKey( 3 ) )
        {
            activeCallbacks[3].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback4( string newValue )
    {
        if( activeCallbacks.ContainsKey( 4 ) )
        {
            activeCallbacks[4].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback5( string newValue )
    {
        if( activeCallbacks.ContainsKey( 5 ) )
        {
            activeCallbacks[5].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback6( string newValue )
    {
        if( activeCallbacks.ContainsKey( 6 ) )
        {
            activeCallbacks[6].myStringValue = newValue;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private static void Callback7( string newValue )
    {
        if( activeCallbacks.ContainsKey( 7 ) )
        {
            activeCallbacks[7].myStringValue = newValue;
        }
    }

    #endif // UNITY_IOS
}
