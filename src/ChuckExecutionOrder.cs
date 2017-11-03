using UnityEditor;

[InitializeOnLoad]
public class ChuckExecutionOrder : Editor
{
	static ChuckExecutionOrder()
	{
		// Get the name of the script we want to change it's execution order
		string chuckInstance = typeof(ChuckInstance).Name;
		string chuckMainInstance = typeof(ChuckMainInstance).Name;
		string chuckSubInstance = typeof(ChuckSubInstance).Name;

		SetExecutionOrder( chuckInstance, -32000 );
		SetExecutionOrder( chuckMainInstance, -31000 );
		SetExecutionOrder( chuckSubInstance, -30000 );

	}

	static void SetExecutionOrder( string scriptName, int order )
	{
		// Iterate through all scripts (Might be a better way to do this?)
		foreach( MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts() )
		{
			// If found our script
			if( monoScript.name == scriptName )
			{
				// And it's not at the execution time we want already
				// (Without this we will get stuck in an infinite loop)
				if( MonoImporter.GetExecutionOrder( monoScript ) != order )
				{
					MonoImporter.SetExecutionOrder( monoScript, order );
				}
				break;
			}
		}
	}
}