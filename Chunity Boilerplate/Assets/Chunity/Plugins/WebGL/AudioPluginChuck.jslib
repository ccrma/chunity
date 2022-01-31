mergeInto(LibraryManager.library, {
    initChuckScript: function()
    {
        this.chucks = {};
        this.subChucks = {};
        this.panners = {};
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
        HEAPF64.set( jsArray, csArray >> 3 );
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
        // appears to be not working correctly
        // e.g. [a,b,c,d,e,f,g,h] becomes [a,c,e,g,junk,junk,junk,junk]
        HEAP32.set( jsArray, csArray >> 2 );
        // both of these result in complete garbage
        // HEAPU8.set( jsArray, csArray );
        // HEAPU8.set( jsArray.buffer, csArray );
    },
    initChuckInstance: function( chuckID, sampleRate )
    {
        // right now, we are secretly using theChuck
        // so clear it as if it's a new chuck
        theChuck.clearChuckInstance();
        theChuck.clearGlobals();

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
        theChuck.runCode( "global Gain " + dacName + " => blackhole; true => " + dacName + ".buffered;" );
        this.subChucks[ subChuckID ] = createASubChuck( theChuck, dacName, thisSubChuckReady );
        this.subChucks[ subChuckID ].connect( audioContext.destination );
        this.subChucks[ subChuckID ].currentlySpatialized = false;

        //await thisSubChuckReady;
        return subChuckID;
    },
    muteSubChuckInstance: function( subChuckID )
    {
        this.subChucks[ subChuckID ].disconnect();
    },
    unMuteSubChuckInstance: function( subChuckID )
    {
        this.subChucks[ subChuckID ].disconnect();
        if( this.subChucks[ subChuckID ].currentlySpatialized )
        {
            this.subChucks[ subChuckID ].connect( this.panners[ subChuckID ] );
        }
        else
        {
            this.subChucks[ subChuckID ].connect( audioContext.destination );
        }
    },
    initSpatializer: function( subChuckID, minDistance, maxDistance )
    {
        this.panners[ subChuckID ] = new PannerNode( audioContext,
        {
            panningModel: 'equalpower',
            distanceModel: 'inverse',
            positionX: 0,
            positionY: 0,
            positionZ: 0,
            orientationX: 0,
            orientationY: 0,
            orientationZ: -1,
            refDistance: minDistance,
            maxDistance: maxDistance,
            rolloffFactor: 1,
            coneInnerAngle: 360,
            coneOuterAngle: 360,
            coneOuterGain: 1
        });
        this.panners[ subChuckID ].connect( audioContext.destination );
        return subChuckID;
    },
    // NOTE: Unity uses left-handed cartesian coordinates (forward is z++)
    // and WebAudio uses right-handed cartesian coordinates (forward is z--)
    // It is necessary to convert by negating every z component.
    setListenerTransform: function( x, y, z, forwardX, forwardY, forwardZ, upX, upY, upZ )
    {
        // set position
        if( chunityAudioListener.positionX ) 
        {
            // most browsers
            chunityAudioListener.positionX.setValueAtTime( x, audioContext.currentTime );
            chunityAudioListener.positionY.setValueAtTime( y, audioContext.currentTime );
            chunityAudioListener.positionZ.setValueAtTime( -z, audioContext.currentTime );
        }
        else
        {
            // firefox still uses deprecated API
            chunityAudioListener.setPosition( x, y, -z );
        }

        // set orientation
        if( chunityAudioListener.forwardX )
        {
            // most browsers
            chunityAudioListener.forwardX.setValueAtTime( forwardX, audioContext.currentTime );
            chunityAudioListener.forwardY.setValueAtTime( forwardY, audioContext.currentTime );
            chunityAudioListener.forwardZ.setValueAtTime( -forwardZ, audioContext.currentTime );
            chunityAudioListener.upX.setValueAtTime( upX, audioContext.currentTime );
            chunityAudioListener.upY.setValueAtTime( upY, audioContext.currentTime );
            chunityAudioListener.upZ.setValueAtTime( -upZ, audioContext.currentTime );
        }
        else
        {
            // firefox still uses deprecated API
            chunityAudioListener.setOrientation( forwardX, forwardY, -forwardZ, upX, upY, -upZ );
        }

    },
    // NOTE: Unity uses left-handed cartesian coordinates (forward is z++)
    // and WebAudio uses right-handed cartesian coordinates (forward is z--)
    // It is necessary to convert by negating every z component.
    setSubChuckTransform: function( subChuckID, posX, posY, posZ, forwardX, forwardY, forwardZ )
    {
        // set position
        if( this.panners[ subChuckID ].positionX )
        {
            // most browsers
            this.panners[ subChuckID ].positionX.setValueAtTime( posX, audioContext.currentTime );
            this.panners[ subChuckID ].positionY.setValueAtTime( posY, audioContext.currentTime );
            this.panners[ subChuckID ].positionZ.setValueAtTime( -posZ, audioContext.currentTime );
        }
        else
        {
            // firefox still uses deprecated API
            this.panners[ subChuckID ].setPosition( posX, posY, -posZ );
        }

        // set forward direction
        if( this.panners[ subChuckID ].orientationX )
        {
            // most browsers
            this.panners[ subChuckID ].orientationX.setValueAtTime( forwardX, audioContext.currentTime );
            this.panners[ subChuckID ].orientationY.setValueAtTime( forwardY, audioContext.currentTime );
            this.panners[ subChuckID ].orientationZ.setValueAtTime( -forwardZ, audioContext.currentTime );
        }
        else
        {
            // firefox still uses deprecated API
            this.panners[ subChuckID ].setOrientation( forwardX, forwardY, -forwardZ );
        }
    },
    // TODO what other values might we want to set?
    setSubChuckSpatializationParameters: function( subChuckID, doSpatialization, minDistance, maxDistance, rolloffFactor )
    {
        if( doSpatialization && !this.subChucks[ subChuckID ].currentlySpatialized )
        {
            this.subChucks[ subChuckID ].disconnect( audioContext.destination );
            this.subChucks[ subChuckID ].connect( this.panners[ subChuckID ] );
        }
        else if( !doSpatialization && this.subChucks[ subChuckID ].currentlySpatialized )
        {
            this.subChucks[ subChuckID ].disconnect( this.panners[ subChuckID ] );
            this.subChucks[ subChuckID ].connect( audioContext.destination );
        }
        this.subChucks[ subChuckID ].currentlySpatialized = doSpatialization;

        this.panners[ subChuckID ].refDistance = minDistance;
        this.panners[ subChuckID ].maxDistance = maxDistance;
        this.panners[ subChuckID ].rolloffFactor = rolloffFactor;
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
                dynCall( 'vi', c, [result] );
            });
        })(callback);
    },
    getNamedChuckInt: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.getInt( n ).then( function( result )
            {
                // need to turn JS name string into Module heap string.
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                // send it along!
                dynCall( 'vfi', c, [buffer, result] );
                // be nice to memory
                _free( buffer );
            });
        })(callback, Pointer_stringify(name));
    },
    getChuckIntWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getInt( Pointer_stringify( name ) ).then( function( result )
            {
                dynCall( 'vii', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getChuckIntWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getInt( Pointer_stringify( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
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
                dynCall( 'vf', c, [result] );
            });
        })(callback); 
    },
    getNamedChuckFloat: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.getFloat( n ).then( function( result )
            {
                // need to turn JS name string into Module heap string.
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                // send it along!
                dynCall( 'vff', c, [buffer, result] );
                // be nice to memory
                _free( buffer );
            });
        })(callback, Pointer_stringify(name)); 
    },
    getChuckFloatWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getFloat( Pointer_stringify( name ) ).then( function( result )
            {
                dynCall( 'vif', c, [i, result] );
            });
        })(callback, callbackID); 
    },
    getChuckFloatWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getFloat( Pointer_stringify( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
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
                dynCall( 'vf', c, [buffer] );
                // be nice to memory
                _free( buffer );
            });
        })(callback);
    },
    getNamedChuckString: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.getString( n ).then( function( result ) {
                // need to turn JS result string into Module heap string.
                var bufferSize = lengthBytesUTF8( result ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( result, buffer, bufferSize );

                // and name
                var nameBufferSize = lengthBytesUTF8( n );
                var nameBuffer = _malloc( nameBufferSize );
                stringToUTF8( n, nameBuffer, nameBufferSize );

                // send it along!
                dynCall( 'vff', c, [nameBuffer, buffer] );
                // be nice to memory
                _free( buffer );
                _free( nameBuffer );
            });
        })(callback, Pointer_stringify(name));
    },
    getChuckStringWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getString( Pointer_stringify( name ) ).then( function( result ) {
                // need to turn JS result string into Module heap string.
                var bufferSize = lengthBytesUTF8( result ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( result, buffer, bufferSize );
                // send it along!
                dynCall( 'vif', c, [i, buffer] );
                // be nice to memory
                _free( buffer );
            });
        })(callback, callbackID);
    },
    getChuckStringWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getString( Pointer_stringify( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
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
        (function( c ) {
            theChuck.listenForEventOnce( Pointer_stringify( name ), function() {
                dynCall( 'v', c, 0 );
            });
        })(callback);
        return true;
    },
    listenForNamedChuckEventOnce: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.listenForEventOnce( Pointer_stringify( name ), function() {
                dynCall( 'vi', c, [n] );
            });
        })(callback, name);
        return true;
    },
    listenForChuckEventOnceWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.listenForEventOnce( Pointer_stringify( name ), function() {
                dynCall( 'vi', c, [i] );
            });
        })(callback, callbackID);
        return true;
    },
    listenForChuckEventOnceWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.listenForEventOnce( Pointer_stringify( name ), function()
            {
                unityInstance.SendMessage( g, m );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    },
    startListeningForChuckEvent: function( chuckID, name, callback )
    {
        (function( c ) {
            var callbackID = theChuck.startListeningForEvent( Pointer_stringify( name ), function() {
                dynCall( 'v', c, 0 );
            });
            this.stopIDs[ c ] = callbackID;
        })(callback);
        
        return true;
    },
    startListeningForChuckEventWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            var callbackID = theChuck.startListeningForEvent( Pointer_stringify( name ), function() {
                unityInstance.SendMessage( g, m );
            });
            this.stopIDs[ g + ":::" + m ] = callbackID;
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    },
    stopListeningForChuckEvent: function( chuckID, name, callback )
    {
        var callbackID = this.stopIDs[ callback ];
        return theChuck.stopListeningForEvent( Pointer_stringify( name ), callbackID );
    },
    stopListeningForChuckEventWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        var callbackID = this.stopIDs[ Pointer_stringify( gameObject ) + ":::" + Pointer_stringify( method ) ];
        theChuck.stopListeningForEvent( Pointer_stringify( name ), callbackID );
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
                console.log( "JS thinks the GET int array is ", result );
                // need to malloc space for the array on the heap
                // assuming 32 bit ints, since that's what unity thinks is an int size!
                var buffer = _malloc( 4 * result.length );
                _jsArrayToCS32Array( result, buffer );
                dynCall( 'vii', c, [buffer, result.length] );
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
                dynCall( 'vi', c, [result] );
            });
        })(callback);
    },
    getGlobalIntArrayValueWithUnityStyleCallback: function( chuckID, name, index, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getIntArrayValue( Pointer_stringify( name ), index ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    },
    setGlobalAssociativeIntArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeIntArrayValue( Pointer_stringify( name ), Pointer_stringify( key ), value );
    },
    getGlobalAssociativeIntArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeIntArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                dynCall( 'vi', c, [result] );
            });
        })(callback);
    },
    getGlobalAssociativeIntArrayValueWithUnityStyleCallback: function( chuckID, name, key, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getAssociativeIntArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    },

    // note: array is t_CKFLOAT == Float64 since that's what Unity thinks CKFLOAT is
    setGlobalFloatArray__deps: ['cs64FArrayToJSArray'],
    setGlobalFloatArray: function( chuckID, name, arrayValues, numValues )
    {
        return theChuck.setFloatArray( Pointer_stringify( name ), _cs64FArrayToJSArray( arrayValues, numValues ) );
    },
    getGlobalFloatArray__deps: ['jsArrayToCS64FArray'],
    getGlobalFloatArray: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getFloatArray( Pointer_stringify( name ) ).then( function( result ) {
                // need to malloc space for the array on the heap
                // assuming 64 bit floats, since that's what unity thinks is a float size!
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS64FArray( result, buffer );
                dynCall( 'vii', c, [buffer, result.length] );
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
                dynCall( 'vf', c, [result] );
            });
        })(callback);
    },
    getGlobalFloatArrayValueWithUnityStyleCallback: function( chuckID, name, index, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getFloatArrayValue( Pointer_stringify( name ), index ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    },
    setGlobalAssociativeFloatArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeFloatArrayValue( Pointer_stringify( name ), Pointer_stringify( key ), value );
    },
    getGlobalAssociativeFloatArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeFloatArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                dynCall( 'vf', c, [result] );
            });
        })(callback);
    },
    getGlobalAssociativeFloatArrayValueWithUnityStyleCallback: function( chuckID, name, key, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getAssociativeFloatArrayValue( Pointer_stringify( name ), Pointer_stringify( key ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( Pointer_stringify( gameObject ), Pointer_stringify( method ) );
    }


});
