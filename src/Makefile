COREDIR=chuck/src/core


SRCS=AudioPluginUtil.cpp \
Plugin_ChucK.cpp \
Plugin_Spatializer.cpp \
hrtftable.cpp
OUTPUT=libAudioPluginDemo.so
CXXFLAGS=-I. -Ichuck/src/core -D__PLATFORM_LINUX__ -O2 -fPIC
LDFLAGS=-shared -rdynamic -fPIC
CXX=g++

#include $(COREDIR)/makefile $(MAKECMDGOALS)


########################## CHUCK CORE LIB TARGETS ##############################
COBJS_CORE+= chuck.tab.o chuck.yy.o util_math.o util_network.o util_raw.o \
	util_xforms.o
CXXOBJS_CORE+= chuck.o chuck_absyn.o chuck_parse.o chuck_errmsg.o \
	chuck_frame.o chuck_symbol.o chuck_table.o chuck_utils.o \
	chuck_vm.o chuck_instr.o chuck_scan.o chuck_type.o chuck_emit.o \
	chuck_compile.o chuck_dl.o chuck_oo.o chuck_lang.o chuck_ugen.o \
	chuck_otf.o chuck_stats.o chuck_shell.o chuck_io.o hidio_sdl.o \
	midiio_rtmidi.o rtmidi.o ugen_osc.o ugen_filter.o \
	ugen_stk.o ugen_xxx.o ulib_machine.o ulib_math.o ulib_std.o \
	ulib_opsc.o ulib_regex.o util_buffers.o util_console.o \
	util_string.o util_thread.o util_opsc.o util_serial.o \
	util_hid.o uana_xform.o uana_extract.o
LO_COBJS_CORE+= lo/address.o lo/blob.o lo/bundle.o lo/message.o lo/method.o \
	lo/pattern_match.o lo/send.o lo/server.o lo/server_thread.o lo/timetag.o


############################ OBJECT FILE TARGETS ###############################
COBJS=$(COBJS_HOST) $(addprefix $(COREDIR)/,$(COBJS_CORE))
CXXOBJS=$(CXXOBJS_HOST) $(addprefix $(COREDIR)/,$(CXXOBJS_CORE))
LO_COBJS=$(addprefix $(COREDIR)/,$(LO_COBJS_CORE))
SF_COBJS=$(addprefix $(COREDIR)/,$(SF_CSRCS:.c=.o))
CK_OBJS=$(COBJS) $(CXXOBJS) $(LO_COBJS) $(SF_COBJS)

################################################################################

# pull in dependency info for *existing* .o files
-include $(OBJS:.o=.d)

OBJS=$(SRCS:.cpp=*.o)

all: $(OUTPUT)
clean:
	rm -f $(OUTPUT) $(OBJS)

$(OUTPUT): $(OBJS) 
	$(CXX) $(LDFLAGS) -o $(OUTPUT) $(OBJS) $(CK_OBJS)

.cpp.o:
	$(CXX) $(CXXFLAGS) -c $<

	
