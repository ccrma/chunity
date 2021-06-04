using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunityDemo
{
    public static bool InteractWithDemo()
	{
		#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
		// is there a finger on the screen?
		if( Input.touchCount > 0 )
        {
			// is this the first time it touched down?
            Touch touch = Input.GetTouch(0);
			if( touch.phase == TouchPhase.Began )
			{
				// center touch only
				return ( touch.position.x >= 600 && touch.position.x <= 1800 );
			}
		}
		// do not interact
		return false;
		#else
        // on computer, use space bar
		return Input.GetKeyDown( "space" );
		#endif
	}
}
