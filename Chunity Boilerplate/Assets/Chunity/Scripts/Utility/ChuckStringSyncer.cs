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
    Chuck.StringCallbackWithID myStringCallback;

    private static Dictionary<CK_INT, ChuckStringSyncer> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    private CK_INT myID;
    

    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckStringSyncer>();
        }
        myID = nextID;
        nextID += 1;
    }


    private void AllocateCallback()
    {
        // register
        activeCallbacks[myID] = this;
        myStringCallback = StaticCallback;
    }

    private void ReturnCallback()
    {
        // deregister
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myStringCallback = null;
    }



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
            myChuck.GetString( myStringName, myStringCallback, myID );
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

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id, string newValue )
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback( newValue );
        }
    }

}
