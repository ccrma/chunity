#pragma once

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "chuck.h"

extern "C" {

namespace ChucK_For_Unity {
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code );
    
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val );
    UNITY_INTERFACE_EXPORT bool getChuckInt( unsigned int chuckID, const char * name, void (* callback)(t_CKINT) );
    
    UNITY_INTERFACE_EXPORT bool setChuckFloat( unsigned int chuckID, const char * name, t_CKFLOAT val );
    UNITY_INTERFACE_EXPORT bool getChuckFloat( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT) );
    
    UNITY_INTERFACE_EXPORT bool signalChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool broadcastChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool listenForChuckEventOnce( unsigned int chuckID, const char * name, void (* callback)(void) );
    UNITY_INTERFACE_EXPORT bool startListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) );
    UNITY_INTERFACE_EXPORT bool stopListeningForChuckEvent( unsigned int chuckID, const char * name, void (* callback)(void) );
    
    UNITY_INTERFACE_EXPORT bool initChuckInstance( unsigned int chuckID, unsigned int sampleRate );
    UNITY_INTERFACE_EXPORT bool cleanupChuckInstance( unsigned int chuckID );
    UNITY_INTERFACE_EXPORT bool chuckManualAudioCallback( unsigned int chuckID, float * inBuffer, float * outBuffer, unsigned int numFrames, unsigned int inChannels, unsigned int outChannels );
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks();
    
    UNITY_INTERFACE_EXPORT bool setChoutCallback( unsigned int chuckID, void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setCherrCallback( unsigned int chuckID, void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStdoutCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStderrCallback( void (* callback)(const char *) );
    
    UNITY_INTERFACE_EXPORT bool setDataDir( const char * dir );
};

};