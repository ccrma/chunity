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
        dacName = UTF8ToString( dacName );
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
        return theChuck.runCode( UTF8ToString( code ) );
    },
    runChuckCodeWithReplacementDac: function( chuckID, code, replacementDac )
    {
        return theChuck.runCodeWithReplacementDac( UTF8ToString( code ), UTF8ToString( replacementDac ) );
    },
    runChuckFile: function( chuckID, filename ) 
    {
        return theChuck.runFile( UTF8ToString( filename ) );
    },
    runChuckFileWithReplacementDac: function( chuckID, filename, replacementDac )
    {
        return theChuck.runFileWithReplacementDac( UTF8ToString( filename ), UTF8ToString( replacementDac ) );
    },
    runChuckFileWithArgs: function( chuckID, filename, args )
    {
        return theChuck.runFileWithArgs( UTF8ToString( filename ), UTF8ToString( args ) );
    },
    runChuckFileWithArgsWithReplacementDac: function( chuckID, filename, args, dacName )
    {
        return theChuck.runFileWithArgsWithReplacementDac( UTF8ToString( filename ), UTF8ToString( args ), UTF8ToString( dacName ) );
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
        return theChuck.setInt( UTF8ToString( name ), val );
    },
    getChuckInt: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getInt( UTF8ToString( name ) ).then( function( result )
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
        })(callback, UTF8ToString(name));
    },
    getChuckIntWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getInt( UTF8ToString( name ) ).then( function( result )
            {
                dynCall( 'vii', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getChuckIntWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getInt( UTF8ToString( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setChuckFloat: function( chuckID, name, val )
    {
        return theChuck.setFloat( UTF8ToString( name ), val );
    },
    getChuckFloat: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getFloat( UTF8ToString( name ) ).then( function( result )
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
        })(callback, UTF8ToString(name)); 
    },
    getChuckFloatWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getFloat( UTF8ToString( name ) ).then( function( result )
            {
                dynCall( 'vif', c, [i, result] );
            });
        })(callback, callbackID); 
    },
    getChuckFloatWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getFloat( UTF8ToString( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setChuckString: function( chuckID, name, val )
    {
        return theChuck.setString( UTF8ToString( name ), UTF8ToString( val ) );
    },
    getChuckString: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getString( UTF8ToString( name ) ).then( function( result ) {
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
        })(callback, UTF8ToString(name));
    },
    getChuckStringWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getString( UTF8ToString( name ) ).then( function( result ) {
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
            theChuck.getString( UTF8ToString( name ) ).then( function( result )
            {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    signalChuckEvent: function( chuckID, name )
    {
        return theChuck.signalEvent( UTF8ToString( name ) );
    },
    broadcastChuckEvent: function( chuckID, name )
    {
        return theChuck.broadcastEvent( UTF8ToString( name ) );
    },
    listenForChuckEventOnce: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.listenForEventOnce( UTF8ToString( name ), function() {
                dynCall( 'v', c, 0 );
            });
        })(callback);
        return true;
    },
    listenForNamedChuckEventOnce: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.listenForEventOnce( UTF8ToString( name ), function() {
                dynCall( 'vi', c, [n] );
            });
        })(callback, name);
        return true;
    },
    listenForChuckEventOnceWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.listenForEventOnce( UTF8ToString( name ), function() {
                dynCall( 'vi', c, [i] );
            });
        })(callback, callbackID);
        return true;
    },
    listenForChuckEventOnceWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.listenForEventOnce( UTF8ToString( name ), function()
            {
                unityInstance.SendMessage( g, m );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    startListeningForChuckEvent: function( chuckID, name, callback )
    {
        (function( c ) {
            var callbackID = theChuck.startListeningForEvent( UTF8ToString( name ), function() {
                dynCall( 'v', c, 0 );
            });
            this.stopIDs[ c ] = callbackID;
        })(callback);
        
        return true;
    },
    startListeningForNamedChuckEvent: function( chuckID, name, callback )
    {
        (function( c, n ) {
            var callbackID = theChuck.startListeningForEvent( UTF8ToString( name ), function() {
                dynCall( 'vi', c, [n] );
            });
            this.stopIDs[ c ] = callbackID;
        })(callback, name);
        return true;
    },
    startListeningForChuckEventWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            var listenerID = theChuck.startListeningForEvent( UTF8ToString( name ), function() {
                dynCall( 'vi', c, [i] );
            });
            this.stopIDs[ c ] = listenerID;
        })(callback, callbackID);
        return true;
    },
    startListeningForChuckEventWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            var callbackID = theChuck.startListeningForEvent( UTF8ToString( name ), function() {
                unityInstance.SendMessage( g, m );
            });
            this.stopIDs[ g + ":::" + m ] = callbackID;
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    stopListeningForChuckEvent: function( chuckID, name, callback )
    {
        var callbackID = this.stopIDs[ callback ];
        return theChuck.stopListeningForEvent( UTF8ToString( name ), callbackID );
    },
    stopListeningForNamedChuckEvent: function( chuckID, name, callback )
    {
        var callbackID = this.stopIDs[ callback ];
        return theChuck.stopListeningForEvent( UTF8ToString( name ), callbackID );
    },
    stopListeningForChuckEventWithID: function( chuckID, callbackID, name, callback )
    {
        var listenerID = this.stopIDs[ callback ];
        return theChuck.stopListeningForEvent( UTF8ToString( name ), listenerID );
    },
    stopListeningForChuckEventWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        var callbackID = this.stopIDs[ UTF8ToString( gameObject ) + ":::" + UTF8ToString( method ) ];
        theChuck.stopListeningForEvent( UTF8ToString( name ), callbackID );
    },

    // note: array is what Unity thinks is CKINT, which is 32 bit
    setGlobalIntArray__deps: ['cs32ArrayToJSArray'],
    setGlobalIntArray: function( chuckID, name, arrayValues, numValues )
    {
        return theChuck.setIntArray( UTF8ToString( name ), _cs32ArrayToJSArray( arrayValues, numValues ) );
    },
    // WebGL has no separate audio-thread path; alias to the regular setter
    setGlobalIntArray_AT__deps: ['setGlobalIntArray'],
    setGlobalIntArray_AT: function( chuckID, name, arrayValues, numValues )
    {
        return _setGlobalIntArray( chuckID, name, arrayValues, numValues );
    },
    getGlobalIntArray__deps: ['jsArrayToCS32Array'],
    getGlobalIntArray: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getIntArray( UTF8ToString( name ) ).then( function( result ) {
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
    getNamedGlobalIntArray__deps: ['jsArrayToCS32Array'],
    getNamedGlobalIntArray: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.getIntArray( n ).then( function( result ) {
                var nameBufferSize = lengthBytesUTF8( n ) + 1;
                var nameBuffer = _malloc( nameBufferSize );
                stringToUTF8( n, nameBuffer, nameBufferSize );
                var buffer = _malloc( 4 * result.length );
                _jsArrayToCS32Array( result, buffer );
                dynCall( 'vfii', c, [nameBuffer, buffer, result.length] );
                _free( buffer );
                _free( nameBuffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalIntArrayWithID__deps: ['jsArrayToCS32Array'],
    getGlobalIntArrayWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getIntArray( UTF8ToString( name ) ).then( function( result ) {
                var buffer = _malloc( 4 * result.length );
                _jsArrayToCS32Array( result, buffer );
                dynCall( 'viii', c, [i, buffer, result.length] );
                _free( buffer );
            });
        })(callback, callbackID);
    },
    getGlobalIntArrayWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getIntArray( UTF8ToString( name ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, JSON.stringify( { items: result } ) );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setGlobalIntArrayValue: function( chuckID, name, index, value )
    {
        return theChuck.setIntArrayValue( UTF8ToString( name ), index, value );
    },
    setGlobalIntArrayValue_AT__deps: ['setGlobalIntArrayValue'],
    setGlobalIntArrayValue_AT: function( chuckID, name, index, value )
    {
        return _setGlobalIntArrayValue( chuckID, name, index, value );
    },
    getGlobalIntArrayValue: function( chuckID, name, index, callback )
    {
        (function( c ) {
            theChuck.getIntArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                dynCall( 'vi', c, [result] );
            });
        })(callback);
    },
    getNamedGlobalIntArrayValue: function( chuckID, name, index, callback )
    {
        (function( c, n ) {
            theChuck.getIntArrayValue( n, index ).then( function( result ) {
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                dynCall( 'vfi', c, [buffer, result] );
                _free( buffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalIntArrayValueWithID: function( chuckID, callbackID, name, index, callback )
    {
        (function( c, i ) {
            theChuck.getIntArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                dynCall( 'vii', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getGlobalIntArrayValueWithUnityStyleCallback: function( chuckID, name, index, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getIntArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setGlobalAssociativeIntArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeIntArrayValue( UTF8ToString( name ), UTF8ToString( key ), value );
    },
    getGlobalAssociativeIntArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeIntArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                dynCall( 'vi', c, [result] );
            });
        })(callback);
    },
    getNamedGlobalAssociativeIntArrayValue: function( chuckID, name, key, callback )
    {
        (function( c, n ) {
            theChuck.getAssociativeIntArrayValue( n, UTF8ToString( key ) ).then( function( result ) {
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                dynCall( 'vfi', c, [buffer, result] );
                _free( buffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalAssociativeIntArrayValueWithID: function( chuckID, callbackID, name, key, callback )
    {
        (function( c, i ) {
            theChuck.getAssociativeIntArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                dynCall( 'vii', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getGlobalAssociativeIntArrayValueWithUnityStyleCallback: function( chuckID, name, key, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getAssociativeIntArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },

    // note: array is t_CKFLOAT == Float64 since that's what Unity thinks CKFLOAT is
    setGlobalFloatArray__deps: ['cs64FArrayToJSArray'],
    setGlobalFloatArray: function( chuckID, name, arrayValues, numValues )
    {
        return theChuck.setFloatArray( UTF8ToString( name ), _cs64FArrayToJSArray( arrayValues, numValues ) );
    },
    setGlobalFloatArray_AT__deps: ['setGlobalFloatArray'],
    setGlobalFloatArray_AT: function( chuckID, name, arrayValues, numValues )
    {
        return _setGlobalFloatArray( chuckID, name, arrayValues, numValues );
    },
    getGlobalFloatArray__deps: ['jsArrayToCS64FArray'],
    getGlobalFloatArray: function( chuckID, name, callback )
    {
        (function( c ) {
            theChuck.getFloatArray( UTF8ToString( name ) ).then( function( result ) {
                // need to malloc space for the array on the heap
                // assuming 64 bit floats, since that's what unity thinks is a float size!
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS64FArray( result, buffer );
                dynCall( 'vii', c, [buffer, result.length] );
                _free( buffer );
            });
        })(callback);
    },
    getNamedGlobalFloatArray__deps: ['jsArrayToCS64FArray'],
    getNamedGlobalFloatArray: function( chuckID, name, callback )
    {
        (function( c, n ) {
            theChuck.getFloatArray( n ).then( function( result ) {
                var nameBufferSize = lengthBytesUTF8( n ) + 1;
                var nameBuffer = _malloc( nameBufferSize );
                stringToUTF8( n, nameBuffer, nameBufferSize );
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS64FArray( result, buffer );
                dynCall( 'vfii', c, [nameBuffer, buffer, result.length] );
                _free( buffer );
                _free( nameBuffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalFloatArrayWithID__deps: ['jsArrayToCS64FArray'],
    getGlobalFloatArrayWithID: function( chuckID, callbackID, name, callback )
    {
        (function( c, i ) {
            theChuck.getFloatArray( UTF8ToString( name ) ).then( function( result ) {
                var buffer = _malloc( 8 * result.length );
                _jsArrayToCS64FArray( result, buffer );
                dynCall( 'viii', c, [i, buffer, result.length] );
                _free( buffer );
            });
        })(callback, callbackID);
    },
    getGlobalFloatArrayWithUnityStyleCallback: function( chuckID, name, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getFloatArray( UTF8ToString( name ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, JSON.stringify( { items: result } ) );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setGlobalFloatArrayValue: function( chuckID, name, index, value )
    {
        return theChuck.setFloatArrayValue( UTF8ToString( name ), index, value );
    },
    setGlobalFloatArrayValue_AT__deps: ['setGlobalFloatArrayValue'],
    setGlobalFloatArrayValue_AT: function( chuckID, name, index, value )
    {
        return _setGlobalFloatArrayValue( chuckID, name, index, value );
    },
    getGlobalFloatArrayValue: function( chuckID, name, index, callback )
    {
        (function( c ) {
            theChuck.getFloatArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                dynCall( 'vf', c, [result] );
            });
        })(callback);
    },
    getNamedGlobalFloatArrayValue: function( chuckID, name, index, callback )
    {
        (function( c, n ) {
            theChuck.getFloatArrayValue( n, index ).then( function( result ) {
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                dynCall( 'vff', c, [buffer, result] );
                _free( buffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalFloatArrayValueWithID: function( chuckID, callbackID, name, index, callback )
    {
        (function( c, i ) {
            theChuck.getFloatArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                dynCall( 'vif', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getGlobalFloatArrayValueWithUnityStyleCallback: function( chuckID, name, index, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getFloatArrayValue( UTF8ToString( name ), index ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    },
    setGlobalAssociativeFloatArrayValue: function( chuckID, name, key, value )
    {
        return theChuck.setAssociativeFloatArrayValue( UTF8ToString( name ), UTF8ToString( key ), value );
    },
    getGlobalAssociativeFloatArrayValue: function( chuckID, name, key, callback )
    {
        (function( c ) {
            theChuck.getAssociativeFloatArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                dynCall( 'vf', c, [result] );
            });
        })(callback);
    },
    getNamedGlobalAssociativeFloatArrayValue: function( chuckID, name, key, callback )
    {
        (function( c, n ) {
            theChuck.getAssociativeFloatArrayValue( n, UTF8ToString( key ) ).then( function( result ) {
                var bufferSize = lengthBytesUTF8( n ) + 1;
                var buffer = _malloc( bufferSize );
                stringToUTF8( n, buffer, bufferSize );
                dynCall( 'vff', c, [buffer, result] );
                _free( buffer );
            });
        })(callback, UTF8ToString(name));
    },
    getGlobalAssociativeFloatArrayValueWithID: function( chuckID, callbackID, name, key, callback )
    {
        (function( c, i ) {
            theChuck.getAssociativeFloatArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                dynCall( 'vif', c, [i, result] );
            });
        })(callback, callbackID);
    },
    getGlobalAssociativeFloatArrayValueWithUnityStyleCallback: function( chuckID, name, key, gameObject, method )
    {
        (function( g, m ) {
            theChuck.getAssociativeFloatArrayValue( UTF8ToString( name ), UTF8ToString( key ) ).then( function( result ) {
                unityInstance.SendMessage( g, m, result );
            });
        })( UTF8ToString( gameObject ), UTF8ToString( method ) );
    }


});
