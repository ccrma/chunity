using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Audio;
using System.Runtime.InteropServices;

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

public class Chuck
{

    public static Chuck Manager
    {
        get
        {
            if( __sharedInstance == null )
            {
                __sharedInstance = new Chuck();
                #if UNITY_WEBGL
                initChuckScript();
                #endif
            }

            return __sharedInstance;
        }
    }

    public void Initialize( AudioMixer mixer, string name )
    {
        // only initialize if haven't initialized yet
        if( !ids.ContainsKey( name ) )
        {
            System.UInt32 id = _nextValidID;
            System.UInt32 sampleRate = Convert.ToUInt32( AudioSettings.outputSampleRate );

            // create a chuck in c++, then connect it to the unity callback
            if( ( !initChuckInstance( id, sampleRate ) ) ||
                ( !mixer.SetFloat( name, id * 1.0f ) ) )
            {
                // note: when things go poorly, mixer.SetFloat 
                // never *actually* returns false and so this error message will not be seen.
                // instead, will see "Assertion failed on expression: 'res == FMOD_OK'
                Debug.Log( "ChucK ID C++ storage failed for " + name );
                return;
            }
            else
            {
                setChoutCallback( id, chout_delegate );
                setCherrCallback( id, cherr_delegate );
            }

            // store association in c-sharp
            ids.Add( name, _nextValidID );
            Debug.Log( "ChucK instance " + name + " has been initialized!" );
            _nextValidID++;
        }
        else
        {
            Debug.Log( "ChucK instance " + name + " has already been initialized." );
        }
    }

    public System.UInt32 InitializeFilter()
    {
        System.UInt32 id = _nextValidID;
        System.UInt32 sampleRate = Convert.ToUInt32( AudioSettings.outputSampleRate );

        if( !initChuckInstance( id, sampleRate ) )
        {
            Debug.Log( "Chuck C++ initialization failed for filter" );
            return System.UInt32.MaxValue;
        }
        else
        {
            setChoutCallback( id, chout_delegate );
            setCherrCallback( id, cherr_delegate );
        }

        _nextValidID++;

        return id;
    }

    public bool CleanupFilter( System.UInt32 id )
    {
        return cleanupChuckInstance( id );
    }

    public bool ManualAudioCallback( System.UInt32 chuckID, float[] inBuffer, float[] outBuffer, System.UInt32 channels )
    {
        #if UNITY_WEBGL
        return false;
        #else
        System.UInt32 numFrames = Convert.ToUInt32( inBuffer.Length / channels );
        return chuckManualAudioCallback( chuckID, inBuffer, outBuffer, numFrames, channels, channels );
        #endif
    }

    public bool RunCode( string name, string code )
    {
        if( ids.ContainsKey( name ) )
        {
            return RunCode( ids[name], code );
        }
        else
        {
            Debug.Log( name + " has not been initialized as a ChucK instance" );
        }
        return false;
    }

    public bool RunCode( System.UInt32 chuckId, string code )
    {
        return runChuckCode( chuckId, code );
    }

    public bool RunCodeWithReplacementDac( System.UInt32 chuckId, string code, string replacementDac )
    {
        return runChuckCodeWithReplacementDac( chuckId, code, replacementDac );
    }

    public bool RunFile( string name, string filename, bool fromStreamingAssets = true )
    {
        if( ids.ContainsKey( name ) )
        {
            return RunFile( ids[name], filename, fromStreamingAssets );
        }
        else
        {
            Debug.Log( name + " has not been initialized as a ChucK instance" );
        }
        return false;
    }

    public bool RunFile( System.UInt32 chuckId, string filename, bool fromStreamingAssets = true )
    {
        if( fromStreamingAssets )
        {
            // Path.Combine() does the wrong thing on Windows for me --
            // Application.streamingAssetsPath has forward slashes, but
            // Path.Combine() uses a back-slash so it thinks the first
            // letter of filename is an unknown escape sequence.
            // --> just manually construct it instead, I guess. 
            // filename = Path.Combine( Application.streamingAssetsPath, filename );
            filename = Application.streamingAssetsPath + '/' + filename;
        }
        return runChuckFile( chuckId, filename );
    }

    public bool RunFileWithReplacementDac( System.UInt32 chuckId, string filename, string replacementDac, bool fromStreamingAssets = true )
    {
        if( fromStreamingAssets )
        {
            // Path.Combine() does the wrong thing on Windows for me --
            // Application.streamingAssetsPath has forward slashes, but
            // Path.Combine() uses a back-slash so it thinks the first
            // letter of filename is an unknown escape sequence.
            // --> just manually construct it instead, I guess. 
            // filename = Path.Combine( Application.streamingAssetsPath, filename );
            filename = Application.streamingAssetsPath + '/' + filename;
        }
        return runChuckFileWithReplacementDac( chuckId, filename, replacementDac );
    }

