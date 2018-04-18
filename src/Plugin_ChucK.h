#pragma once

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "chuck.h"

extern "C" {

namespace ChucK_For_Unity {
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code );
    UNITY_INTERFACE_EXPORT bool runChuckCodeWithReplacementDac( unsigned int chuckID, const char * code, const char * replacement_dac );
    UNITY_INTERFACE_EXPORT bool runChuckFile( unsigned int chuckID, const char * filename );
    UNITY_INTERFACE_EXPORT bool runChuckFileWithReplacementDac( unsigned int chuckID, const char * filename, const char * replacement_dac );
    UNITY_INTERFACE_EXPORT bool runChuckFileWithArgs( unsigned int chuckID, const char * filename, const char * args );
    UNITY_INTERFACE_EXPORT bool runChuckFileWithArgsWithReplacementDac( unsigned int chuckID, const char * filename, const char * args, const char * replacement_dac );
    
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val );
    UNITY_INTERFACE_EXPORT bool getChuckInt( unsigned int chuckID, const char * name, void (* callback)(t_CKINT) );
    
    UNITY_INTERFACE_EXPORT bool setChuckFloat( unsigned int chuckID, const char * name, t_CKFLOAT val );
    UNITY_INTERFACE_EXPORT bool getChuckFloat( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT) );
    
    UNITY_INTERFACE_EXPORT bool setChuckString( unsigned int chuckID, const char * name, const char * val );
    UNITY_INTERFACE_EXPORT bool getChuckString( unsigned int chuckID, const char * name, void (* callback)(const char *) );
    
    UNITY_INTERFACE_EXPORT bool signalChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool broadcastChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool listenForChuckEventOnce( unsigned int chuckID, const char * name, void (* callback)(void) );
    UNITY_INTERFACE_EXPORT bool startListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) );
    UNITY_INTERFACE_EXPORT bool stopListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) );
    
    UNITY_INTERFACE_EXPORT bool getExternalUGenSamples( unsigned int chuckID, const char * name, SAMPLE * buffer, int numSamples );
    
    // int array methods
    UNITY_INTERFACE_EXPORT bool setExternalIntArray( unsigned int chuckID, const char * name, t_CKINT arrayValues[], uint numValues );
    UNITY_INTERFACE_EXPORT bool getExternalIntArray( unsigned int chuckID, const char * name, void (* callback)(t_CKINT[], t_CKUINT));
    UNITY_INTERFACE_EXPORT bool setExternalIntArrayValue( unsigned int chuckID, const char * name, unsigned int index, t_CKINT value );
    UNITY_INTERFACE_EXPORT bool getExternalIntArrayValue( unsigned int chuckID, const char * name, unsigned int index, void (* callback)(t_CKINT) );
    UNITY_INTERFACE_EXPORT bool setExternalAssociativeIntArrayValue( unsigned int chuckID, const char * name, char * key, t_CKINT value );
    UNITY_INTERFACE_EXPORT bool getExternalAssociativeIntArrayValue( unsigned int chuckID, const char * name, char * key, void (* callback)(t_CKINT) );
    // TODO: set entire dict, add to dict in batch; get entire dict
    
    // float array methods
    UNITY_INTERFACE_EXPORT bool setExternalFloatArray( unsigned int chuckID, const char * name, t_CKFLOAT arrayValues[], uint numValues );
    UNITY_INTERFACE_EXPORT bool getExternalFloatArray( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT[], t_CKUINT));
    UNITY_INTERFACE_EXPORT bool setExternalFloatArrayValue( unsigned int chuckID, const char * name, unsigned int index, t_CKFLOAT value );
    UNITY_INTERFACE_EXPORT bool getExternalFloatArrayValue( unsigned int chuckID, const char * name, unsigned int index, void (* callback)(t_CKFLOAT) );
    UNITY_INTERFACE_EXPORT bool setExternalAssociativeFloatArrayValue( unsigned int chuckID, const char * name, char * key, t_CKFLOAT value );
    UNITY_INTERFACE_EXPORT bool getExternalAssociativeFloatArrayValue( unsigned int chuckID, const char * name, char * key, void (* callback)(t_CKFLOAT) );
    
    
    UNITY_INTERFACE_EXPORT bool initChuckInstance( unsigned int chuckID, unsigned int sampleRate );
    UNITY_INTERFACE_EXPORT bool cleanupChuckInstance( unsigned int chuckID );
    UNITY_INTERFACE_EXPORT bool chuckManualAudioCallback( unsigned int chuckID, float * inBuffer, float * outBuffer, unsigned int numFrames, unsigned int inChannels, unsigned int outChannels );
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks();
    
    UNITY_INTERFACE_EXPORT bool setChoutCallback( unsigned int chuckID, void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setCherrCallback( unsigned int chuckID, void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStdoutCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStderrCallback( void (* callback)(const char *) );
    
    UNITY_INTERFACE_EXPORT bool setDataDir( const char * dir );
    
    UNITY_INTERFACE_EXPORT bool setLogLevel( unsigned int level );
};

};
