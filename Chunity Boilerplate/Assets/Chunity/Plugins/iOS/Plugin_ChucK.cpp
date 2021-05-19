//
//  Plugin_ChucK.cpp
//  AudioPluginDemo
//
//  Created by Jack Atherton on 4/19/17.
//
//


#include "Plugin_ChucK.h"
#include "chuck_globals.h"

#include <iostream>
#include <map>
#ifndef WIN32
#include <unistd.h>
#endif

extern "C"
{
    // If exported by a plugin, this function will be called when the plugin is loaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
    {
        // Things that need to be common to ALL ChucK instances will be loaded here
        // (This seems to be called reliably.)
    }
    
    // If exported by a plugin, this function will be called when the plugin is about to be unloaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
    {
        // Things that need to be common to ALL ChucK instances will be unloaded here
        // (I don't think this is being called. See https://medium.com/@bengreenier/building-native-unity-plugins-with-visual-studio-8f470e5af9ca )
        ChucK::globalCleanup();
    }

}

namespace ChucK_For_Unity
{
    enum Param
    {
        P_CHUCKID,
        P_NUM
    };

    struct EffectData
    {
        struct Data
        {
            float p[P_NUM];
            t_CKINT myId;
            bool initialized;
        };
        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };
    
    std::map< unsigned int, ChucK * > chuck_instances;
    std::map< unsigned int, EffectData::Data * > data_instances;
    std::string chuck_global_data_dir;



    // C# "string" corresponds to passing char *
    UNITY_INTERFACE_EXPORT bool EMSCRIPTEN_KEEPALIVE runChuckCode( unsigned int chuckID, const char * code )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        // don't want to replace dac
        // (a safeguard in case compiler got interrupted while replacing dac)
        chuck_instances[chuckID]->compiler()->setReplaceDac( FALSE, "" );

        // compile it!
        return chuck_instances[chuckID]->compileCode(
            std::string( code ), std::string("") );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool runChuckCodeWithReplacementDac(
        unsigned int chuckID, const char * code, const char * replacement_dac )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        // replace dac
        chuck_instances[chuckID]->compiler()->setReplaceDac( TRUE,
            std::string( replacement_dac ) );
        
        // compile it!
        bool ret = chuck_instances[chuckID]->compileCode(
            std::string( code ), std::string("") );
        
        // don't replace dac for future compilations
        chuck_instances[chuckID]->compiler()->setReplaceDac( FALSE, "" );
        
