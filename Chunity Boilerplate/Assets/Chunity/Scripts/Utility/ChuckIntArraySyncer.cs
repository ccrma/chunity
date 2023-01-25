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

public class ChuckIntArraySyncer : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: SyncFloat
    // desc: start checking chuck for floatToSync so that
    //       later, you can get it whenever you want
    // ----------------------------------------------------
    public void SyncIntArray( ChuckSubInstance chuck, string IntArrayToSync )
    {
        // cancel existing if it's happening
        StopSyncing();

        // start up again
        myChuck = chuck;
        myIntArrayName = IntArrayToSync;
        #if !UNITY_WEBGL
        AllocateCallback();
        #endif
    }




    // ----------------------------------------------------
    // name: GetCurrentArray
    // desc: returns the most recently fetched array
    //       your synced float array
    // ----------------------------------------------------
    public CK_INT[] GetCurrentArray()
    {
        return myIntArray;
    }




    // ----------------------------------------------------
    // name: GetCurrentArrayValue
    // desc: returns the most recently fetched value for
    //       your synced float array at index
    // ----------------------------------------------------
    public CK_INT GetCurrentArrayValue(uint index)
    {
        return myIntArray[index];
    }




    // ----------------------------------------------------
    // name: SetNewArray
    // desc: sends a new value to chuck for your synced float
    // ----------------------------------------------------
    public void SetNewArray( CK_INT[] newArray )
    {
        // set
        if( myChuck != null && myIntArrayName != "" )
        {
            myChuck.SetIntArray( myIntArrayName, (CK_INT[]) newArray );
        }

        // pre-set my storage too
        myIntArray = newArray;
    }




    // ----------------------------------------------------
    // name: SetNewValue
    // desc: sends a new value to chuck for your synced float
    // ----------------------------------------------------
    public void SetNewArrayValue( uint index, CK_INT newValue )
    {
        // set
        if( myChuck != null && myIntArrayName != "" )
        {
            myChuck.SetIntArrayValue( myIntArrayName, index, (CK_INT) newValue );
        }

        // pre-set my storage too
        myIntArray[index] = newValue;
    }




    // ----------------------------------------------------
    // name: StopSyncing
    // desc: stops checking chuck for your float
    // ----------------------------------------------------
    public void StopSyncing()
    {
        myChuck = null;
        myIntArrayName = "";
        #if !UNITY_WEBGL
        ReturnCallback();
        #endif
    }




    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    Chuck.IntArrayCallbackWithID myIntArrayCallback;

    private static Dictionary<CK_INT, ChuckIntArraySyncer> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    CK_INT myID = (CK_INT)(-1);

    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckIntArraySyncer>();
        }
        myID = nextID;
        nextID += 1;
    }


    private void AllocateCallback()
    {
        // regular allocation
        myIntArrayCallback = StaticCallback;
        activeCallbacks[myID] = this;
    }

    private void ReturnCallback()
    {
        // deallocate
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myIntArrayCallback = null;
    }


    string myIntArrayName = "";

    private void Update()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myIntArrayName != "" )
        {
            myChuck.GetIntArray( myIntArrayName, gameObject.name, "MyCallback" );
        }
        #else
        if( myChuck != null && myIntArrayCallback != null && myIntArrayName != "" )
        {
            myChuck.GetIntArray( myIntArrayName, myIntArrayCallback, myID );
        }
        #endif
    }

    private CK_INT[] myIntArray = {};
    private void MyCallback( CK_INT[] newArray )
    {
        myIntArray = newArray;
    }

    void OnDestroy()
    {
        StopSyncing();
    }

    #if AOT
    [AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id, CK_INT[] newValue, CK_UINT size)
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback( newValue );
        }
    }
    
}