    public bool RunFile( string name, string filename, string args, bool fromStreamingAssets = true )
    {
        if( ids.ContainsKey( name ) )
        {
            return RunFile( ids[name], filename, args, fromStreamingAssets );
        }
        else
        {
            Debug.Log( name + " has not been initialized as a ChucK instance" );
        }
        return false;
    }

    public bool RunFile( System.UInt32 chuckId, string filename, string args, bool fromStreamingAssets = true )
    {
        if( fromStreamingAssets )
        {
            // Path.Combine() does the wrong thing on Windows for me --
            // Application.streamingAssetsPath has forward slashes, but
            // Path.Combine() uses a back-slash so it thinks the first
            // letter of filename is an unknown escape sequence.
            // --> just manually construct it instead, I guess. 
            // filename = Path.Combine( Application.streamingAssetsPath, filename );
            filename = Application.streamingAssetsPath + '/' + filename;
        }
        return runChuckFileWithArgs( chuckId, filename, args );
    }

    public bool RunFileWithReplacementDac( System.UInt32 chuckId, string filename, string args, string replacementDac, bool fromStreamingAssets = true )
    {
        if( fromStreamingAssets )
        {
            // Path.Combine() does the wrong thing on Windows for me --
            // Application.streamingAssetsPath has forward slashes, but
            // Path.Combine() uses a back-slash so it thinks the first
            // letter of filename is an unknown escape sequence.
            // --> just manually construct it instead, I guess. 
            // filename = Path.Combine( Application.streamingAssetsPath, filename );
            filename = Application.streamingAssetsPath + '/' + filename;
        }
        return runChuckFileWithArgsWithReplacementDac( chuckId, filename, args, replacementDac );
    }

