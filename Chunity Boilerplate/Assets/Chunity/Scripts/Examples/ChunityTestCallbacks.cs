using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class ChunityTestCallbacks : MonoBehaviour
{
    ChuckSubInstance myChuck;
    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        StartCoroutine( IntTests() );
        StartCoroutine( FloatTests() );
        StartCoroutine( StringTests() ); 
        StartCoroutine( EventTests() );
        IntArrayTests();
        FloatArrayTests();
    }

    static void Success( string testName, bool b )
    {
        if( b )
        {
            Debug.Log( testName + ": success" );
        }
        else
        {
            Debug.Log( "WARNING: " + testName + " failed" );
        }
    }



    // ------------------ Int tests --------------------- //
    IEnumerator IntTests()
    {
        myChuck.RunCode(@"
            global int myInt;
        ");
        myChuck.SetInt( "myInt", (CK_INT)37 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetInt( "myInt", IntPlain );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetInt( "myInt", (CK_INT)39 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetInt( "myInt", IntName );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetInt( "myInt", (CK_INT)41 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetInt( "myInt", IntID, (CK_INT)8080 );
        yield return new WaitForSeconds( 0.1f );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntCallback))]
	#endif
    static void IntPlain( CK_INT value )
    {
        Success( "int plain value", value == (CK_INT)37 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntCallback))]
	#endif
    static void IntName( string varName, CK_INT value )
    {
        Success( "named int name", varName == "myInt" );
        Success( "named int value", value == (CK_INT)39 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntCallbackWithID))]
	#endif
    static void IntID( CK_INT id, CK_INT value )
    {
        Success( "id int id", id == (CK_INT)8080 );
        Success( "id int value", value == (CK_INT)41 );
    }


    // ------------------ Float tests --------------------- //
    IEnumerator FloatTests()
    {
        myChuck.RunCode(@"
            global float myFloat;
        ");
        myChuck.SetFloat( "myFloat", 37.5 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetFloat( "myFloat", FloatPlain );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetFloat( "myFloat", 39.8 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetFloat( "myFloat", FloatName );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetFloat( "myFloat", 41.3 );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetFloat( "myFloat", FloatID, (CK_INT)9090 );
        yield return new WaitForSeconds( 0.1f );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallback))]
	#endif
    static void FloatPlain( CK_FLOAT value )
    {
        Success( "float plain value", value == 37.5 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatCallback))]
	#endif
    static void FloatName( string varName, CK_FLOAT value )
    {
        Success( "named float name", varName == "myFloat" );
        Success( "named float value", value == 39.8 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatCallbackWithID))]
	#endif
    static void FloatID( CK_INT id, CK_FLOAT value )
    {
        Success( "id float id", id == (CK_INT)9090 );
        Success( "id float value", value == 41.3 );
    }



    // ------------------ String tests --------------------- //
    IEnumerator StringTests()
    {
        myChuck.RunCode(@"
            global string myString;
        ");
        myChuck.SetString( "myString", "hello" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetString( "myString", StringPlain );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetString( "myString", "hi" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetString( "myString", StringName );
        yield return new WaitForSeconds( 0.1f );
        myChuck.SetString( "myString", "yo" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.GetString( "myString", StringID, (CK_INT) (-1010) );
        yield return new WaitForSeconds( 0.1f );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.StringCallback))]
	#endif
    static void StringPlain( string value )
    {
        Success( "plain string value", value == "hello" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedStringCallback))]
	#endif
    static void StringName( string varName, string value )
    {
        Success( "named string name", varName == "myString" );
        Success( "named string value", value == "hi" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.StringCallbackWithID))]
	#endif
    static void StringID( CK_INT id, string value )
    {
        Success( "named string id", id == (CK_INT)(-1010) );
        Success( "named string value", value == "yo" );
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
        myChuck.ListenForChuckEventOnce( "myEvent", VoidID, (CK_INT)33 );

        // (wait between broadcasts; same-sample Event broadcast+listen has race conditions)
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        
        yield return new WaitForSeconds( 0.1f );

        // 6 here
        myChuck.StartListeningForChuckEvent( "myEvent", VoidPlain );
        myChuck.StartListeningForChuckEvent( "myEvent", VoidName );
        myChuck.StartListeningForChuckEvent( "myEvent", VoidID, (CK_INT)33 );

        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );

        yield return new WaitForSeconds( 0.1f );

        // 0 here
        myChuck.StopListeningForChuckEvent( "myEvent", VoidPlain );
        myChuck.StopListeningForChuckEvent( "myEvent", VoidName );
        myChuck.StopListeningForChuckEvent( "myEvent", VoidID, (CK_INT)33 );

        yield return new WaitForSeconds( 0.1f );
        myChuck.BroadcastEvent( "myEvent" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallback))]
	#endif
    static void VoidPlain()
    {
        Success( "plain event callback", true );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedVoidCallback))]
	#endif
    static void VoidName( string varName )
    {
        Success( "named event callback", varName == "myEvent" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.VoidCallbackWithID))]
	#endif
    static void VoidID( CK_INT id )
    {
        Success( "id event callback", id == (CK_INT)33 );
    }



    // ------------------ Int array tests --------------------- //
    void IntArrayTests()
    {
        myChuck.RunCode(@"
            global int myIntArray[3];
        ");
        // entire array
        myChuck.SetIntArray( "myIntArray", new CK_INT[] { (CK_INT)0, (CK_INT)1, (CK_INT)37 } );
        myChuck.GetIntArray( "myIntArray", IntArrayPlain );
        myChuck.GetIntArray( "myIntArray", IntArrayName );
        myChuck.GetIntArray( "myIntArray", IntArrayID, (CK_INT)3030 );

        // one element at a time
        myChuck.SetIntArrayValue( "myIntArray", 1, (CK_INT)37 );
        myChuck.GetIntArrayValue( "myIntArray", 1, IntPlain );
        myChuck.SetIntArrayValue( "myIntArray", 2, (CK_INT)39 );
        myChuck.GetIntArrayValue( "myIntArray", 2, IntArrayValueName );
        myChuck.SetIntArrayValue( "myIntArray", 0, (CK_INT)41 );
        myChuck.GetIntArrayValue( "myIntArray", 0, IntID, (CK_INT)8080 );

        // using array as a dictionary instead of an array
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key1", (CK_INT)37 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key1", IntPlain );
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key2", (CK_INT)39 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key2", IntArrayValueName );
        myChuck.SetAssociativeIntArrayValue( "myIntArray", "key3", (CK_INT)41 );
        myChuck.GetAssociativeIntArrayValue( "myIntArray", "key3", IntID, (CK_INT)8080 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallback))]
	#endif
    static void IntArrayPlain( CK_INT[] value, CK_UINT length )
    {
        Success( "int array plain values", value[0] == (CK_INT)0 && value[1] == (CK_INT)1 && value[2] == (CK_INT)37 );
        Success( "int array plain length", length == (CK_UINT)3 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntArrayCallback))]
	#endif
    static void IntArrayName( string name, CK_INT[] value, CK_UINT length )
    {
        Success( "named int array values", value[0] == (CK_INT)0 && value[1] == (CK_INT)1 && value[2] == (CK_INT)37 );
        Success( "named int array length", length == (CK_UINT)3 );
        Success( "named int array name", name == "myIntArray" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.IntArrayCallbackWithID))]
	#endif
    static void IntArrayID( CK_INT id, CK_INT[] value, CK_UINT length )
    {
        Success( "id int array values", value[0] == (CK_INT)0 && value[1] == (CK_INT)1 && value[2] == (CK_INT)37 );
        Success( "id int array length", length == (CK_UINT)3 );
        Success( "id int array id", id == (CK_INT)3030 );
    }


    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedIntCallback))]
	#endif
    static void IntArrayValueName( string varName, CK_INT value )
    {
        Success( "named int array name", varName == "myIntArray" );
        Success( "named int array value", value == (CK_INT)39 );
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
        myChuck.GetFloatArray( "myFloatArray", FloatArrayID, (CK_INT)4040 );

        // one element at a time
        myChuck.SetFloatArrayValue( "myFloatArray", 1, 37.5 );
        myChuck.GetFloatArrayValue( "myFloatArray", 1, FloatPlain );
        myChuck.SetFloatArrayValue( "myFloatArray", 2, 39.8 );
        myChuck.GetFloatArrayValue( "myFloatArray", 2, FloatArrayValueName );
        myChuck.SetFloatArrayValue( "myFloatArray", 0, 41.3 );
        myChuck.GetFloatArrayValue( "myFloatArray", 0, FloatID, (CK_INT)9090 );

        // using array as a dictionary instead of an array
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key1", 37.5 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key1", FloatPlain );
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key2", 39.8 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key2", FloatArrayValueName );
        myChuck.SetAssociativeFloatArrayValue( "myFloatArray", "key3", 41.3 );
        myChuck.GetAssociativeFloatArrayValue( "myFloatArray", "key3", FloatID, (CK_INT)9090 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallback))]
	#endif
    static void FloatArrayPlain( CK_FLOAT[] value, CK_UINT length )
    {
        Success( "float array values", value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( "float array length", length == (CK_UINT)3 );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatArrayCallback))]
	#endif
    static void FloatArrayName( string name, CK_FLOAT[] value, CK_UINT length )
    {
        Success( "named float array values", value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( "named float array length", length == (CK_UINT)3 );
        Success( "named float array name", name == "myFloatArray" );
    }

    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.FloatArrayCallbackWithID))]
	#endif
    static void FloatArrayID( CK_INT id, CK_FLOAT[] value, CK_UINT length )
    {
        Success( "id float array values", value[0] == 0.5 && value[1] == 0.7 && value[2] == 37.9 );
        Success( "id float array length", length == (CK_UINT)3 );
        Success( "id float array id", id == (CK_INT)4040 );
    }


    #if ( UNITY_IOS || UNITY_ANDROID ) && !UNITY_EDITOR
	[AOT.MonoPInvokeCallback(typeof(Chuck.NamedFloatCallback))]
	#endif
    static void FloatArrayValueName( string varName, CK_FLOAT value )
    {
        Success( "named float array name", varName == "myFloatArray" );
        Success( "named float array value", value == 39.8 );
    }

}
