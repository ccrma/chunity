LOCAL_PATH := $(call my-dir)

# necessary to obtain correct file output name
include $(CLEAR_VARS) 

LOCAL_MODULE := AudioPluginChuck

# src unnecessary as we removed all chuck-as-mixer-plugin functionality for android
#../../AudioPluginUtil.cpp

LOCAL_SRC_FILES := ../../Plugin_ChucK.cpp \
../../chuck/src/core/chuck.cpp \
../../chuck/src/core/chuck_absyn.cpp \
../../chuck/src/core/chuck_parse.cpp \
../../chuck/src/core/chuck_errmsg.cpp \
../../chuck/src/core/chuck_frame.cpp \
../../chuck/src/core/chuck_symbol.cpp \
../../chuck/src/core/chuck_table.cpp \
../../chuck/src/core/chuck_utils.cpp \
../../chuck/src/core/chuck_vm.cpp \
../../chuck/src/core/chuck_instr.cpp \
../../chuck/src/core/chuck_scan.cpp \
../../chuck/src/core/chuck_type.cpp \
../../chuck/src/core/chuck_emit.cpp \
../../chuck/src/core/chuck_compile.cpp \
../../chuck/src/core/chuck_dl.cpp \
../../chuck/src/core/chuck_oo.cpp \
../../chuck/src/core/chuck_lang.cpp \
../../chuck/src/core/chuck_ugen.cpp \
../../chuck/src/core/chuck_stats.cpp \
../../chuck/src/core/chuck_carrier.cpp \
../../chuck/src/core/hidio_sdl.cpp \
../../chuck/src/core/ugen_osc.cpp \
../../chuck/src/core/ugen_filter.cpp \
../../chuck/src/core/ugen_stk.cpp \
../../chuck/src/core/ugen_xxx.cpp \
../../chuck/src/core/ulib_machine.cpp \
../../chuck/src/core/ulib_math.cpp \
../../chuck/src/core/ulib_std.cpp \
../../chuck/src/core/ulib_opsc.cpp \
../../chuck/src/core/ulib_regex.cpp \
../../chuck/src/core/util_buffers.cpp \
../../chuck/src/core/util_console.cpp \
../../chuck/src/core/util_string.cpp \
../../chuck/src/core/util_thread.cpp \
../../chuck/src/core/util_opsc.cpp \
../../chuck/src/core/util_serial.cpp \
../../chuck/src/core/util_hid.cpp \
../../chuck/src/core/uana_xform.cpp \
../../chuck/src/core/uana_extract.cpp \
../../chuck/src/core/util_math.c \
../../chuck/src/core/util_network.c \
../../chuck/src/core/util_raw.c \
../../chuck/src/core/util_xforms.c \
../../chuck/src/core/chuck.tab.c \
../../chuck/src/core/chuck.yy.c \
../../chuck/src/core/util_sndfile.c \
../../chuck/src/core/lo/address.c \
../../chuck/src/core/lo/blob.c \
../../chuck/src/core/lo/bundle.c \
../../chuck/src/core/lo/message.c \
../../chuck/src/core/lo/method.c \
../../chuck/src/core/lo/pattern_match.c \
../../chuck/src/core/lo/send.c \
../../chuck/src/core/lo/server.c \
../../chuck/src/core/lo/server_thread.c \
../../chuck/src/core/lo/timetag.c


LOCAL_CPP_FEATURES := rtti exceptions

LOCAL_C_INCLUDES := $(LOCAL_PATH)/../../ $(LOCAL_PATH)/../../chuck/src/core $(LOCAL_PATH)/../../chuck/src/core/lo

LOCAL_CFLAGS += -D__PLATFORM_LINUX__ -D__ANDROID__ -D__DISABLE_OTF_SERVER__ -D__DISABLE_FILEIO__ -D__DISABLE_SERIAL__ -D__DISABLE_MIDI__ -D__DISABLE_SHELL__ -DHAVE_CONFIG_H -fPIC -fno-strict-aliasing 

#TODO: -lsndfile for SndBuf ? -lasound for MIDI (currently disabled)?
LOCAL_LDLIBS := -lstdc++ -ldl -lm 

LOCAL_LDFLAGS += -shared -rdynamic -fPIC

include $(BUILD_SHARED_LIBRARY)
