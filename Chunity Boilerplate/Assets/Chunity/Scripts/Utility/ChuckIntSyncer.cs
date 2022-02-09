using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    Chuck.IntCallbackWithID myIntCallback;

    private static Dictionary<CK_INT, ChuckIntSyncer> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    private CK_INT myID;
    
    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckIntSyncer>();
        }
        myID = nextID;
        nextID += 1;
    }


    private void AllocateCallback()
    {
        // regular allocation
        activeCallbacks[myID] = this;
        myIntCallback = StaticCallback;
    }

    private void ReturnCallback()
    {
        // deregister
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myIntCallback = null;
    }


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
            myChuck.GetInt( myIntName, myIntCallback, myID );
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

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id, CK_INT newValue )
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback( newValue );
        }
    }

}