    public bool SetInt( string chuckName, string variableName, CK_INT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetInt( ids[chuckName], variableName, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetInt( System.UInt32 chuckId, string variableName, CK_INT value )
    {
        return setChuckInt( chuckId, variableName, value );
    }

    public static Chuck.IntCallback CreateGetIntCallback( Action<CK_INT> callbackFunction )
    {
        return new IntCallback( callbackFunction );
    }

    public static Chuck.NamedIntCallback CreateNamedGetIntCallback( Action<string, CK_INT> callbackFunction )
    {
        return new NamedIntCallback( callbackFunction );
    }

    public static Chuck.IntCallbackWithID CreateIDGetIntCallback( Action<CK_INT, CK_INT> callbackFunction )
    {
        return new IntCallbackWithID( callbackFunction );
    }

    public bool GetInt( string chuckName, string variableName, Chuck.IntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetInt( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetInt( string chuckName, string variableName, Chuck.NamedIntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetInt( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetInt( string chuckName, string variableName, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetInt( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetInt( System.UInt32 chuckId, string variableName, Chuck.IntCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        intCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckInt( chuckId, variableName, intCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetInt( System.UInt32 chuckId, string variableName, Chuck.NamedIntCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        namedIntCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getNamedChuckInt( chuckId, variableName, namedIntCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetInt( System.UInt32 chuckId, string variableName, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        idIntCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckIntWithID( chuckId, callbackID, variableName, idIntCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool SetFloat( string chuckName, string variableName, CK_FLOAT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetFloat( ids[chuckName], variableName, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetFloat( System.UInt32 chuckId, string variableName, CK_FLOAT value )
    {
        return setChuckFloat( chuckId, variableName, value );
    }

    public static Chuck.FloatCallback CreateGetFloatCallback( Action<CK_FLOAT> callbackFunction )
    {
        return new FloatCallback( callbackFunction );
    }

    public static Chuck.NamedFloatCallback CreateNamedGetFloatCallback( Action<string, CK_FLOAT> callbackFunction )
    {
        return new NamedFloatCallback( callbackFunction );
    }

    public static Chuck.FloatCallbackWithID CreateIDGetFloatCallback( Action<CK_INT, CK_FLOAT> callbackFunction )
    {
        return new FloatCallbackWithID( callbackFunction );
    }

    public bool GetFloat( string chuckName, string variableName, Chuck.FloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloat( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloat( string chuckName, string variableName, Chuck.NamedFloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloat( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloat( string chuckName, string variableName, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloat( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloat( System.UInt32 chuckId, string variableName, Chuck.FloatCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        floatCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckFloat( chuckId, variableName, floatCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetFloat( System.UInt32 chuckId, string variableName, Chuck.NamedFloatCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        namedFloatCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getNamedChuckFloat( chuckId, variableName, namedFloatCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetFloat( System.UInt32 chuckId, string variableName, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        idFloatCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckFloatWithID( chuckId, callbackID, variableName, idFloatCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool SetString( string chuckName, string variableName, System.String value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetString( ids[chuckName], variableName, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetString( System.UInt32 chuckId, string variableName, System.String value )
    {
        return setChuckString( chuckId, variableName, value );
    }

    public static Chuck.StringCallback CreateGetStringCallback( Action<System.String> callbackFunction )
    {
        return new StringCallback( callbackFunction );
    }

    public static Chuck.NamedStringCallback CreateNamedGetStringCallback( Action<string, System.String> callbackFunction )
    {
        return new NamedStringCallback( callbackFunction );
    }

    public static Chuck.StringCallbackWithID CreateIDGetStringCallback( Action<CK_INT, System.String> callbackFunction )
    {
        return new StringCallbackWithID( callbackFunction );
    }

    public bool GetString( string chuckName, string variableName, Chuck.StringCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetString( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetString( string chuckName, string variableName, Chuck.NamedStringCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetString( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetString( string chuckName, string variableName, Chuck.StringCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetString( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetString( System.UInt32 chuckId, string variableName, Chuck.StringCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        stringCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckString( chuckId, variableName, stringCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetString( System.UInt32 chuckId, string variableName, Chuck.NamedStringCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        namedStringCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getNamedChuckString( chuckId, variableName, namedStringCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public bool GetString( System.UInt32 chuckId, string variableName, Chuck.StringCallbackWithID callback, CK_INT callbackID )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        idStringCallbacks[internalKey] = callback;
        // register the callback with ChucK
        if( !getChuckStringWithID( chuckId, callbackID, variableName, idStringCallbacks[internalKey] ) )
        {
            return false;
        }
        return true;
    }

    public static Chuck.VoidCallback CreateVoidCallback( Action callbackFunction )
    {
        return new VoidCallback( callbackFunction );
    }

    public static Chuck.NamedVoidCallback CreateNamedVoidCallback( Action<string> callbackFunction )
    {
        return new NamedVoidCallback( callbackFunction );
    }

    public static Chuck.VoidCallbackWithID CreateIDVoidCallback( Action<CK_INT> callbackFunction )
    {
        return new VoidCallbackWithID( callbackFunction );
    }

    public bool SignalEvent( string chuckName, string variableName )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SignalEvent( ids[chuckName], variableName );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SignalEvent( System.UInt32 chuckId, string variableName )
    {
        return signalChuckEvent( chuckId, variableName );
    }

    public bool BroadcastEvent( string chuckName, string variableName )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return BroadcastEvent( ids[chuckName], variableName );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool BroadcastEvent( System.UInt32 chuckId, string variableName )
    {
        return broadcastChuckEvent( chuckId, variableName );
    }

    public bool ListenForChuckEventOnce( string chuckName, string variableName, Chuck.VoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return ListenForChuckEventOnce( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool ListenForChuckEventOnce( string chuckName, string variableName, Chuck.NamedVoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return ListenForChuckEventOnce( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool ListenForChuckEventOnce( string chuckName, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return ListenForChuckEventOnce( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool ListenForChuckEventOnce( System.UInt32 chuckId, string variableName, Chuck.VoidCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        voidCallbacks[internalKey] = callback;
        return listenForChuckEventOnce( chuckId, variableName, callback );
    }

    public bool ListenForChuckEventOnce( System.UInt32 chuckId, string variableName, Chuck.NamedVoidCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        namedVoidCallbacks[internalKey] = callback;
        return listenForNamedChuckEventOnce( chuckId, variableName, callback );
    }

    public bool ListenForChuckEventOnce( System.UInt32 chuckId, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        idVoidCallbacks[internalKey] = callback;
        return listenForChuckEventOnceWithID( chuckId, callbackID, variableName, callback );
    }

    public bool StartListeningForChuckEvent( string chuckName, string variableName, Chuck.VoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StartListeningForChuckEvent( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StartListeningForChuckEvent( string chuckName, string variableName, Chuck.NamedVoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StartListeningForChuckEvent( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StartListeningForChuckEvent( string chuckName, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StartListeningForChuckEvent( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StartListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.VoidCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        voidCallbacks[internalKey] = callback;
        return startListeningForChuckEvent( chuckId, variableName, callback );
    }

    public bool StartListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.NamedVoidCallback callback )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        namedVoidCallbacks[internalKey] = callback;
        return startListeningForNamedChuckEvent( chuckId, variableName, callback );
    }

    public bool StartListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        // save a copy of the delegate so it doesn't get garbage collected!
        string internalKey = chuckId.ToString() + "$" + variableName;
        idVoidCallbacks[internalKey] = callback;
        return startListeningForChuckEventWithID( chuckId, callbackID, variableName, callback );
    }


    public bool StopListeningForChuckEvent( string chuckName, string variableName, Chuck.VoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StopListeningForChuckEvent( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StopListeningForChuckEvent( string chuckName, string variableName, Chuck.NamedVoidCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StopListeningForChuckEvent( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StopListeningForChuckEvent( string chuckName, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return StopListeningForChuckEvent( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool StopListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.VoidCallback callback )
    {
        // Don't need to save the callback - it will not be called; only the value of its pointer will be checked
        return stopListeningForChuckEvent( chuckId, variableName, callback );
    }

    public bool StopListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.NamedVoidCallback callback )
    {
        // Don't need to save the callback - it will not be called; only the value of its pointer will be checked
        return stopListeningForNamedChuckEvent( chuckId, variableName, callback );
    }

    public bool StopListeningForChuckEvent( System.UInt32 chuckId, string variableName, Chuck.VoidCallbackWithID callback, CK_INT callbackID )
    {
        // Don't need to save the callback - it will not be called; only the value of its pointer will be checked
        return stopListeningForChuckEventWithID( chuckId, callbackID, variableName, callback );
    }

    public bool GetUGenSamples( System.UInt32 chuckID, System.String name,
        float[] buffer, System.Int32 numSamples )
    {
        #if UNITY_WEBGL
        return false;
        #else
        return getGlobalUGenSamples( chuckID, name, buffer, numSamples );
        #endif
    }

    public static Chuck.IntArrayCallback CreateGetIntArrayCallback( Action<CK_INT[], CK_UINT> callbackFunction )
    {
        return new IntArrayCallback( callbackFunction );
    }

    public static Chuck.NamedIntArrayCallback CreateNamedGetIntArrayCallback( Action<string, CK_INT[], CK_UINT> callbackFunction )
    {
        return new NamedIntArrayCallback( callbackFunction );
    }

    public static Chuck.IntArrayCallbackWithID CreateIDGetIntArrayCallback( Action<CK_INT, CK_INT[], CK_UINT> callbackFunction )
    {
        return new IntArrayCallbackWithID( callbackFunction );
    }

    public bool SetIntArray( string chuckName, string variableName, CK_INT[] values )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetIntArray( ids[chuckName], variableName, values );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetIntArray( System.UInt32 chuckId, string variableName, CK_INT[] values )
    {
        return setGlobalIntArray( chuckId, variableName, values, (uint) values.Length );
    }

    public bool GetIntArray( string chuckName, string variableName, Chuck.IntArrayCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArray( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArray( string chuckName, string variableName, Chuck.NamedIntArrayCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArray( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArray( string chuckName, string variableName, Chuck.IntArrayCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArray( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArray( System.UInt32 chuckId, string variableName, Chuck.IntArrayCallback callback )
    {
        return getGlobalIntArray( chuckId, variableName, callback );
    }

    public bool GetIntArray( System.UInt32 chuckId, string variableName, Chuck.NamedIntArrayCallback callback )
    {
        return getNamedGlobalIntArray( chuckId, variableName, callback );
    }

    public bool GetIntArray( System.UInt32 chuckId, string variableName, Chuck.IntArrayCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalIntArrayWithID( chuckId, callbackID, variableName, callback );
    }

    public bool SetIntArrayValue( string chuckName, string variableName, uint index, CK_INT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetIntArrayValue( ids[chuckName], variableName, index, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetIntArrayValue( System.UInt32 chuckId, string variableName, uint index, CK_INT value )
    {
        return setGlobalIntArrayValue( chuckId, variableName, index, value );
    }

    public bool GetIntArrayValue( string chuckName, string variableName, uint index, Chuck.IntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArrayValue( ids[chuckName], variableName, index, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArrayValue( string chuckName, string variableName, uint index, Chuck.NamedIntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArrayValue( ids[chuckName], variableName, index, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArrayValue( string chuckName, string variableName, uint index, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetIntArrayValue( ids[chuckName], variableName, index, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetIntArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.IntCallback callback )
    {
        return getGlobalIntArrayValue( chuckId, variableName, index, callback );
    }

    public bool GetIntArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.NamedIntCallback callback )
    {
        return getNamedGlobalIntArrayValue( chuckId, variableName, index, callback );
    }

    public bool GetIntArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalIntArrayValueWithID( chuckId, callbackID, variableName, index, callback );
    }

    public bool SetAssociativeIntArrayValue( string chuckName, string variableName, string key, CK_INT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetAssociativeIntArrayValue( ids[chuckName], variableName, key, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetAssociativeIntArrayValue( System.UInt32 chuckId, string variableName, string key, CK_INT value )
    {
        return setGlobalAssociativeIntArrayValue( chuckId, variableName, key, value );
    }

    public bool GetAssociativeIntArrayValue( string chuckName, string variableName, string key, Chuck.IntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeIntArrayValue( ids[chuckName], variableName, key, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeIntArrayValue( string chuckName, string variableName, string key, Chuck.NamedIntCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeIntArrayValue( ids[chuckName], variableName, key, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeIntArrayValue( string chuckName, string variableName, string key, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeIntArrayValue( ids[chuckName], variableName, key, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeIntArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.IntCallback callback )
    {
        return getGlobalAssociativeIntArrayValue( chuckId, variableName, key, callback );
    }

    public bool GetAssociativeIntArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.NamedIntCallback callback )
    {
        return getNamedGlobalAssociativeIntArrayValue( chuckId, variableName, key, callback );
    }

    public bool GetAssociativeIntArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.IntCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalAssociativeIntArrayValueWithID( chuckId, callbackID, variableName, key, callback );
    }

    public static Chuck.FloatArrayCallback CreateGetFloatArrayCallback( Action<CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return new FloatArrayCallback( callbackFunction );
    }

    public static Chuck.NamedFloatArrayCallback CreateNamedGetFloatArrayCallback( Action<string, CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return new NamedFloatArrayCallback( callbackFunction );
    }

    public static Chuck.FloatArrayCallbackWithID CreateIDGetFloatArrayCallback( Action<CK_INT, CK_FLOAT[], CK_UINT> callbackFunction )
    {
        return new FloatArrayCallbackWithID( callbackFunction );
    }

    public bool SetFloatArray( string chuckName, string variableName, CK_FLOAT[] values )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetFloatArray( ids[chuckName], variableName, values );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetFloatArray( System.UInt32 chuckId, string variableName, CK_FLOAT[] values )
    {
        return setGlobalFloatArray( chuckId, variableName, values, (uint) values.Length );
    }

    public bool GetFloatArray( string chuckName, string variableName, Chuck.FloatArrayCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArray( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArray( string chuckName, string variableName, Chuck.NamedFloatArrayCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArray( ids[chuckName], variableName, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArray( string chuckName, string variableName, Chuck.FloatArrayCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArray( ids[chuckName], variableName, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArray( System.UInt32 chuckId, string variableName, Chuck.FloatArrayCallback callback )
    {
        return getGlobalFloatArray( chuckId, variableName, callback );
    }

    public bool GetFloatArray( System.UInt32 chuckId, string variableName, Chuck.NamedFloatArrayCallback callback )
    {
        return getNamedGlobalFloatArray( chuckId, variableName, callback );
    }

    public bool GetFloatArray( System.UInt32 chuckId, string variableName, Chuck.FloatArrayCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalFloatArrayWithID( chuckId, callbackID, variableName, callback );
    }

    public bool SetFloatArrayValue( string chuckName, string variableName, uint index, CK_FLOAT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetFloatArrayValue( ids[chuckName], variableName, index, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetFloatArrayValue( System.UInt32 chuckId, string variableName, uint index, CK_FLOAT value )
    {
        return setGlobalFloatArrayValue( chuckId, variableName, index, value );
    }

    public bool GetFloatArrayValue( string chuckName, string variableName, uint index, Chuck.FloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArrayValue( ids[chuckName], variableName, index, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArrayValue( string chuckName, string variableName, uint index, Chuck.NamedFloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArrayValue( ids[chuckName], variableName, index, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArrayValue( string chuckName, string variableName, uint index, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetFloatArrayValue( ids[chuckName], variableName, index, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetFloatArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.FloatCallback callback )
    {
        return getGlobalFloatArrayValue( chuckId, variableName, index, callback );
    }

    public bool GetFloatArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.NamedFloatCallback callback )
    {
        return getNamedGlobalFloatArrayValue( chuckId, variableName, index, callback );
    }

    public bool GetFloatArrayValue( System.UInt32 chuckId, string variableName, uint index, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalFloatArrayValueWithID( chuckId, callbackID, variableName, index, callback );
    }

    public bool SetAssociativeFloatArrayValue( string chuckName, string variableName, string key, CK_FLOAT value )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return SetAssociativeFloatArrayValue( ids[chuckName], variableName, key, value );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool SetAssociativeFloatArrayValue( System.UInt32 chuckId, string variableName, string key, CK_FLOAT value )
    {
        return setGlobalAssociativeFloatArrayValue( chuckId, variableName, key, value );
    }

    public bool GetAssociativeFloatArrayValue( string chuckName, string variableName, string key, Chuck.FloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeFloatArrayValue( ids[chuckName], variableName, key, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeFloatArrayValue( string chuckName, string variableName, string key, Chuck.NamedFloatCallback callback )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeFloatArrayValue( ids[chuckName], variableName, key, callback );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeFloatArrayValue( string chuckName, string variableName, string key, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        if( ids.ContainsKey( chuckName ) )
        {
            return GetAssociativeFloatArrayValue( ids[chuckName], variableName, key, callback, callbackID );
        }
        else
        {
            Debug.Log( chuckName + " has not been initialized as a ChucK instance" );
            return false;
        }
    }

    public bool GetAssociativeFloatArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.FloatCallback callback )
    {
        return getGlobalAssociativeFloatArrayValue( chuckId, variableName, key, callback );
    }

    public bool GetAssociativeFloatArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.NamedFloatCallback callback )
    {
        return getNamedGlobalAssociativeFloatArrayValue( chuckId, variableName, key, callback );
    }

    public bool GetAssociativeFloatArrayValue( System.UInt32 chuckId, string variableName, string key, Chuck.FloatCallbackWithID callback, CK_INT callbackID )
    {
        return getGlobalAssociativeFloatArrayValueWithID( chuckId, callbackID, variableName, key, callback );
    }

    public enum LogLevel
    {
        None = 0,
        Core,
        System,
        Severe,
        Warning,
        Info,
        Config,
        Fine,
        Finer,
        Finest,
        Crazy
    }

    public static void SetLogLevel( Chuck.LogLevel level )
    {
        setLogLevel( (uint) level );
    }


    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void MyLogCallback( System.String str );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void IntCallback( CK_INT i );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedIntCallback( System.String name, CK_INT i );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void IntCallbackWithID( CK_INT callbackID, CK_INT i );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void VoidCallback();

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedVoidCallback( System.String name );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void VoidCallbackWithID( CK_INT callbackID );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void FloatCallback( CK_FLOAT f );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedFloatCallback( System.String name, CK_FLOAT f );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void FloatCallbackWithID( CK_INT callbackID, CK_FLOAT f );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void StringCallback( System.String str );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedStringCallback( System.String name, System.String str );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void StringCallbackWithID( CK_INT callbackID, System.String str );


    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void IntArrayCallback(
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U8, SizeParamIndex = 1)]
        CK_INT[] values,
        CK_UINT numValues
    );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedIntArrayCallback(
        System.String name,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U8, SizeParamIndex = 2)]
        CK_INT[] values,
        CK_UINT numValues
    );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void IntArrayCallbackWithID(
        CK_INT callbackID,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U8, SizeParamIndex = 2)]
        CK_INT[] values,
        CK_UINT numValues
    );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void FloatArrayCallback(
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeParamIndex = 1)]
        CK_FLOAT[] values,
        CK_UINT numValues
    );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void NamedFloatArrayCallback(
        System.String name,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeParamIndex = 2)]
        CK_FLOAT[] values,
        CK_UINT numValues
    );

    [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
    public delegate void FloatArrayCallbackWithID(
        CK_INT callbackID,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R8, SizeParamIndex = 2)]
        CK_FLOAT[] values,
        CK_UINT numValues
    );

    private MyLogCallback chout_delegate;
    private MyLogCallback cherr_delegate;
    private MyLogCallback stdout_delegate;
    private MyLogCallback stderr_delegate;
    private Dictionary<string, IntCallback> intCallbacks;
    private Dictionary<string, FloatCallback> floatCallbacks;
    private Dictionary<string, StringCallback> stringCallbacks;
    private Dictionary<string, VoidCallback> voidCallbacks;
    private Dictionary<string, NamedIntCallback> namedIntCallbacks;
    private Dictionary<string, NamedFloatCallback> namedFloatCallbacks;
    private Dictionary<string, NamedStringCallback> namedStringCallbacks;
    private Dictionary<string, NamedVoidCallback> namedVoidCallbacks;
    private Dictionary<string, IntCallbackWithID> idIntCallbacks;
    private Dictionary<string, FloatCallbackWithID> idFloatCallbacks;
    private Dictionary<string, StringCallbackWithID> idStringCallbacks;
    private Dictionary<string, VoidCallbackWithID> idVoidCallbacks;

#if UNITY_WEBGL
    // method calls specific to WebGL
    public bool GetInt( System.UInt32 chuckID, string variableName, string gameObjectWithCallback, string callback )
    {
        getChuckIntWithUnityStyleCallback( chuckID, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetFloat( System.UInt32 chuckID, string variableName, string gameObjectWithCallback, string callback )
    {
        getChuckFloatWithUnityStyleCallback( chuckID, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetString( System.UInt32 chuckID, string variableName, string gameObjectWithCallback, string callback )
    {
        getChuckStringWithUnityStyleCallback( chuckID, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool ListenForChuckEventOnce( System.UInt32 chuckId, string variableName, string gameObjectWithCallback, string callback )
    {
        listenForChuckEventOnceWithUnityStyleCallback( chuckId, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool StartListeningForChuckEvent( System.UInt32 chuckId, string variableName, string gameObjectWithCallback, string callback )
    {
        startListeningForChuckEventWithUnityStyleCallback( chuckId, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool StopListeningForChuckEvent( System.UInt32 chuckId, string variableName, string gameObjectWithCallback, string callback )
    {
        stopListeningForChuckEventWithUnityStyleCallback( chuckId, variableName, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetIntArrayValue( System.UInt32 chuckId, string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        getGlobalIntArrayValueWithUnityStyleCallback( chuckId, variableName, index, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetAssociativeIntArrayValue( System.UInt32 chuckId, string variableName, string key, string gameObjectWithCallback, string callback )
    {
        getGlobalAssociativeIntArrayValueWithUnityStyleCallback( chuckId, variableName, key, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetFloatArrayValue( System.UInt32 chuckId, string variableName, uint index, string gameObjectWithCallback, string callback )
    {
        getGlobalFloatArrayValueWithUnityStyleCallback( chuckId, variableName, index, gameObjectWithCallback, callback );
        return true;
    }

    public bool GetAssociativeFloatArrayValue( System.UInt32 chuckId, string variableName, string key, string gameObjectWithCallback, string callback )
    {
        getGlobalAssociativeFloatArrayValueWithUnityStyleCallback( chuckId, variableName, key, gameObjectWithCallback, callback );
        return true;
    }
    
    const string PLUGIN_NAME = "__Internal";

    // imports specific to WebGL
    [DllImport( PLUGIN_NAME) ]
    private static extern void initChuckScript();
    [DllImport( PLUGIN_NAME) ]
    private static extern void getChuckIntWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getChuckFloatWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getChuckStringWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void listenForChuckEventOnceWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void startListeningForChuckEventWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void stopListeningForChuckEventWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getGlobalIntArrayValueWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.UInt32 index, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getGlobalAssociativeIntArrayValueWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String key, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getGlobalFloatArrayValueWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.UInt32 index, System.String gameObject, System.String method );
    [DllImport( PLUGIN_NAME) ]
    private static extern void getGlobalAssociativeFloatArrayValueWithUnityStyleCallback( System.UInt32 chuckID, System.String name, System.String key, System.String gameObject, System.String method );
    
#else
    #if UNITY_IOS && !UNITY_EDITOR
    const string PLUGIN_NAME = "__Internal";
    #else
    const string PLUGIN_NAME = "AudioPluginChuck";
    #endif

    // imports specific to Windows / Mac
    [DllImport( PLUGIN_NAME )]
    private static extern bool chuckManualAudioCallback( System.UInt32 chuckID, float[] inBuffer, float[] outBuffer,
        System.UInt32 numFrames, System.UInt32 inChannels, System.UInt32 outChannels );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalUGenSamples( System.UInt32 chuckID, System.String name,
        float[] buffer, System.Int32 numSamples );
#endif
    // TODO: actually implement named-gets in iOS and WebGL

    [DllImport( PLUGIN_NAME )]
    private static extern void cleanRegisteredChucks();

    [DllImport( PLUGIN_NAME )]
    private static extern bool initChuckInstance( System.UInt32 chuckID, System.UInt32 sampleRate );

    [DllImport( PLUGIN_NAME )]
    private static extern bool cleanupChuckInstance( System.UInt32 chuckID );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckCode( System.UInt32 chuckID, System.String code );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckCodeWithReplacementDac( System.UInt32 chuckID, System.String code, System.String replacement_dac );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckFile( System.UInt32 chuckID, System.String filename );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckFileWithReplacementDac( System.UInt32 chuckID, System.String filename, System.String replacement_dac );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckFileWithArgs( System.UInt32 chuckID, System.String filename, System.String args );

    [DllImport( PLUGIN_NAME )]
    private static extern bool runChuckFileWithArgsWithReplacementDac( System.UInt32 chuckID, System.String filename, System.String args, System.String replacement_dac );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setChuckInt( System.UInt32 chuckID, System.String name, CK_INT val );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckInt( System.UInt32 chuckID, System.String name, IntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedChuckInt( System.UInt32 chuckID, System.String name, NamedIntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckIntWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, IntCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setChuckFloat( System.UInt32 chuckID, System.String name, CK_FLOAT val );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckFloat( System.UInt32 chuckID, System.String name, FloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedChuckFloat( System.UInt32 chuckID, System.String name, NamedFloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckFloatWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, FloatCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setChuckString( System.UInt32 chuckID, System.String name, System.String val );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckString( System.UInt32 chuckID, System.String name, StringCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedChuckString( System.UInt32 chuckID, System.String name, NamedStringCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getChuckStringWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, StringCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool signalChuckEvent( System.UInt32 chuckID, System.String name );

    [DllImport( PLUGIN_NAME )]
    private static extern bool broadcastChuckEvent( System.UInt32 chuckID, System.String name );

    [DllImport( PLUGIN_NAME )]
    private static extern bool listenForChuckEventOnce( System.UInt32 chuckID, System.String name, VoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool listenForNamedChuckEventOnce( System.UInt32 chuckID, System.String name, NamedVoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool listenForChuckEventOnceWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, VoidCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool startListeningForChuckEvent( System.UInt32 chuckID, System.String name, VoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool startListeningForNamedChuckEvent( System.UInt32 chuckID, System.String name, NamedVoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool startListeningForChuckEventWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, VoidCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool stopListeningForChuckEvent( System.UInt32 chuckID, System.String name, VoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool stopListeningForNamedChuckEvent( System.UInt32 chuckID, System.String name, NamedVoidCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool stopListeningForChuckEventWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, VoidCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalIntArray( System.UInt32 chuckID, System.String name, CK_INT[] arrayValues, System.UInt32 numValues );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalIntArray( System.UInt32 chuckID, System.String name, IntArrayCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalIntArray( System.UInt32 chuckID, System.String name, NamedIntArrayCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalIntArrayWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, IntArrayCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalIntArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, CK_INT value );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalIntArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, IntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalIntArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, NamedIntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalIntArrayValueWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, System.UInt32 index, IntCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalAssociativeIntArrayValue( System.UInt32 chuckID, System.String name, System.String key, CK_INT value );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalAssociativeIntArrayValue( System.UInt32 chuckID, System.String name, System.String key, IntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalAssociativeIntArrayValue( System.UInt32 chuckID, System.String name, System.String key, NamedIntCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalAssociativeIntArrayValueWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, System.String key, IntCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalFloatArray( System.UInt32 chuckID, System.String name, CK_FLOAT[] arrayValues, System.UInt32 numValues );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalFloatArray( System.UInt32 chuckID, System.String name, FloatArrayCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalFloatArray( System.UInt32 chuckID, System.String name, NamedFloatArrayCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalFloatArrayWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, FloatArrayCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalFloatArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, CK_FLOAT value );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalFloatArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, FloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalFloatArrayValue( System.UInt32 chuckID, System.String name, System.UInt32 index, NamedFloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalFloatArrayValueWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, System.UInt32 index, FloatCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setGlobalAssociativeFloatArrayValue( System.UInt32 chuckID, System.String name, System.String key, CK_FLOAT value );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalAssociativeFloatArrayValue( System.UInt32 chuckID, System.String name, System.String key, FloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getNamedGlobalAssociativeFloatArrayValue( System.UInt32 chuckID, System.String name, System.String key, NamedFloatCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool getGlobalAssociativeFloatArrayValueWithID( System.UInt32 chuckID, CK_INT callbackID, System.String name, System.String key, FloatCallbackWithID callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setChoutCallback( System.UInt32 chuckID, MyLogCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setCherrCallback( System.UInt32 chuckID, MyLogCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setStdoutCallback( MyLogCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setStderrCallback( MyLogCallback callback );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setDataDir( System.String dir );

    [DllImport( PLUGIN_NAME )]
    private static extern bool setLogLevel( System.UInt32 level );

    private static Chuck __sharedInstance;
    private System.UInt32 _nextValidID;
    private Dictionary<string, System.UInt32> ids;

    private static AudioMixer chuckMixer;

    private Chuck()
    {
        // Store the location of data files
        setDataDir( Application.streamingAssetsPath );

        // Important in the editor, where native static arrays won't be cleaned up when entering / exiting play mode
        cleanRegisteredChucks();

        // First id is 1
        _nextValidID = 1;

        // Store exposed parameter names -> ids
        ids = new Dictionary<string, System.UInt32>();

        // Store global callbacks to avoid garbage collection of callbacks
        intCallbacks = new Dictionary<string, IntCallback>();
        floatCallbacks = new Dictionary<string, FloatCallback>();
        stringCallbacks = new Dictionary<string, StringCallback>();
        voidCallbacks = new Dictionary<string, VoidCallback>();
        namedIntCallbacks = new Dictionary<string, NamedIntCallback>();
        namedFloatCallbacks = new Dictionary<string, NamedFloatCallback>();
        namedStringCallbacks = new Dictionary<string, NamedStringCallback>();
        namedVoidCallbacks = new Dictionary<string, NamedVoidCallback>();
        idIntCallbacks = new Dictionary<string, IntCallbackWithID>();
        idFloatCallbacks = new Dictionary<string, FloatCallbackWithID>();
        idStringCallbacks = new Dictionary<string, StringCallbackWithID>();
        idVoidCallbacks = new Dictionary<string, VoidCallbackWithID>();

        // Create and store callbacks
        chout_delegate = new MyLogCallback( ChoutCallback );
        cherr_delegate = new MyLogCallback( CherrCallback );
        stdout_delegate = new MyLogCallback( StdoutCallback );
        stderr_delegate = new MyLogCallback( StderrCallback );

        // Store pointers to callbacks inside ChucK's inner workings
        setStdoutCallback( stdout_delegate );
        setStderrCallback( stderr_delegate );

        // Load up the ChuckMixer
        chuckMixer = Resources.Load( "Mixers/ChuckMixer" ) as AudioMixer;
    }

    public void Quit()
    {
        Debug.Log( "ChucK quitting now" );
        cleanRegisteredChucks();
    }

    [AOT.MonoPInvokeCallback(typeof(MyLogCallback))]
    static void ChoutCallback( System.String str )
    {
        Debug.Log( "[chout]: " + str );
    }

    [AOT.MonoPInvokeCallback(typeof(MyLogCallback))]
    static void CherrCallback( System.String str )
    {
        Debug.LogError( "[cherr]: " + str );
    }

    [AOT.MonoPInvokeCallback(typeof(MyLogCallback))]
    static void StdoutCallback( System.String str )
    {
        Debug.Log( str );
    }

    [AOT.MonoPInvokeCallback(typeof(MyLogCallback))]
    static void StderrCallback( System.String str )
    {
        Debug.LogError( str );
    }

    public static AudioMixerGroup FindAudioMixerGroup( string name )
    {
        return chuckMixer.FindMatchingGroups( name )[0];
    }
}
