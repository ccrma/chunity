using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        #if UNITY_WEBGL
        #else
        myStringCallback = MyCallback;
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
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    #if UNITY_WEBGL
    #else
    Chuck.StringCallback myStringCallback;
    #endif
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
    [AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
    private void MyCallback( string newValue )
    {
        myStringValue = newValue;
    }

    void OnDestroy()
    {
        StopSyncing();
    }
}
