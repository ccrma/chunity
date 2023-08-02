using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityExampleRunCode : MonoBehaviour
{
    // This example runs a ChucK script every time
    // the space bar is pressed.

    ChuckSubInstance myChuck;

    // Use this for initialization
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
    }

    // Update is called once per frame
    void Update()
    {
        if( ChunityDemo.InteractWithDemo() )
        {
            // rotate my cube's transform
            transform.Rotate( new Vector3( 0, 15, 5 ) );

            // play a chuck script
            myChuck.RunCode( @"
                // print chuck version
                chout <= ""chuck version: "" <= Machine.version() <= IO.nl();

                // sound buffer player
                SndBuf buffy => dac;
                // TODO: broken on iOS?
                //""special:dope"" => buffy.read;

                // load file
                me.dir() + ""impact.wav"" => buffy.read;
                // play for the length of the file
                buffy.length() => now;
            " );
        }

    }
}