        return ret;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool runChuckFile( unsigned int chuckID,
        const char * filename )
    {
        // run with empty args
        return runChuckFileWithArgs( chuckID, filename, "" );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool runChuckFileWithArgs( unsigned int chuckID,
        const char * filename, const char * args )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        // don't want to replace dac
        // (a safeguard in case compiler got interrupted while replacing dac)
        chuck_instances[chuckID]->compiler()->setReplaceDac( FALSE, "" );

        // compile it!
        return chuck_instances[chuckID]->compileFile(
            std::string( filename ), std::string( args )
        );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool runChuckFileWithReplacementDac(
        unsigned int chuckID, const char * filename,
        const char * replacement_dac )
    {
        // run with empty args
        return runChuckFileWithArgsWithReplacementDac(
            chuckID, filename, "", replacement_dac
        );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool runChuckFileWithArgsWithReplacementDac(
        unsigned int chuckID, const char * filename, const char * args,
        const char * replacement_dac )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        // replace dac
        chuck_instances[chuckID]->compiler()->setReplaceDac( TRUE,
            std::string( replacement_dac ) );
        
        // compile it!
        bool ret = chuck_instances[chuckID]->compileFile(
            std::string( filename ), std::string( args )
        );
        
        // don't replace dac for future compilations
        chuck_instances[chuckID]->compiler()->setReplaceDac( FALSE, "" );
        
        return ret;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->setGlobalInt( name, val );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getChuckInt( unsigned int chuckID, const char * name, void (* callback)(t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalInt( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedChuckInt( unsigned int chuckID, const char * name, void (* callback)(const char *, t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalInt( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getChuckIntWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT, t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalInt( name, callbackID, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChuckFloat( unsigned int chuckID, const char * name, t_CKFLOAT val )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->setGlobalFloat( name, val );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getChuckFloat( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalFloat( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedChuckFloat( unsigned int chuckID, const char * name, void (* callback)(const char *, t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalFloat( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getChuckFloatWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT, t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalFloat( name, callbackID, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChuckString( unsigned int chuckID, const char * name, const char * val )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->setGlobalString( name, val );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getChuckString( unsigned int chuckID, const char * name, void (* callback)(const char *) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalString( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedChuckString( unsigned int chuckID, const char * name, void (* callback)(const char *, const char *) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalString( name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getChuckStringWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT, const char*) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->getGlobalString( name, callbackID, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool signalChuckEvent( unsigned int chuckID, const char * name )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->signalGlobalEvent( name );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool broadcastChuckEvent( unsigned int chuckID, const char * name )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }

        return gm->broadcastGlobalEvent( name );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool listenForChuckEventOnce( unsigned int chuckID, const char * name, void (* callback)(void) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callback, FALSE );
    }



    UNITY_INTERFACE_EXPORT bool listenForNamedChuckEventOnce( unsigned int chuckID, const char * name, void (* callback)(const char *) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callback, FALSE );
    }



    UNITY_INTERFACE_EXPORT bool listenForChuckEventOnceWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callbackID, callback, FALSE );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool startListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callback, TRUE );
    }



    UNITY_INTERFACE_EXPORT bool startListeningForNamedChuckEvent( unsigned int chuckID, const char * name, void (* callback)(const char *) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callback, TRUE );
    }



    UNITY_INTERFACE_EXPORT bool startListeningForChuckEventWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->listenForGlobalEvent(
            name, callbackID, callback, TRUE );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool stopListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->stopListeningForGlobalEvent(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool stopListeningForNamedChuckEvent( unsigned int chuckID, const char * name, void (* callback)(const char *) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->stopListeningForGlobalEvent(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool stopListeningForChuckEventWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->stopListeningForGlobalEvent(
            name, callbackID, callback );
    }
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalUGenSamples( unsigned int chuckID,
        const char * name, SAMPLE * buffer, int numSamples )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        if( !gm->getGlobalUGenSamples(
            name, buffer, numSamples ) )
        {
            // failed. fill with zeroes.
            memset( buffer, 0, sizeof( SAMPLE ) * numSamples );
            return false;
        }
        
        return true;
    }
    
    
    
    // int array methods
    UNITY_INTERFACE_EXPORT bool setGlobalIntArray( unsigned int chuckID,
        const char * name, t_CKINT arrayValues[], unsigned int numValues )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->setGlobalIntArray(
            name, arrayValues, numValues );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalIntArray( unsigned int chuckID,
        const char * name, void (* callback)(t_CKINT[], t_CKUINT))
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArray(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalIntArray( unsigned int chuckID,
        const char * name, void (* callback)(const char *, t_CKINT[], t_CKUINT))
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArray(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalIntArrayWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT, t_CKINT[], t_CKUINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArray(
            name, callbackID, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setGlobalIntArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, t_CKINT value )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->setGlobalIntArrayValue(
            name, index, value );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalIntArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, void (* callback)(t_CKINT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArrayValue(
            name, index, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalIntArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, void (* callback)(const char *, t_CKINT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArrayValue(
            name, index, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalIntArrayValueWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, unsigned int index, void(*callback)(t_CKINT, t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalIntArrayValue(
            name, callbackID, index, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setGlobalAssociativeIntArrayValue(
        unsigned int chuckID, const char * name, char * key, t_CKINT value )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->setGlobalAssociativeIntArrayValue(
            name, key, value );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalAssociativeIntArrayValue(
        unsigned int chuckID, const char * name, char * key,
        void (* callback)(t_CKINT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeIntArrayValue(
            name, key, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalAssociativeIntArrayValue(
        unsigned int chuckID, const char * name, char * key,
        void (* callback)(const char *, t_CKINT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeIntArrayValue(
            name, key, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalAssociativeIntArrayValueWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, char* key, void(*callback)(t_CKINT, t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeIntArrayValue(
            name, callbackID, key, callback );
    }
    
    
    
    // float array methods
    UNITY_INTERFACE_EXPORT bool setGlobalFloatArray( unsigned int chuckID,
        const char * name, t_CKFLOAT arrayValues[], unsigned int numValues )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->setGlobalFloatArray(
            name, arrayValues, numValues );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalFloatArray( unsigned int chuckID,
        const char * name, void (* callback)(t_CKFLOAT[], t_CKUINT))
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArray(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalFloatArray( unsigned int chuckID,
        const char * name, void (* callback)(const char *, t_CKFLOAT[], t_CKUINT))
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArray(
            name, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalFloatArrayWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, void(*callback)(t_CKINT, t_CKFLOAT[], t_CKUINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArray(
            name, callbackID, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setGlobalFloatArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, t_CKFLOAT value )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->setGlobalFloatArrayValue(
            name, index, value );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalFloatArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, void (* callback)(t_CKFLOAT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArrayValue(
            name, index, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalFloatArrayValue( unsigned int chuckID,
        const char * name, unsigned int index, void (* callback)(const char *, t_CKFLOAT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArrayValue(
            name, index, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalFloatArrayValueWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, unsigned int index, void(*callback)(t_CKINT, t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalFloatArrayValue(
            name, callbackID, index, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setGlobalAssociativeFloatArrayValue(
        unsigned int chuckID, const char * name, char * key, t_CKFLOAT value )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->setGlobalAssociativeFloatArrayValue(
            name, key, value );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getGlobalAssociativeFloatArrayValue(
        unsigned int chuckID, const char * name, char * key,
        void (* callback)(t_CKFLOAT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeFloatArrayValue(
            name, key, callback );
    }



    UNITY_INTERFACE_EXPORT bool getNamedGlobalAssociativeFloatArrayValue(
        unsigned int chuckID, const char * name, char * key,
        void (* callback)(const char *, t_CKFLOAT) )
    {
       if( chuck_instances.count( chuckID ) == 0 ) { return false; }
       Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
       if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeFloatArrayValue(
            name, key, callback );
    }



    UNITY_INTERFACE_EXPORT bool getGlobalAssociativeFloatArrayValueWithID( unsigned int chuckID, t_CKINT callbackID, const char* name, char* key, void(*callback)(t_CKINT, t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        Chuck_Globals_Manager * gm = chuck_instances[chuckID]->globals();
        if( gm == NULL ) { return false; }
        
        return gm->getGlobalAssociativeFloatArrayValue(
            name, callbackID, key, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChoutCallback( unsigned int chuckID, void (* callback)(const char *) )
    {
        return chuck_instances[chuckID]->setChoutCallback( callback );
    }



    UNITY_INTERFACE_EXPORT bool setCherrCallback( unsigned int chuckID, void (* callback)(const char *) )
    {
        return chuck_instances[chuckID]->setCherrCallback( callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setStdoutCallback( void (* callback)(const char *) )
    {
        return ChucK::setStdoutCallback( callback );
    }



    UNITY_INTERFACE_EXPORT bool setStderrCallback( void (* callback)(const char *) )
    {
        return ChucK::setStderrCallback( callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setDataDir( const char * dir )
    {
        chuck_global_data_dir = std::string( dir );
        return true;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setLogLevel( unsigned int level )
    {
        EM_setlog( level );
        return true;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool EMSCRIPTEN_KEEPALIVE initChuckInstance( unsigned int chuckID, unsigned int sampleRate )
    {
        if( chuck_instances.count( chuckID ) == 0 )
        {
            // if we aren't tracking a chuck vm on this ID, create a new one
            ChucK * chuck = new ChucK();
            
            // set params: sample rate, 2 in channels, 2 out channels,
            // don't halt the vm, and use our data directory
            chuck->setParam( CHUCK_PARAM_SAMPLE_RATE, (t_CKINT) sampleRate );
            chuck->setParam( CHUCK_PARAM_INPUT_CHANNELS, (t_CKINT) 2 );
            chuck->setParam( CHUCK_PARAM_OUTPUT_CHANNELS, (t_CKINT) 2 );
            chuck->setParam( CHUCK_PARAM_VM_HALT, (t_CKINT) 0 );
            chuck->setParam( CHUCK_PARAM_DUMP_INSTRUCTIONS, (t_CKINT) 0 );
            // directory for compiled.code
            chuck->setParam( CHUCK_PARAM_WORKING_DIRECTORY, chuck_global_data_dir );
            // directories to search for chugins and auto-run ck files
            std::list< std::string > chugin_search;
            chugin_search.push_back( chuck_global_data_dir + "/Chugins" );
            chugin_search.push_back( chuck_global_data_dir + "/ChuGins" );
            chugin_search.push_back( chuck_global_data_dir + "/chugins" );
            chuck->setParam( CHUCK_PARAM_USER_CHUGIN_DIRECTORIES, chugin_search );
            
            // initialize and start
            chuck->init();
            chuck->start();
            
            chuck_instances[chuckID] = chuck;
        }
        return true;
    }



    UNITY_INTERFACE_EXPORT bool clearChuckInstance( unsigned int chuckID )
    {
        if( chuck_instances.count( chuckID ) > 0 )
        {
            // the chuck to clear
            ChucK * chuck = chuck_instances[chuckID];
            
            // create a msg asking to clear the VM
            Chuck_Msg * msg = new Chuck_Msg;
            msg->type = MSG_CLEARVM;
            
            // null reply so that VM will delete for us when it's done
            msg->reply = ( ck_msg_func )NULL;
            
            // tell the VM to clear
            chuck->vm()->globals_manager()->execute_chuck_msg_with_globals( msg );
            
            return true;
        }
        
        return false;
    }



    UNITY_INTERFACE_EXPORT bool clearGlobals( unsigned int chuckID )
    {
        if( chuck_instances.count( chuckID ) > 0 )
        {
            // the chuck to clear
            ChucK * chuck = chuck_instances[chuckID];
            
            // create a msg asking to clear the globals
            Chuck_Msg * msg = new Chuck_Msg;
            msg->type = MSG_CLEARGLOBALS;
            
            // null reply so that VM will delete for us when it's done
            msg->reply = ( ck_msg_func )NULL;
            
            // tell the VM to clear
            chuck->vm()->globals_manager()->execute_chuck_msg_with_globals( msg );
            
            return true;
        }
        
        return false;
    }


    UNITY_INTERFACE_EXPORT bool cleanupChuckInstance( unsigned int chuckID )
    {
        if( chuck_instances.count( chuckID ) > 0 )
        {
            ChucK * chuck = chuck_instances[chuckID];
            
            // don't track it anymore
            chuck_instances.erase( chuckID );

            if( data_instances.count( chuckID ) > 0 )
            {
                data_instances[chuckID]->myId = -1;
                data_instances.erase( chuckID );
            }

            // wait a bit
            usleep( 30000 );

            // cleanup this chuck early
            delete chuck;

        }

        return true;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool EMSCRIPTEN_KEEPALIVE chuckManualAudioCallback( unsigned int chuckID, float * inBuffer, float * outBuffer, unsigned int numFrames, unsigned int inChannels, unsigned int outChannels )
    {
        if( chuck_instances.count( chuckID ) > 0 )
        {
            // zero out the output buffer, in case chuck isn't running
            for( unsigned int n = 0; n < numFrames * outChannels; n++ )
            {
                outBuffer[n] = 0;
            }
            
            // call callback
            // TODO: check inChannels, outChannels
            chuck_instances[chuckID]->run( inBuffer, outBuffer, numFrames );
            
        }
        
        return true;
    }



    // on launch, reset all ids (necessary when relaunching a lot in unity editor)
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks() {
    
        // first, invalidate all callbacks' references to chucks
        for( std::map< unsigned int, EffectData::Data * >::iterator it =
             data_instances.begin(); it != data_instances.end(); it++ )
        {
            EffectData::Data * data = it->second;
            data->myId = -1;
        }
        
        // wait for callbacks to finish their current run
        usleep( 30000 );
        
        // next, delete chucks
        for( std::map< unsigned int, ChucK * >::iterator it =
             chuck_instances.begin(); it != chuck_instances.end(); it++ )
        {
            ChucK * chuck = it->second;
            delete chuck;
        }
        
        // delete stored chuck pointers
        chuck_instances.clear();
        // delete data instances
        data_instances.clear();
        
        // clear out callbacks also
        setStdoutCallback( NULL );
        setStderrCallback( NULL );
    }

    
    bool RegisterChuckData( EffectData::Data * data, const unsigned int id )
    {
        // only store if id has been used / a chuck is already initialized
        if( chuck_instances.count( id ) == 0 )
        {
            return false;
        }
        
        // store id on data; note we might be replacing a non-zero id
        //  in the case when unity is reusing an audio callback the next time
        //  the scene is entered.
        data->myId = id;
        
        // store the data pointer, for validation later.
        // the chuck associated with id should only work with *this* data.
        data_instances[id] = data;
        
        return true;
    }
    

#if !UNITY_SPU

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
        // float vals are: min, max, default, scale (?), scale (?)
        RegisterParameter( definition, "ChucK ID", "#", -1.0f, 256.0f, -1.0f, 1.0f, 1.0f, P_CHUCKID, "Internal ID number used to run ChucK scripts programmatically. Leave set to -1 manually.");
        return numparams;
    }


    // NOTE: CreateCallback and ReleaseCallback are called at odd times, e.g.
    // - When unity launches for the first time and when unity exits
    // - When a new effect is added
    // - When a parameter is exposed
    // - NOT when play mode is activated or deactivated
    // instantiation
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        
        // don't hook into any particular chuck; id not yet set
        effectdata->data.myId = -1;
        
        state->effectdata = effectdata;
        InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->data.p);

        return UNITY_AUDIODSP_OK;
    }


    // deletion
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData::Data * data = &state->GetEffectData<EffectData>()->data;
        
        delete data;

        return UNITY_AUDIODSP_OK;
    }


    // set param (piggy-backed to store chucks by id)
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        
        // setting ID, time to cache the pointer to effect data
        if( index == P_CHUCKID && value >= 0.0f )
        {
            // if false( ie already registered data or id ),
            // an error will be outputted to the Unity console
            if( RegisterChuckData( data, ( unsigned int )value ) == false )
            {
                return UNITY_AUDIODSP_ERR_UNSUPPORTED;
            }
        }
        
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }


    // get param (not useful for anything for ChucK)
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL) {
            *value = data->p[index];
        }
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }


    // get float buffer (not useful for anything for ChucK)
    int UNITY_AUDIODSP_CALLBACK GetFloatBufferCallback(UnityAudioEffectState* state, const char* name, float* buffer, int numsamples)
    {
        return UNITY_AUDIODSP_OK;
    }

#endif

#if !UNITY_PS3 || UNITY_SPU

#if UNITY_SPU
    EffectData  g_EffectData __attribute__((aligned(16)));
    extern "C"
#endif
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ProcessCallback(UnityAudioEffectState* state, float* inbuffer, float* outbuffer, unsigned int length, int inchannels, int outchannels)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

#if UNITY_SPU
        UNITY_PS3_CELLDMA_GET(&g_EffectData, state->effectdata, sizeof(g_EffectData));
        data = &g_EffectData.data;
#endif

        // zero out the output buffer, in case chuck isn't running
        for( unsigned int n = 0; n < length * outchannels; n++ )
        {
            outbuffer[n] = 0;
        }
        
        // if we think we can, call chuck callback
        if( chuck_instances.count( data->myId ) > 0    // do we have a chuck
            && data_instances.count( data->myId ) > 0  // do we have a data
            && data_instances[data->myId] == data )    // && is it still aligned
        {
            ChucK * chuck = chuck_instances[data->myId];

            // TODO: check inChannels, outChannels
            chuck->run( inbuffer, outbuffer, length );
        }
        
        // Need to add small amount of white noise (amplitude 0.00017 is fine)
        // to prevent Unity from disabling our plugin sometimes
        for (unsigned int n = 0; n < length; n++)
        {
            for (int i = 0; i < outchannels; i++)
            {
                // multiplier:
                // 0.00017 works
                // 0.00015 does not work
                outbuffer[n * outchannels + i] += rand() * 0.00017 / RAND_MAX;
            }
        }

#if UNITY_SPU
        UNITY_PS3_CELLDMA_PUT(&g_EffectData, state->effectdata, sizeof(g_EffectData));
#endif

        return UNITY_AUDIODSP_OK;
    }

#endif
}
