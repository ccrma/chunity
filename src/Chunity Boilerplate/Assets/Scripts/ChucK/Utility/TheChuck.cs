using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheChuck : MonoBehaviour
{
	public static ChuckMainInstance theChuck = null;

	void Awake()
	{
		if( theChuck == null )
		{
			theChuck = GetComponent<ChuckMainInstance>();
		}
	}
}
