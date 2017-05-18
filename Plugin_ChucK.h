#pragma once

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "chuck_system.h"

extern "C" {

namespace ChucK {
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code );
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val );
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks();
    UNITY_INTERFACE_EXPORT bool setChoutCallback( void (*fp)(const char *) );
    UNITY_INTERFACE_EXPORT bool setCherrCallback( void (*fp)(const char *) );
};

};