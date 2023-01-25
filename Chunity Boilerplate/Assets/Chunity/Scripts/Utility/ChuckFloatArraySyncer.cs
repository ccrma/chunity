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

public class ChuckFloatArraySyncer : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: SyncFloat
    // desc: start checking chuck for floatToSync so that
    //       later, you can get it whenever you want
    // ----------------------------------------------------
    public void SyncFloatArray( ChuckSubInstance chuck, string floatArrayToSync )
    {
        // cancel existing if it's happening
        StopSyncing();

        // start up again
        myChuck = chuck;
        myFloatArrayName = floatArrayToSync;
        #if !UNITY_WEBGL
        AllocateCallback();
        #endif
    }




    // ----------------------------------------------------
    // name: GetCurrentArray
    // desc: returns the most recently fetched array
    //       your synced float array
    // ----------------------------------------------------
    public CK_FLOAT[] GetCurrentArray()
    {
        return myFloatArray;
    }




    // ----------------------------------------------------
    // name: GetCurrentArrayValue
    // desc: returns the most recently fetched value for
    //       your synced float array at index
    // ----------------------------------------------------
    public CK_FLOAT GetCurrentArrayValue(uint index)
    {
        return myFloatArray[index];
    }




    // ----------------------------------------------------
    // name: SetNewArray
    // desc: sends a new value to chuck for your synced float
    // ----------------------------------------------------
    public void SetNewArray( CK_FLOAT[] newArray )
    {
        // set
        if( myChuck != null && myFloatArrayName != "" )
        {
            myChuck.SetFloatArray( myFloatArrayName, newArray );
        }

        // pre-set my storage too
        myFloatArray = newArray;
    }




    // ----------------------------------------------------
    // name: SetNewValue
    // desc: sends a new value to chuck for your synced float
    // ----------------------------------------------------
    public void SetNewArrayValue( uint index, CK_FLOAT newValue )
    {
        // set
        if( myChuck != null && myFloatArrayName != "" )
        {
            myChuck.SetFloatArrayValue( myFloatArrayName, index, newValue );
        }

        // pre-set my storage too
        myFloatArray[index] = newValue;
    }




    // ----------------------------------------------------
    // name: StopSyncing
    // desc: stops checking chuck for your float
    // ----------------------------------------------------
    public void StopSyncing()
    {
        myChuck = null;
        myFloatArrayName = "";
        #if !UNITY_WEBGL
        ReturnCallback();
        #endif
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    Chuck.FloatArrayCallbackWithID myFloatArrayCallback;

    private static Dictionary<CK_INT, ChuckFloatArraySyncer> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    CK_INT myID = (CK_INT)(-1);

    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckFloatArraySyncer>();
        }
        myID = nextID;
        nextID += 1;
    }


    private void AllocateCallback()
    {
        // regular allocation
        myFloatArrayCallback = StaticCallback;
        activeCallbacks[myID] = this;
    }

    private void ReturnCallback()
    {
        // deallocate
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myFloatArrayCallback = null;
    }


    string myFloatArrayName = "";

    private void Update()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myFloatArrayName != "" )
        {
            myChuck.GetFloatArray( myFloatArrayName, gameObject.name, "MyCallback" );
        }
        #else
        if( myChuck != null && myFloatArrayCallback != null && myFloatArrayName != "" )
        {
            myChuck.GetFloatArray( myFloatArrayName, myFloatArrayCallback, myID );
        }
        #endif
    }

    private CK_FLOAT[] myFloatArray = {};
    private void MyCallback( CK_FLOAT[] newArray )
    {
        myFloatArray = newArray;
    }

    void OnDestroy()
    {
        StopSyncing();
    }

    #if AOT
    [AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id, CK_FLOAT[] newValue, CK_UINT size)
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback( newValue );
        }
    }
    
}
