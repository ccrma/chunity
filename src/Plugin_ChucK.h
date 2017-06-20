#pragma once

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "chuck_external.h"

extern "C" {

namespace ChucK {
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code );
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val );
    UNITY_INTERFACE_EXPORT bool getChuckInt( unsigned int chuckID, const char * name, void (* callback)(t_CKINT) );
    UNITY_INTERFACE_EXPORT bool setChuckFloat( unsigned int chuckID, const char * name, t_CKFLOAT val );
    UNITY_INTERFACE_EXPORT bool getChuckFloat( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT) );
    UNITY_INTERFACE_EXPORT bool signalChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool broadcastChuckEvent( unsigned int chuckID, const char * name );
    UNITY_INTERFACE_EXPORT bool initChuckInstance( unsigned int chuckID, unsigned int sampleRate );
    UNITY_INTERFACE_EXPORT bool chuckManualAudioCallback( unsigned int chuckID, float * inBuffer, float * outBuffer, unsigned int numFrames, unsigned int inChannels, unsigned int outChannels );
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks();
    UNITY_INTERFACE_EXPORT bool setChoutCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setCherrCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStdoutCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setStderrCallback( void (* callback)(const char *) );
    UNITY_INTERFACE_EXPORT bool setDataDir( const char * dir );
};

};