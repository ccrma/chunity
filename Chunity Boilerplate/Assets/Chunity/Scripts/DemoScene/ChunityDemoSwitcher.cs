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
        #if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
        // only repsond to the initial touch
        if( Input.touchCount > 0 )
        {
            Touch touch = Input.GetTouch(0);
            if( touch.phase == TouchPhase.Began )
            {
                float horizontalPosition = touch.position.x;
                bool leftClick = horizontalPosition < 600;
                bool rightClick = horizontalPosition > 1800;
                // switch to previous or next demo
                if( leftClick || rightClick )
                {
                    // deactivate this one
                    DeactivateDemo( myDemos[myCurrentDemo] );
                    
                    // update demo number
                    if( leftClick )
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
        }

        #else
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
        #endif
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
