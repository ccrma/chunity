using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChunityDemoSwitcher : MonoBehaviour
{
    public Text myDemoText;

    private ChunityDemoDescription[] myDemos;
    private int myCurrentDemo;

    // Use this for initialization
    void Start()
    {
        // find demos
        myDemos = GetComponentsInChildren<ChunityDemoDescription>( includeInactive: true );
        myCurrentDemo = 0;

        // deactivate all
        foreach( ChunityDemoDescription demo in myDemos )
        {
            DeactivateDemo( demo );
        }

        ActivateDemo( myDemos[myCurrentDemo] );
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( "return" ) )
        {
            // deactivate current demo
            DeactivateDemo( myDemos[myCurrentDemo] );

            // switch to previous or next demo
            if( Input.GetKey( "left shift" ) || Input.GetKey( "right shift" ) )
            {
                myCurrentDemo += myDemos.Length - 1;
            }
            else
            {
                myCurrentDemo++;
            }
            myCurrentDemo %= myDemos.Length;

            // activate the new demo
            ActivateDemo( myDemos[myCurrentDemo] );
        }
    }

    void ActivateDemo( ChunityDemoDescription demo )
    {
        // display text
        myDemoText.text = demo.description;

        // activate demo
        demo.gameObject.SetActive( true );
    }

    void DeactivateDemo( ChunityDemoDescription demo )
    {
        demo.gameObject.SetActive( false );
    }
}
