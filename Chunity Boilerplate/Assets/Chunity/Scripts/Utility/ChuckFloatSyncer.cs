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
    Chuck.FloatCallbackWithID myFloatCallback;

    private static Dictionary<CK_INT, ChuckFloatSyncer> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    CK_INT myID = (CK_INT)(-1);

    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckFloatSyncer>();
        }
        myID = nextID;
        nextID += 1;
    }


    private void AllocateCallback()
    {
        // regular allocation
        myFloatCallback = StaticCallback;
        activeCallbacks[myID] = this;
    }

    private void ReturnCallback()
    {
        // deallocate
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myFloatCallback = null;
    }


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
            myChuck.GetFloat( myFloatName, myFloatCallback, myID );
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

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id, CK_FLOAT newValue )
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback( newValue );
        }
    }
    
}
