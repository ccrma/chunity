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
        myVoidCallback = Chuck.CreateVoidCallback( MyCallback );
        userCallback = callback;
        myChuck = chuck;
        myEventName = eventToListenFor;
        myChuck.StartListeningForChuckEvent( myEventName, myVoidCallback );
    }


    // ----------------------------------------------------
    // name: StopListening
    // desc: stop calling the previous callback that was
    //       set up with ListenForEvent
    // ----------------------------------------------------
    public void StopListening()
    {
        if( myChuck != null && myVoidCallback != null )
        {
            myChuck.StopListeningForChuckEvent( myEventName, myVoidCallback );
        }
        myChuck = null;
        myVoidCallback = null;
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

    private Chuck.VoidCallback myVoidCallback;
    private Action userCallback;

    private int numTimesCalled = 0;
    private void MyCallback()
    {
        numTimesCalled++;
    }


    void OnDestroy()
    {
        StopListening();
    }
}
