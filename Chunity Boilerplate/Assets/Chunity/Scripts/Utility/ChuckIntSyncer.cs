using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
using CK_FLOAT = System.Single;
#else
using CK_INT = System.Int64;
using CK_UINT = System.UInt64;
using CK_FLOAT = System.Double;
#endif

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
        myIntCallback = Chuck.CreateGetIntCallback( MyCallback );
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
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    Chuck.IntCallback myIntCallback;
    string myIntName = "";

    private void Update()
    {
        if( myChuck != null && myIntCallback != null && myIntName != "" )
        {
            myChuck.GetInt( myIntName, myIntCallback );
        }
    }

    private int myIntValue = 0;
    [AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
    private void MyCallback( CK_INT newValue )
    {
        myIntValue = (int) newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }
}
