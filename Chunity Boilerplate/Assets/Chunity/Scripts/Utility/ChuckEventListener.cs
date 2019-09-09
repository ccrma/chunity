using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        myVoidCallback = MyCallback;
        myChuck.StartListeningForChuckEvent( myEventName, myVoidCallback );
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
            myChuck.StopListeningForChuckEvent( myEventName, myVoidCallback );
        }
        myVoidCallback = null;
        #endif

        myChuck = null;
        myEventName = "";
    }





    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    string myEventName = "";

    private void Update()
    {
        while( numTimesCalled > 0 )
        {
            userCallback();
            numTimesCalled--;
        }
    }

    #if UNITY_WEBGL
    #else
    private Chuck.VoidCallback myVoidCallback;
    #endif
    private Action userCallback;

    private int numTimesCalled = 0;
    
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
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
}
