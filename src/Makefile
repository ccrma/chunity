SRCS=AudioPluginUtil.cpp \
Plugin_ChucK.cpp \
Plugin_Spatializer.cpp \
Plugin_SpatializerReverb.cpp \
hrtftable.cpp \
chuck/src/core/chuck.cpp \
chuck/src/core/chuck_absyn.cpp \
chuck/src/core/chuck_parse.cpp \
chuck/src/core/chuck_errmsg.cpp \
chuck/src/core/chuck_frame.cpp \
chuck/src/core/chuck_symbol.cpp \
chuck/src/core/chuck_table.cpp \
chuck/src/core/chuck_utils.cpp \
chuck/src/core/chuck_vm.cpp \
chuck/src/core/chuck_instr.cpp \
chuck/src/core/chuck_scan.cpp \
chuck/src/core/chuck_type.cpp \
chuck/src/core/chuck_emit.cpp \
chuck/src/core/chuck_compile.cpp \
chuck/src/core/chuck_dl.cpp \
chuck/src/core/chuck_oo.cpp \
chuck/src/core/chuck_lang.cpp \
chuck/src/core/chuck_ugen.cpp \
chuck/src/core/chuck_otf.cpp \
chuck/src/core/chuck_stats.cpp \
chuck/src/core/chuck_shell.cpp \
chuck/src/core/chuck_globals.cpp \
chuck/src/core/chuck_io.cpp \
chuck/src/core/chuck_carrier.cpp \
chuck/src/core/hidio_sdl.cpp \
chuck/src/core/midiio_rtmidi.cpp \
chuck/src/core/rtmidi.cpp \
chuck/src/core/ugen_osc.cpp \
chuck/src/core/ugen_filter.cpp \
chuck/src/core/ugen_stk.cpp \
chuck/src/core/ugen_xxx.cpp \
chuck/src/core/ulib_ai.cpp \
chuck/src/core/ulib_machine.cpp \
chuck/src/core/ulib_math.cpp \
chuck/src/core/ulib_std.cpp \
chuck/src/core/ulib_opsc.cpp \
chuck/src/core/ulib_regex.cpp \
chuck/src/core/util_buffers.cpp \
chuck/src/core/util_console.cpp \
chuck/src/core/util_string.cpp \
chuck/src/core/util_thread.cpp \
chuck/src/core/util_opsc.cpp \
chuck/src/core/util_serial.cpp \
chuck/src/core/util_hid.cpp \
chuck/src/core/uana_xform.cpp \
chuck/src/core/uana_extract.cpp

# chuck.yy.?? chuck.tab.??
CSRCS=chuck/src/core/util_math.c \
chuck/src/core/util_network.c \
chuck/src/core/util_raw.c \
chuck/src/core/util_xforms.c \
chuck/src/core/chuck.tab.c \
chuck/src/core/chuck.yy.c

SF_CSRCS=chuck/src/core/util_sndfile.c

LO_CSRCS=chuck/src/core/lo/address.c \
chuck/src/core/lo/blob.c \
chuck/src/core/lo/bundle.c \
chuck/src/core/lo/message.c \
chuck/src/core/lo/method.c \
chuck/src/core/lo/pattern_match.c \
chuck/src/core/lo/send.c \
chuck/src/core/lo/server.c \
chuck/src/core/lo/server_thread.c \
chuck/src/core/lo/timetag.c



OBJS=$(SRCS:.cpp=.o)
COBJS=$(CSRCS:.c=.o)
SF_COBJS=$(SF_CSRCS:.c=.o)
LO_COBJS=$(LO_CSRCS:.c=.o)
OUTPUT=libAudioPluginChuck.so

CXXFLAGS=-I. -Ichuck/src/core -Ichuck/src/core/lo -D__PLATFORM_LINUX__ -O2 -fPIC -fno-strict-aliasing -D__CK_SNDFILE_NATIVE__ -DHAVE_CONFIG_H


LDFLAGS=-shared -rdynamic -fPIC 
LDFLAGS2=-lstdc++ -ldl -lm -lpthread -lsndfile -lasound
LEX=flex
YACC=bison
CXX=gcc


all: $(OUTPUT)
clean:
	rm -f $(OUTPUT) $(OBJS) $(COBJS)

chuck/src/core/chuck.tab.c chuck/src/core/chuck.tab.h: chuck/src/core/chuck.y
	$(YACC) -dv -b chuck/src/core/chuck chuck/src/core/chuck.y

chuck/src/core/chuck.yy.c: chuck/src/core/chuck.lex
	$(LEX) -ochuck/src/core/chuck.yy.c chuck/src/core/chuck.lex

$(OUTPUT): chuck/src/core/chuck.tab.c chuck/src/core/chuck.tab.h chuck/src/core/chuck.yy.c $(OBJS) $(COBJS) $(SF_COBJS) $(LO_COBJS)
	$(CXX) $(LDFLAGS) -o $(OUTPUT) $(OBJS) $(COBJS) $(SF_COBJS) $(LO_COBJS) $(LDFLAGS2)
	cp $(OUTPUT) ../Assets/Plugins/

.cpp.o:
	$(CXX) $(CXXFLAGS) -c -o $@ $<

.c.o:
	$(CXX) $(CXXFLAGS) -c -o $@ $<

