using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChuckEventListener2 : MonoBehaviour
{


    // ================= PUBLIC FACING ================== //
    // This version of the class can only be used with a 
    // fixed number of this component, but its callbacks
    // are static, which means it can be used on iOS.
    const int numCallbacks = 8;


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
        // sets myVoidCallback to one from a static pool
        AllocateCallback();
        // listen for event
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
        // resets myVoidCallback
        ReturnCallback();

        myChuck = null;
        myEventName = "";
    }





    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    string myEventName = "";
    int myCallbackNumber = -1;

    private static HashSet<int> availableIndices;
    private static Dictionary<int, ChuckEventListener2> activeCallbacks;

    private void Awake()
    {
        if( availableIndices == null )
        {
            availableIndices = new HashSet<int>();
            for( int i = 0; i < numCallbacks; i++ )
            {
                availableIndices.Add( i );
            }
        }
        if( activeCallbacks == null )
        {
            activeCallbacks = new Dictionary<int, ChuckEventListener2>();
        }
    }

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
    

    void OnDestroy()
    {
        StopListening();
    }

    private void AllocateCallback()
    {
        if( availableIndices.Count == 0 )
        {
            throw new Exception( "Ran out of callbacks in ChuckEventListener2" );
        }
        myCallbackNumber = availableIndices.First();
        availableIndices.Remove( myCallbackNumber );
        activeCallbacks[ myCallbackNumber ] = this;
        myVoidCallback = GetMyCallback( myCallbackNumber );
    }

    private void ReturnCallback()
    {
        activeCallbacks.Remove( myCallbackNumber );
        availableIndices.Add( myCallbackNumber );
        myCallbackNumber = -1;
        myVoidCallback = null;
    }

    
    private static Chuck.VoidCallback GetMyCallback( int myNumber )
    {
        switch( myNumber )
        {
            case 0: return Callback0;
            case 1: return Callback1;
            case 2: return Callback2;
            case 3: return Callback3;
            case 4: return Callback4;
            case 5: return Callback5;
            case 6: return Callback6;
            case 7: return Callback7;
        }
    }
    
    // dumb repetitive code to get around the fact that
    // we have to use static callbacks for iOS

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback0()
    {
        if( activeCallbacks.ContainsKey( 0 ) )
        {
            activeCallbacks[0].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback1()
    {
        if( activeCallbacks.ContainsKey( 1 ) )
        {
            activeCallbacks[1].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback2()
    {
        if( activeCallbacks.ContainsKey( 2 ) )
        {
            activeCallbacks[2].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback3()
    {
        if( activeCallbacks.ContainsKey( 3 ) )
        {
            activeCallbacks[3].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback4()
    {
        if( activeCallbacks.ContainsKey( 4 ) )
        {
            activeCallbacks[4].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback5()
    {
        if( activeCallbacks.ContainsKey( 5 ) )
        {
            activeCallbacks[5].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback6()
    {
        if( activeCallbacks.ContainsKey( 6 ) )
        {
            activeCallbacks[6].numTimesCalled++;
        }
    }

    #if UNITY_IOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    #endif
    private static void Callback7()
    {
        if( activeCallbacks.ContainsKey( 7 ) )
        {
            activeCallbacks[7].numTimesCalled++;
        }
    }

    

}
