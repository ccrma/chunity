#pragma once

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

extern "C" {

namespace ChucK {
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code );
    UNITY_INTERFACE_EXPORT void cleanRegisteredChucks();
};

};