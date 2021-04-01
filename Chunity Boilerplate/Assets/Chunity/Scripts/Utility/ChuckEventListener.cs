using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
        ReturnCallback();
        #endif

        myChuck = null;
        myEventName = "";
    }





    // =========== INTERNAL MECHANICS ========== //

    ChuckSubInstance myChuck = null;
    string myEventName = "";

    #if UNITY_IOS && !UNITY_EDITOR
    // This version of the class can only be used with a 
    // fixed number of this component, but its callbacks
    // are static, which means it can be used on iOS.
    const int numCallbacks = 8;

    int myCallbackNumber = -1;
    private static HashSet<int> availableIndices;
    private static Dictionary<int, ChuckEventListener> activeCallbacks;
    #endif

    private void Awake()
    {
        #if UNITY_IOS && !UNITY_EDITOR
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
            activeCallbacks = new Dictionary<int, ChuckEventListener>();
        }
        #endif
    }

    private void Update()
    {
        while( numTimesCalled > 0 )
        {
            userCallback();
            numTimesCalled--;
        }
    }

    #if !UNITY_WEBGL

    private Chuck.VoidCallback myVoidCallback;

    private void AllocateCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        // iOS allocation
        if( availableIndices.Count == 0 )
        {
            throw new Exception( "Ran out of callbacks in ChuckEventListener" );
        }
        myCallbackNumber = availableIndices.First();
        availableIndices.Remove( myCallbackNumber );
        activeCallbacks[ myCallbackNumber ] = this;
        myVoidCallback = GetMyCallback( myCallbackNumber );
        #else
        // regular allocation
        myVoidCallback = MyCallback;
        #endif
    }

    private void ReturnCallback()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        activeCallbacks.Remove( myCallbackNumber );
        availableIndices.Add( myCallbackNumber );
        myCallbackNumber = -1;
        #endif
        // always set my callback to null
        myVoidCallback = null;
    }

    #endif // !UNITY_WEBGL

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


    #if UNITY_IOS && !UNITY_EDITOR
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
            default: return null;
        }
    }
    
    // dumb repetitive code to get around the fact that
    // we have to use static callbacks for iOS
    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback0()
    {
        if( activeCallbacks.ContainsKey( 0 ) )
        {
            activeCallbacks[0].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback1()
    {
        if( activeCallbacks.ContainsKey( 1 ) )
        {
            activeCallbacks[1].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback2()
    {
        if( activeCallbacks.ContainsKey( 2 ) )
        {
            activeCallbacks[2].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback3()
    {
        if( activeCallbacks.ContainsKey( 3 ) )
        {
            activeCallbacks[3].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback4()
    {
        if( activeCallbacks.ContainsKey( 4 ) )
        {
            activeCallbacks[4].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback5()
    {
        if( activeCallbacks.ContainsKey( 5 ) )
        {
            activeCallbacks[5].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback6()
    {
        if( activeCallbacks.ContainsKey( 6 ) )
        {
            activeCallbacks[6].numTimesCalled++;
        }
    }

    [AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
    private static void Callback7()
    {
        if( activeCallbacks.ContainsKey( 7 ) )
        {
            activeCallbacks[7].numTimesCalled++;
        }
    }
    #endif //UNITY_IOS && !UNITY_EDITOR
}
