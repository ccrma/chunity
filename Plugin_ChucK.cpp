//
//  Plugin_ChucK.cpp
//  AudioPluginDemo
//
//  Created by Jack Atherton on 4/19/17.
//
//


#include "Plugin_ChucK.h"

#include <iostream>
#include <map>
#include <unistd.h>

namespace ChucK
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
    
    std::map< unsigned int, Chuck_System * > chuck_instances;
    std::map< unsigned int, EffectData::Data * > data_instances;
    std::string chuck_external_data_dir;



    // C# "string" corresponds to passing char *
    UNITY_INTERFACE_EXPORT bool runChuckCode( unsigned int chuckID, const char * code )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }
        
        return Chuck_External::runCode( chuck_instances[chuckID], code );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChuckInt( unsigned int chuckID, const char * name, t_CKINT val )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::setExternalInt( chuck_instances[chuckID], name, val );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getChuckInt( unsigned int chuckID, const char * name, void (* callback)(t_CKINT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::getExternalInt( chuck_instances[chuckID], name, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChuckFloat( unsigned int chuckID, const char * name, t_CKFLOAT val )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::setExternalFloat( chuck_instances[chuckID], name, val );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool getChuckFloat( unsigned int chuckID, const char * name, void (* callback)(t_CKFLOAT) )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::getExternalFloat( chuck_instances[chuckID], name, callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool signalChuckEvent( unsigned int chuckID, const char * name )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::signalExternalEvent( chuck_instances[chuckID], name );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool broadcastChuckEvent( unsigned int chuckID, const char * name )
    {
        if( chuck_instances.count( chuckID ) == 0 ) { return false; }

        return Chuck_External::broadcastExternalEvent( chuck_instances[chuckID], name );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setChoutCallback( void (* callback)(const char *) )
    {
        return Chuck_External::setChoutCallback( callback );
    }



    UNITY_INTERFACE_EXPORT bool setCherrCallback( void (* callback)(const char *) )
    {
        return Chuck_External::setCherrCallback( callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setStdoutCallback( void (* callback)(const char *) )
    {
        return Chuck_External::setStdoutCallback( callback );
    }



    UNITY_INTERFACE_EXPORT bool setStderrCallback( void (* callback)(const char *) )
    {
        return Chuck_External::setStderrCallback( callback );
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool setDataDir( const char * dir )
    {
        chuck_external_data_dir = std::string( dir );
        return true;
    }
    
    
    
    UNITY_INTERFACE_EXPORT bool initChuckInstance( unsigned int chuckID )
    {
        if( chuck_instances.count( chuckID ) == 0 )
        {
            // if we aren't tracking a chuck vm on this ID, create a new one
            chuck_instances[chuckID] = Chuck_External::startChuck( chuck_external_data_dir.c_str() );
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
        for( std::map< unsigned int, Chuck_System * >::iterator it =
             chuck_instances.begin(); it != chuck_instances.end(); it++ )
        {
            Chuck_System * chuck = it->second;
            Chuck_External::quitChuck( chuck );
        }
        
        // delete stored chuck pointers
        chuck_instances.clear();
        // delete data instances
        data_instances.clear();
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
    
    
    // Called when the plugin is loaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
    {
        // Things that need to be common to ALL ChucK instances will be loaded here
        // (I am pretty sure this is not called or is not called reliably.)
    }
    
    
    // Called when the plugin is about to be unloaded.
    void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
    {
        // Things that need to be common to ALL ChucK instances will be unloaded here
        Chuck_External::finalCleanup();
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
            Chuck_System * chuck = chuck_instances[data->myId];

            Chuck_External::audioCallback( chuck, inbuffer, outbuffer, length, inchannels, outchannels );
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
