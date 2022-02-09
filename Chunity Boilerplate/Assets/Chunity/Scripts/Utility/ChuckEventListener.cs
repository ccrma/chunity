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

public class ChuckEventListener : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //


    // ----------------------------------------------------
    // name: ListenForEvent
    // desc: start checking chuck for eventToListenFor so
    //       that callback will be called when the event
    //       broadcasts (or signals, if it's its turn)
    // ----------------------------------------------------
    public void ListenForEvent( ChuckSubInstance chuck, string eventToListenFor, Action callback )
    {
        // cancel existing if it's happening
        StopListening();

        // start up again
        userCallback = callback;
        myChuck = chuck;
        myEventName = eventToListenFor;
        #if UNITY_WEBGL
        myChuck.StartListeningForChuckEvent( myEventName, gameObject.name, "MyDirectCallback" );
        #else
        AllocateCallback();
        myChuck.StartListeningForChuckEvent( myEventName, myVoidCallback, myID );
        #endif
    }


    // ----------------------------------------------------
    // name: StopListening
    // desc: stop calling the previous callback that was
    //       set up with ListenForEvent
    // ----------------------------------------------------
    public void StopListening()
    {
        #if UNITY_WEBGL
        if( myChuck != null && myEventName != "" )
        {
            myChuck.StopListeningForChuckEvent( myEventName, gameObject.name, "MyDirectCallback" );
        }
        #else
        if( myChuck != null && myVoidCallback != null )
        {
            myChuck.StopListeningForChuckEvent( myEventName, myVoidCallback, myID );
        }
        ReturnCallback();
        #endif

        myChuck = null;
        myEventName = "";
    }





    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    string myEventName = "";

    private static Dictionary<CK_INT, ChuckEventListener> activeCallbacks;
    private static CK_INT nextID = (CK_INT)0;
    private CK_INT myID = (CK_INT)(-1);
    private Chuck.VoidCallbackWithID myVoidCallback;


    private void Awake()
    {
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<CK_INT, ChuckEventListener>();
        }
        myID = nextID;
        nextID += 1;
    }

    private void Update()
    {
        while( numTimesCalled > 0 )
        {
            userCallback();
            numTimesCalled--;
        }
    }



    private void AllocateCallback()
    {
        
        // regular allocation
        myVoidCallback = ChuckEventListener.StaticCallback;
        activeCallbacks[myID] = this;
    }

    private void ReturnCallback()
    {
        // stop tracking
        activeCallbacks.Remove( myID );
        // always set my callback to null
        myVoidCallback = null;
    }

    private Action userCallback;

    private int numTimesCalled = 0;
    
    private void MyCallback()
    {
        numTimesCalled++;
    }

    private void MyDirectCallback()
    {
        userCallback();
    }


    void OnDestroy()
    {
        StopListening();
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallbackWithID))]
    #endif
    private static void StaticCallback( CK_INT id )
    {
        if( activeCallbacks.ContainsKey( id ) )
        {
            activeCallbacks[id].MyCallback();
        }
    }

}
