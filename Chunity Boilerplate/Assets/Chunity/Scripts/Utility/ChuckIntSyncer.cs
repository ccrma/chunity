using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            myChuck.SetInt( myIntName, (long) newValue );
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
    private void MyCallback( System.Int64 newValue )
    {
        myIntValue = (int) newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }
}
