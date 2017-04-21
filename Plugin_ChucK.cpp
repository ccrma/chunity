//
//  Plugin_ChucK.cpp
//  AudioPluginDemo
//
//  Created by Jack Atherton on 4/19/17.
//
//

#include "AudioPluginUtil.h"
#include "IUnityInterface.h"
#include "IUnityGraphics.h"

#include "chuck_system.h"
#include <iostream>

namespace ChucK
{
//    bool isChuckRunning = false;
//    Chuck_System chuck;
    t_CKUINT numInstances;
    enum Param
    {
//        P_FREQ,
//        P_MIX,
        P_CHUCKPTR,
        P_NUM
    };

    struct EffectData
    {
        struct Data
        {
            float p[P_NUM];
            Chuck_System * chuck;
            XThread * loop_thread;
        };
        union
        {
            Data data;
            unsigned char pad[(sizeof(Data) + 15) & ~15]; // This entire structure must be a multiple of 16 bytes (and and instance 16 byte aligned) for PS3 SPU DMA requirements
        };
    };
    
    void * launchChuck( void * c )
    {
        Chuck_System * chuck = (Chuck_System *) c;
        
        // TODO: is this a terrible way to construct a ** char?
        std::vector< char * > argsVector;
        char arg1[] = "chuck";
        char arg2[] = "--loop";
        // char arg3[] = "--silent";
        argsVector.push_back( & arg1[0] );
        argsVector.push_back( & arg2[0] );
        // argsVector.push_back( & arg3[0] );
        const char ** args = (const char **) & argsVector[0];
        // TODO: need to be successful in killing this when Unity shuts down!!
        // turns out shutdowns and startups are called a LOT
        chuck->go( 2, args, FALSE, TRUE );
//        chuck.go( 1, args, FALSE, TRUE );
//        NOTE: trying to do it this way causes horrible crashes to all of unity!
//        chuck.vm()->start();
        

        // TODO: add shreds another way than in the launch code
        // Wow, um the most recently, this ran more than one time. which is weird because chuck should only be launched once. So there must be more than one instance of the plugin or...
        // TODO: this stopped making sound. which makes me sad. :( but at least unity can fully quit now.
        chuck->compileCode("SinOsc foo => dac; repeat(10) { Math.random2f(300, 1000) => foo.freq; 200::ms => now; }", "");
        
        numInstances++;
        
        return NULL;
    };
    
    // If exported by a plugin, this function will be called when the plugin is loaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
    {
        // Things that need to be common to ALL ChucK instances will be loaded here
    }
    
    
    
    // PROBLEM: This shutdown code is not being run!
    // If exported by a plugin, this function will be called when the plugin is about to be unloaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
    {
        // Things that need to be common to ALL ChucK instances will be unloaded here
        // all_detach();
        // NOTE: I don't think this function is getting called when unity is closed.
        signal_int( 2 );
    }
    
    // PROBLEM: THIS shutdown code is also not being run!!!
    static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
    {
        switch (eventType)
        {
            case kUnityGfxDeviceEventInitialize:
            {
                // s_RendererType = s_Graphics->GetRenderer();
                //TODO: user initialization code
                break;
            }
            case kUnityGfxDeviceEventShutdown:
            {
                // s_RendererType = kUnityGfxRendererNull;
                // user shutdown code
                signal_int( 2 );
                
                
                
                break;
            }
            case kUnityGfxDeviceEventBeforeReset:
            {
                //TODO: user Direct3D 9 code
                break;
            }
            case kUnityGfxDeviceEventAfterReset:
            {
                //TODO: user Direct3D 9 code
                break;
            }
        };
    }




#if !UNITY_SPU

    int InternalRegisterEffectDefinition(UnityAudioEffectDefinition& definition)
    {
        int numparams = P_NUM;
        definition.paramdefs = new UnityAudioParameterDefinition[numparams];
// TODO: somehow register the ChucK ptr or at least keep it from being edited...
        // float vals are: min, max, default, scale (?), scale (?)
        RegisterParameter( definition, "ChucK Pointer", "Don't touch", 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, P_CHUCKPTR, "Pointer used to run ChucK scripts programmatically");
        return numparams;
    }


