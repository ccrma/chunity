mergeInto(LibraryManager.library, {
    initChuckScript: function()
    {
        this.chucks = {};
        this.subChucks = {};
        this.stopIDs = {};
        chuckPrint = function( text )
        {
            console.log( text );
        };
    },
    // helper function to turn csharp array pointer and length
    // into JS array
    cs64FArrayToJSArray: function ( csArray, csArrayLength )
    {
        var result = [];
        for( var i = 0; i < csArrayLength; i++ )
        {
            result.push( HEAPF64[(csArray >> 3) + i]);
        }
        return result;
    },

    // helper function to put jsArray onto heap where csharp pointer is
    jsArrayToCS64FArray: function( jsArray, csArray )
    {
        for( var i = 0; i < jsArray.length; i++ )
        {
            HEAPF64[(csArray >> 3) + i] = jsArray[i];
        }
    },
    // helper function to turn csharp array pointer and length
    // into JS array
    cs32ArrayToJSArray: function ( csArray, csArrayLength )
    {
        var result = [];
        for( var i = 0; i < csArrayLength; i++ )
        {
            result.push( HEAP32[(csArray >> 2) + i]);
        }
        return result;
    },

    // helper function to put jsArray onto heap where csharp pointer is
    jsArrayToCS32Array: function( jsArray, csArray )
    {
        for( var i = 0; i < jsArray.length; i++ )
        {
            HEAP32[(csArray >> 2) + i] = jsArray[i];
        }
    },
    initChuckInstance: function( chuckID, sampleRate )
    {
        // do nothing and secretly use theChuck

        // // ignore srate; it will be set to WebAudio's srate.
        // var thisChuckReady = defer();
        // createAChuck( chuckID, thisChuckReady ).then( function( newChuck ) {
        //     this.chucks[ chuckID ] = newChuck;
        //     this.chucks[ chuckID ].connect( audioContext.destination );
        // });
        // //await thisChuckReady;
        // return chuckID;
    },
    initSubChuckInstance: function( chuckID, subChuckID, dacName )
    {
        var thisSubChuckReady = defer();
        dacName = Pointer_stringify( dacName );
        /*await*/ theChuck.runCode( "global Gain " + dacName + " => blackhole; true => " + dacName + ".buffered;" );
        this.subChucks[ subChuckID ] = createASubChuck( theChuck, dacName, thisSubChuckReady );
        // TODO: connect to a panner when spatialization turned on.
        this.subChucks[ subChuckID ].connect( audioContext.destination );
        //await thisSubChuckReady;
        return subChuckID;
    },
    clearChuckInstance: function( chuckID )
    {
        return theChuck.clearChuckInstance();
    },
    clearGlobals: function( chuckID )
    {
        return theChuck.clearGlobals();
    },
    cleanupChuckInstance: function( chuckID )
    {
        // pass, oops
    },
    cleanRegisteredChucks: function()
    {
        // pass, oops
    },
    runChuckCode: function( chuckID, code )
    {
        return theChuck.runCode( Pointer_stringify( code ) );
    },
    runChuckCodeWithReplacementDac: function( chuckID, code, replacementDac )
    {
        return theChuck.runCodeWithReplacementDac( Pointer_stringify( code ), Pointer_stringify( replacementDac ) );
    },
    runChuckFile: function( chuckID, filename ) 
    {
        return theChuck.runFile( Pointer_stringify( filename ) );
    },
    runChuckFileWithReplacementDac: function( chuckID, filename, replacementDac )
    {
        return theChuck.runFileWithReplacementDac( Pointer_stringify( filename ), Pointer_stringify( replacementDac ) );
    },
    runChuckFileWithArgs: function( chuckID, filename, args )
    {
        return theChuck.runFileWithArgs( Pointer_stringify( filename ), Pointer_stringify( args ) );
    },
    runChuckFileWithArgsWithReplacementDac: function( chuckID, filename, args, dacName )
    {
        return theChuck.runFileWithArgsWithReplacementDac( Pointer_stringify( filename ), Pointer_stringify( args ), Pointer_stringify( dacName ) );
    },
    setChoutCallback: function( chuckID, callback )
    {
        // pass, oops
    },
    setCherrCallback: function( chuckID, callback )
    {
        // pass, oops
    },
    setStdoutCallback: function( callback )
    {
        // pass, oops
    }, 
    setStderrCallback: function( callback )
    {
        // pass, oops
    },
    setDataDir: function( dir )
    {
        // pass, oops
    },
    setLogLevel: function( level )
    {
        // pass, oops
    },

    setChuckInt: function( chuckID, name, val )
    {
        return theChuck.setInt( Pointer_stringify( name ), val );
    },
    getChuckInt: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getInt( Pointer_stringify( name ) ).then( function( result )
            {
                c( result );
            });
        })(callback); 
    },
    setChuckFloat: function( chuckID, name, val )
    {
        return theChuck.setFloat( Pointer_stringify( name ), val );
    },
    getChuckFloat: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getFloat( Pointer_stringify( name ) ).then( function( result )
            {
                c( result );
            });
        })(callback); 
    },
    setChuckString: function( chuckID, name, val )
    {
        return theChuck.setString( Pointer_stringify( name ), Pointer_stringify( val ) );
    },
    getChuckString: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getString( Pointer_stringify( name ) ).then( function( result ) {
                // need to turn JS result string into Module heap string.
                var bufferSize = lengthBytesUTF8( result ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( result, buffer, bufferSize );
                // send it along!
                c( buffer );
                // be nice to memory
                _free( buffer );
            });
        })(callback);
    },
    signalChuckEvent: function( chuckID, name )
    {
        return theChuck.signalEvent( Pointer_stringify( name ) );
    },
    broadcastChuckEvent: function( chuckID, name )
    {
        return theChuck.broadcastEvent( Pointer_stringify( name ) );
    },
    listenForChuckEventOnce: function( chuckID, name, callback )
    {
        return theChuck.listenForEventOnce( Pointer_stringify( name ), callback );
    },
    startListeningForChuckEvent: function( chuckID, name, callback )
    {
        var callbackID = theChuck.startListeningForEvent( Pointer_stringify( name ), callback );
        this.stopIDs[ callback ] = callbackID;
        return true;
    },
    stopListeningForChuckEvent: function( chuckID, name, callback )
    {
        var callbackID = this.stopIDs[ callback ];
        return theChuck.stopListeningForEvent( Pointer_stringify( name ), callbackID );
    },

    // note: array is what Unity thinks is CKINT, which is 32 bit
    setGlobalIntArray__deps: ['cs32ArrayToJSArray'],
    setGlobalIntArray: function( chuckID, name, arrayValues, numValues )
    {
        return theChuck.setIntArray( Pointer_stringify( name ), _cs32ArrayToJSArray( arrayValues, numValues ) );
    },
    getGlobalIntArray__deps: ['jsArrayToCS32Array'],
    getGlobalIntArray: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getIntArray( Pointer_stringify( name ) ).then( function( result ) {
                // need to malloc space for the array on the heap
                // assuming 64 bit ints, since that's what unity thinks is an int size!
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS32Array( result, buffer );
                c( buffer, result.length );
                _free( buffer );
            });
        })(callback);
    },
    setGlobalIntArrayValue: function( chuckID, name, index, value )
    {
        return theChuck.setIntArrayValue( Pointer_stringify( name ), index, value );
    },
    getGlobalIntArrayValue: function( chuckID, name, index, callback )
    {
        (function( c ) {
            theChuck.getIntArrayValue( Pointer_stringify( name ), index ).then( function( result ) {
                c( result );
            });
        })(callback);
    },
    setGlobalAssociativeIntArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeIntArrayValue( Pointer_stringify( name ), Pointer_stringify( key ), value );
    },
    getGlobalAssociativeIntArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeIntArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                c( result );
            });
        })(callback);
    },

    // note: array is t_CKFLOAT == Float64 since that's what Unity thinks CKFLOAT is
    setGlobalFloatArray__deps: ['cs64ArrayToJSArray'],
    setGlobalFloatArray: function( chuckID, name, arrayValues, numValues )
    {
        return theChuck.setFloatArray( Pointer_stringify( name ), _cs64ArrayToJSArray( arrayValues, numValues ) );
    },
    getGlobalFloatArray__deps: ['jsArrayToCS64Array'],
    getGlobalFloatArray: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getFloatArray( Pointer_stringify( name ) ).then( function( result ) {
                // need to malloc space for the array on the heap
                // assuming 64 bit floats, since that's what unity thinks is a float size!
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS64Array( result, buffer );
                c( buffer, result.length );
                _free( buffer );
            });
        })(callback);
    },
    setGlobalFloatArrayValue: function( chuckID, name, index, value )
    {
        return theChuck.setFloatArrayValue( Pointer_stringify( name ), index, value );
    },
    getGlobalFloatArrayValue: function( chuckID, name, index, callback )
    {
        (function( c ) {
            theChuck.getFloatArrayValue( Pointer_stringify( name ), index ).then( function( result ) {
                c( result );
            });
        })(callback);
    },
    setGlobalAssociativeFloatArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeFloatArrayValue( Pointer_stringify( name ), Pointer_stringify( key ), value );
    },
    getGlobalAssociativeFloatArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeFloatArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                c( result );
            });
        })(callback);
    }


});
