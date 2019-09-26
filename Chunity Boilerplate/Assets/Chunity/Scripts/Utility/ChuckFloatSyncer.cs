using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        #if UNITY_WEBGL
        #else
        myFloatCallback = MyCallback;
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
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    #if UNITY_WEBGL
    #else
    Chuck.FloatCallback myFloatCallback;
    #endif
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
    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
    private void MyCallback( CK_FLOAT newValue )
    {
        myFloatValue = (float) newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }
}