    // instantiation
    // TODO: problems occur when you try to instantiate more than one chuck
    // is it because it's trying to access TCP or because of some other thing?
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK CreateCallback(UnityAudioEffectState* state)
    {
        EffectData* effectdata = new EffectData;
        memset(effectdata, 0, sizeof(EffectData));
        
        // create chuck and launch it on a new thread (without audio callback)
        effectdata->data.chuck = new Chuck_System;
        effectdata->data.loop_thread = new XThread();
        //effectdata->data.loop_thread->start( launchChuck, effectdata->data.chuck );
        launchChuck( effectdata->data.chuck );
        
        state->effectdata = effectdata;
        InitParametersFromDefinitions(InternalRegisterEffectDefinition, effectdata->data.p);
        // :( int params are not allowed, but can't cast ptr to float :(
        //effectdata->data.p[P_CHUCKPTR] = (float) effectdata->data.chuck;
        
//        if (!isChuckRunning) {
//            // loop and wait for incoming shreds
//            // problem: need to shut down somehow :(
//            
//            // not sure if this works!
//            XThread * loop_thread = new XThread();
//            loop_thread->start( launchChuck, NULL );
//            isChuckRunning = true;
//        }
        
        
        return UNITY_AUDIODSP_OK;
    }


    // deletion
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK ReleaseCallback(UnityAudioEffectState* state)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;

        Chuck_System * chuck = data->chuck;
        // TODO: this runs "all_stop" and "all_detach" which might be bad if we have more than one chuck running
        chuck->clientPartialShutdown();
        //signal_int(2); // send "control-c"; this also runs all_stop and all_detach
    
        numInstances--;
        
        delete chuck;
        delete data;

        return UNITY_AUDIODSP_OK;
    }


    // set param
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK SetFloatParameterCallback(UnityAudioEffectState* state, int index, float value)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        data->p[index] = value;
        return UNITY_AUDIODSP_OK;
    }


    // get param
    UNITY_AUDIODSP_RESULT UNITY_AUDIODSP_CALLBACK GetFloatParameterCallback(UnityAudioEffectState* state, int index, float* value, char *valuestr)
    {
        EffectData::Data* data = &state->GetEffectData<EffectData>()->data;
        if (index >= P_NUM)
            return UNITY_AUDIODSP_ERR_UNSUPPORTED;
        if (value != NULL)
            *value = data->p[index];
        if (valuestr != NULL)
            valuestr[0] = 0;
        return UNITY_AUDIODSP_OK;
    }

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
        Chuck_System * chuck = data->chuck;
        // TODO: need to handle # channels other than just hoping they match
        // (might need to translate here between chuck # channels and unity # channels
        if( chuck->vm()->m_num_adc_channels != inchannels )
        {
            std::cout << "chuck in: " << chuck->vm()->m_num_adc_channels << " unity in: " << inchannels << std::endl;
        }
        else if( chuck->vm()->m_num_dac_channels != outchannels )
        {
            std::cout << "chuck out: " << chuck->vm()->m_num_dac_channels << " unity out: " << outchannels << std::endl;
        }
        else
        {
            chuck->run(inbuffer, outbuffer, length);
        }

        
        // Ringmod example
        /*float w = 2.0f * sinf(kPI * data->p[P_FREQ] / state->samplerate);
        for (unsigned int n = 0; n < length; n++)
        {
            for (int i = 0; i < outchannels; i++)
            {
                outbuffer[n * outchannels + i] = inbuffer[n * outchannels + i] * (1.0f - data->p[P_MIX] + data->p[P_MIX] * data->s);
            }
            data->s += data->c * w; // cheap way to calculate a steady sine-wave
            data->c -= data->s * w;
        }*/

#if UNITY_SPU
        UNITY_PS3_CELLDMA_PUT(&g_EffectData, state->effectdata, sizeof(g_EffectData));
#endif

        return UNITY_AUDIODSP_OK;
    }

#endif
}
