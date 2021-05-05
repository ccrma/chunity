using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WEBGL
using CK_INT = System.Int32;
using CK_UINT = System.UInt32;
#else
using CK_INT = System.Int64;
using CK_UINT = System.UInt64;
#endif
using CK_FLOAT = System.Double;

public class ChunityTestCallbacks : MonoBehaviour
{
    ChuckSubInstance myChuck;
    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        IntTests();
        FloatTests();
        StringTests();
        StartCoroutine( EventTests() );
        IntArrayTests();
        FloatArrayTests();
    }

    static void Success( bool b )
    {
        Debug.Log( b ? "success" : "failure" );
    }



    // ------------------ Int tests --------------------- //
    void IntTests()
    {
        myChuck.RunCode(@"
            global int myInt;
        ");
        myChuck.SetInt( "myInt", 37 );
        myChuck.GetInt( "myInt", IntPlain );
        myChuck.SetInt( "myInt", 39 );
        myChuck.GetInt( "myInt", IntName );
        myChuck.SetInt( "myInt", 41 );
        myChuck.GetInt( "myInt", IntID, 8080 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
	#endif
    static void IntPlain( CK_INT value )
    {
        Success( value == 37 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntCallback))]
	#endif
    static void IntName( string varName, CK_INT value )
    {
        Success( varName == "myInt" );
        Success( value == 39 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntCallbackWithID))]
	#endif
    static void IntID( CK_INT id, CK_INT value )
    {
        Success( id == 8080 );
        Success( value == 41 );
    }


    // ------------------ Float tests --------------------- //
    void FloatTests()
    {
        myChuck.RunCode(@"
            global float myFloat;
        ");
        myChuck.SetFloat( "myFloat", 37.5 );
        myChuck.GetFloat( "myFloat", FloatPlain );
        myChuck.SetFloat( "myFloat", 39.8 );
        myChuck.GetFloat( "myFloat", FloatName );
        myChuck.SetFloat( "myFloat", 41.3 );
        myChuck.GetFloat( "myFloat", FloatID, 9090 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
	#endif
    static void FloatPlain( CK_FLOAT value )
    {
        Success( value == 37.5 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatCallback))]
	#endif
    static void FloatName( string varName, CK_FLOAT value )
    {
        Success( varName == "myFloat" );
        Success( value == 39.8 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallbackWithID))]
	#endif
    static void FloatID( CK_INT id, CK_FLOAT value )
    {
        Success( id == 9090 );
        Success( value == 41.3 );
    }



    // ------------------ String tests --------------------- //
    void StringTests()
    {
        myChuck.RunCode(@"
            global string myString;
        ");
        myChuck.SetString( "myString", "hello" );
        myChuck.GetString( "myString", StringPlain );
        myChuck.SetString( "myString", "hi" );
        myChuck.GetString( "myString", StringName );
        myChuck.SetString( "myString", "yo" );
        myChuck.GetString( "myString", StringID, -1010 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
	#endif
    static void StringPlain( string value )
    {
        Success( value == "hello" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedStringCallback))]
	#endif
    static void StringName( string varName, string value )
    {
        Success( varName == "myString" );
        Success( value == "hi" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.StringCallbackWithID))]
	#endif
    static void StringID( CK_INT id, string value )
    {
        Success( id == -1010 );
        Success( value == "yo" );
    }



    // ------------------ Event tests --------------------- //
    IEnumerator EventTests()
    {
        myChuck.RunCode(@"
            global Event myEvent;
        ");
        // expected number of successes: 9 (3 per callback)
        // 3 here
        myChuck.ListenForChuckEventOnce( "myEvent", VoidPlain );
        myChuck.ListenForChuckEventOnce( "myEvent", VoidName );
        myChuck.ListenForChuckEventOnce( "myEvent", VoidID, 33 );

        // (wait between broadcasts; same-sample Event broadcast+listen has race conditions)
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        
        yield return new WaitForSeconds( 0.1f );

        // 6 here
        myChuck.StartListeningForChuckEvent( "myEvent", VoidPlain );
        myChuck.StartListeningForChuckEvent( "myEvent", VoidName );
        myChuck.StartListeningForChuckEvent( "myEvent", VoidID, 33 );

        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );

        yield return new WaitForSeconds( 0.1f );

        // 0 here
        myChuck.StopListeningForChuckEvent( "myEvent", VoidPlain );
        myChuck.StopListeningForChuckEvent( "myEvent", VoidName );
        myChuck.StopListeningForChuckEvent( "myEvent", VoidID, 33 );

        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
	#endif
    static void VoidPlain()
    {
        Success( true );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedVoidCallback))]
	#endif
    static void VoidName( string varName )
    {
        Success( varName == "myEvent" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallbackWithID))]
	#endif
    static void VoidID( CK_INT id )
    {
        Success( id == 33 );
    }



    // ------------------ Int array tests --------------------- //
    void IntArrayTests()
    {
        myChuck.RunCode(@"
            global int myIntArray[3];
        ");
        // entire array
        myChuck.SetIntArray( "myIntArray", new CK_INT[] {0, 1, 37} );
        myChuck.GetIntArray( "myIntArray", IntArrayPlain );
        myChuck.GetIntArray( "myIntArray", IntArrayName );
        myChuck.GetIntArray( "myIntArray", IntArrayID, 3030 );

        // one element at a time
        myChuck.SetIntArrayValue( "myIntArray", 1, 37 );
        myChuck.GetIntArrayValue( "myIntArray", 1, IntPlain );
        myChuck.SetIntArrayValue( "myIntArray", 2, 39 );
        myChuck.GetIntArrayValue( "myIntArray", 2, IntArrayValueName );
        myChuck.SetIntArrayValue( "myIntArray", 0, 41 );
        myChuck.GetIntArrayValue( "myIntArray", 0, IntID, 8080 );

        // using array as a dictionary instead of an array
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key1", 37 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key1", IntPlain );
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key2", 39 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key2", IntArrayValueName );
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key3", 41 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key3", IntID, 8080 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallback))]
	#endif
    static void IntArrayPlain( CK_INT[] value, CK_UINT length )
    {
        Success( value[0] == 0 && value[1] == 1 && value[2] == 37 );
        Success( length == 3 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntArrayCallback))]
	#endif
    static void IntArrayName( string name, CK_INT[] value, CK_UINT length )
    {
        Success( value[0] == 0 && value[1] == 1 && value[2] == 37 );
        Success( length == 3 );
        Success( name == "myIntArray" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallbackWithID))]
	#endif
    static void IntArrayID( CK_INT id, CK_INT[] value, CK_UINT length )
    {
        Success( value[0] == 0 && value[1] == 1 && value[2] == 37 );
        Success( length == 3 );
        Success( id == 3030 );
    }


    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntCallback))]
	#endif
    static void IntArrayValueName( string varName, CK_INT value )
    {
        Success( varName == "myIntArray" );
        Success( value == 39 );
    }



    // ------------------ Float array tests --------------------- //
    void FloatArrayTests()
    {
        myChuck.RunCode(@"
            global float myFloatArray[3];
        ");
        // entire array
        myChuck.SetFloatArray( "myFloatArray", new CK_FLOAT[] {0.5, 0.7, 37.9} );
        myChuck.GetFloatArray( "myFloatArray", FloatArrayPlain );
        myChuck.GetFloatArray( "myFloatArray", FloatArrayName );
        myChuck.GetFloatArray( "myFloatArray", FloatArrayID, 4040 );

        // one element at a time
        myChuck.SetFloatArrayValue( "myFloatArray", 1, 37.5 );
        myChuck.GetFloatArrayValue( "myFloatArray", 1, FloatPlain );
        myChuck.SetFloatArrayValue( "myFloatArray", 2, 39.8 );
        myChuck.GetFloatArrayValue( "myFloatArray", 2, FloatArrayValueName );
        myChuck.SetFloatArrayValue( "myFloatArray", 0, 41.3 );
        myChuck.GetFloatArrayValue( "myFloatArray", 0, FloatID, 9090 );

        // using array as a dictionary instead of an array
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key1", 37.5 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key1", FloatPlain );
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key2", 39.8 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key2", FloatArrayValueName );
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key3", 41.3 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key3", FloatID, 9090 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallback))]
	#endif
    static void FloatArrayPlain( CK_FLOAT[] value, CK_UINT length )
    {
        Success( value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( length == 3 );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatArrayCallback))]
	#endif
    static void FloatArrayName( string name, CK_FLOAT[] value, CK_UINT length )
    {
        Success( value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( length == 3 );
        Success( name == "myFloatArray" );
    }

    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallbackWithID))]
	#endif
    static void FloatArrayID( CK_INT id, CK_FLOAT[] value, CK_UINT length )
    {
        Success( value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( length == 3 );
        Success( id == 4040 );
    }


    #if UNITY_IOS && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatCallback))]
	#endif
    static void FloatArrayValueName( string varName, CK_FLOAT value )
    {
        Success( varName == "myFloatArray" );
        Success( value == 39.8 );
    }

}
